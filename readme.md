# example-webapp-with-auth

- [features](#features)
- [quickstart (dev)](#quickstart-dev)
  - [clone sources and install template](#clone-sources-and-install-template)
  - [create project source tree](#create-project-source-tree)
  - [configure project](#configure-project)
  - [configuration parameters for mail server](#configuration-parameters-for-mail-server)
  - [start debug](#start-debug)
- [prerequisites](#prerequisites)
  - [development database setup](#development-database-setup)
  - [create selfsigned cert](#create-selfsigned-cert)
  - [setup development nginx](#setup-development-nginx)
  - [adjust local dns](#adjust-local-dns)
  - [install root ca for local development](#install-root-ca-for-local-development)
- [dev notes](#dev-notes)
  - [JWT auth access and refresh token](#jwt-auth-access-and-refresh-token)
  - [backend](#backend)
    - [run tests](#run-tests)
    - [configuration parameters](#configuration-parameters)
    - [add more migrations](#add-more-migrations)
    - [db dia gen](#db-dia-gen)
  - [frontend](#frontend)
    - [configuration parameters](#configuration-parameters-1)
    - [openapi usage](#openapi-usage)
    - [invoke api](#invoke-api)
  - [white papers](#white-papers)
    - [reset password](#reset-password)
    - [user manager](#user-manager)
    - [username and password criteria](#username-and-password-criteria)
    - [predefined roles and permissions](#predefined-roles-and-permissions)
- [production deployment](#production-deployment)
  - [db machine prerequisite](#db-machine-prerequisite)
  - [ssh config on development machine](#ssh-config-on-development-machine)
  - [target machine](#target-machine)
  - [publish to target machine](#publish-to-target-machine)
- [how this project was built](#how-this-project-was-built)

<hr/>

## features

- Security
  - Development with https self signed wildcard certs
  - JWT auth flags `secure`, `httponly`, `samesite` strict
  - Roles `admin`, `advanced`, `normal` with static [UserPermission][2] matrix

- Backend
  - C# asp net core
  - Configuration
    - development user-secrets
    - appsettings.json, appsettings.[Environment].json ( autoreload on change )
    - environment variables

- Frontend
  - Typescript react frontend + vite tooling
  - React redux `GlobalState` for current user
  - Layout with responsive appbar, public and protected pages with react router dom
  - Openapi typescript/axios generate from backend swagger endpoint
  - Login / Logout / Reset lost password through email link
  - User manager with auth controller [edit user][3] to create, edit username, password, email, roles, disable
  - Light/Dark themes, Snacks

- Debugging
  - backend and frontend debugging in a solution from the same IDE

- Production
  - publish release with frontend webpacked available through server static files available directly from within backend
  - publish deployment script with systemd service and environment secrets

## quickstart (dev)

- see [**prerequisites**](#prerequisites) to setup self signed dev cert and nginx proxy

### clone sources and install template

- clone repo

```sh
git clone https://github.com/devel0/example-webapp-with-auth.git
cd example-webapp-with-auth
dotnet new install .
cd ..
```

### create project source tree

```sh
dotnet new webapp-with-auth -n project-folder --namespace My.Some
cd project-folder
source misc/restore-permissions.sh
dotnet build
```

### configure project

- set shell variables replacing *REPL_* vars

```sh
SEED_ADMIN_EMAIL=REPL_ADMIN_EMAIL
SEED_ADMIN_PASS="REPL_ADMIN_PASS"
DB_PROVIDER="Postgres"
DB_CONN_STRING="Host=localhost; Database=ExampleWebApp; Username=example_webapp_user; Password=$(cat ~/security/devel/ExampleWebApp/postgres-user)"
JWTKEY="$(openssl rand -hex 32)"
```

- set development user secrets

```sh
cd WebApiServer
dotnet user-secrets init
dotnet user-secrets set "SeedUsers:Admin:Email" "$SEED_ADMIN_EMAIL"
dotnet user-secrets set "SeedUsers:Admin:Password" "$SEED_ADMIN_PASS"
dotnet user-secrets set "DbProvider" "$DB_PROVIDER"
dotnet user-secrets set "ConnectionStrings:Main" "$DB_CONN_STRING"
dotnet user-secrets set "JwtSettings:Key" "$JWTKEY"
cd ..
```

### configuration parameters for mail server

- to be able to use the reset password feature configure also the smtp server

```sh
cd WebApiServer
dotnet user-secrets set "EmailServer:SmtpServerName" REPL_MAILSERVER_HOSTNAME
dotnet user-secrets set "EmailServer:SmtpServerPort" REPL_MAILSERVER_PORT
dotnet user-secrets set "EmailServer:Security" REPL_MAILSERVER_SECURITY
dotnet user-secrets set "EmailServer:Username" REPL_MAILSERVER_USER_EMAIL
dotnet user-secrets set "EmailServer:Password" REPL_MAILSERVER_USER_PASSWORD
cd ..
```

accepted values for `EmailServer:Security` are `Tls`, `Ssl`, `Auto`, `None`.

### start debug

```sh
code .
```

- choose `.NET Core Launch (web)` from run and debug then hit F5 ( this will start asp net web server on `https://webapp-test.searchathing.com/swagger/index.html` )

- restore client node modules

```sh
cd clientapp
npm i
cd ..
```

- start frontend

```sh
./run-frontend.sh
```

- choose `Launch Chrome` from run and debug then click the play icon ( this will start browser )

- try to login/current user/logout/current user button from frontend

- login page

![](./doc/login.png)

- master page

![](./doc/main-page.png)

- user manager

![](./doc/user-manager.png)

## prerequisites

### development database setup

- create db secrets

```sh
apt install pwgen
mkdir -p ~/security/devel/ExampleWebApp
chmod 700 ~/security
pwgen -s 12 -n 1 > ~/security/devel/postgres
echo "$(pwgen -s 12 -n 1)#" > ~/security/devel/ExampleWebApp/admin
pwgen -s 12 -n 1 > ~/security/devel/ExampleWebApp/postgres-user
echo "localhost:*:*:postgres:$(cat ~/security/devel/postgres)" >> ~/.pgpass
chmod 600 ~/.pgpass
```  

- install postgres as docker and psql client in the host

```sh
docker volume create pgdata
docker run -e POSTGRES_PASSWORD=`cat ~/security/devel/postgres` --restart=unless-stopped --name postgres -v pgdata:/var/lib/postgresql/data -d -p 5432:5432/tcp postgres:latest
apt install postgresql-client-16
```  

- this will allow you to connect to localhost postgres db as postgres user ( test with `psql -h localhost -U postgres` if connects )

- create postgres `example_webapp_user` user with capability to createdb

- local db setup

```sh
echo "CREATE USER example_webapp_user WITH PASSWORD '$(cat ~/security/devel/ExampleWebApp/postgres-user)' CREATEDB" | psql -h localhost -U postgres
```

### create selfsigned cert

- clone [linux-scripts-utils](https://github.com/devel0/linux-scripts-utils)

```sh
mkdir -p ~/opensource
cd ~/opensource
git clone git@github.com:devel0/linux-scripts-utils.git
export PATH=$PATH:~/opensource/linux-scripts-utils
mkdir -p ~/sscerts
chmod 700 ~/sscerts
```

- create cert parameters file `~/sscerts/searchathing.com.params` ( replace `searchathing.com` with your own )

```
COUNTRY="IT"
STATE="Italy"
CITY="Trento"
ORGNAME="SearchAThing"
ORGUNIT="Development"
DOMAIN=localhost
DURATION_DAYS=36500 # 100 years
```

- create root-ca certificates

```sh
CERTPARAMS=~/sscerts/searchathing.com.params create-root-ca.sh
```

- generated root-ca files

| file                             | description                                                                             |
| -------------------------------- | --------------------------------------------------------------------------------------- |
| `~/sscerts/searchathing.com.crt` | root-ca certificate that you can register into the browser to trust linked certificates |
| `~/sscerts/searchathing.com.key` | key of the root-ca certificte ( this is NOT NEEDED anywhere, do not share )             |

- create test certificates ( note: this generate a wildcard certificate `*.yourdomain`, so can be reused for other development projects )

```sh
CERTPARAMS=~/sscerts/searchathing.com.params create-cert.sh --add-empty --add-wildcard
```

| file                                              | description                                  |
| ------------------------------------------------- | -------------------------------------------- |
| `~/sscerts/searchathing.com/searchathing.com.crt` | is the certificate crt for nginx https proxy |
| `~/sscerts/searchathing.com/searchathing.com.key` | is the certificate key for nginx https proxy |

### setup development nginx

- install nginx

```sh
apt install nginx
```

- create `/etc/nginx/conf.d/dev-webapp-test.conf` by symlink [webapp-test.conf](./deploy/nginx/dev/webapp-test.conf)

```sh
cd /etc/nginx/conf.d
ln -s PATH_TO/example-webapp-with-auth/deploy/nginx/dev/webapp-test.conf dev-webapp-test.conf
```

### adjust local dns

- edit `/etc/hosts`

```sh
127.0.0.1  localhost

#-----------------------------------
# DEVELOPMENT
#-----------------------------------
127.0.0.1 dev-webapp-test.searchathing.com
```

### install root ca for local development

Installing root-ca certificate imply that certificates generated within that will be consequently trusted.

- **chrome**: settings/Privacy and security/Security/Manage certificates/Authorities/Import
  - select `~/sscerts/searchathing.com_CA.crt`
  - tick `Trust this certificate for identifying websites`

- **firefox**: settings/Privacy & Security/Certificates/View Certificates/Authorities/Import
  - select `~/sscerts/searchathing.com_CA.crt`
  - tick `Trust this CA to identify websites`
  
- **shell**
  - copy `~/sscerts/searchathing.com_CA.crt` to `/usr/local/share/ca-certificates`
  - issue `sudo update-ca-certificates`

## dev notes

### JWT auth access and refresh token

- authentication and authorization are managed entirely by the backend, in fact the frontend doesn't store any access token or restore token in local storage ; from the frontend side point of view the authentication is transparently managed through the browser `X-Access-Token` and `X-Refresh-Token` that the [server][23] sets after successful login through `Set-Cookie` header ( the frontend only call the login and logout webapi without storing anything on javascript side ):
  - XSS ( Cross-site scripting ) attack are prevented because the absence of access token from the local storage makes javascript unable to read these token
  - CSRF ( Cross-site request forgery ) attack are prevented because the cookie is stored within follow attributes
    - `secure` : prevent the cookie to be stored against a phising site because https will identify the server autenticity
    - `httponly` : prevent the javascript to read the cookie ( only the browser can handle by sending through the request header )
    - `samesite strict` : prevent to send the access token to other servers
- web api controller methods are executable only from user with valid access token because of the [`[Authorize]`][24] attribute ; further refinement can require user to have one or more roles through the attribute specialization with [`Roles`][25]. To allow anonymous api use [`[AllowAnonymous]`][26] attribute.
- use of the access token allow the server to authenticate the user by reading user, role and other info contained in the token itself; note that these info are not encrypted and can be viewed, but the token contains a signature that can't be generated from other than the server that contains the JWT key to create the signature itself. In other words the server validate the access token and signature match considering as valid the provided identity informations ( because it was the server itself that signed the data no other could generate corresponding signature ). This requires less hardware resources than using a db to validate the user.
- for paranoid setting the expiration of an access token should short and this maintain ability to execute high rate operations retaining the ability to block a user within a short response time. In fact a valid access token can't revoked by default rule but having a short time of validity allow the server to ban any other authorized api for that user simply disabling it. In fact after user is disabled the process of renew of another access token, even with a valid refresh token ( that has longer expire time ) gets [disabled immediately][27].
- more, when a refresh token is used to renew an access token it gets rotated invalidating the previous one

### backend

#### run tests

- configure unit test db settings

```sh
cd WebApiServer

TEST_DB_CONN_STRING="Host=localhost; Database=ExampleWebAppTest; Username=example_webapp_user; Password=$(cat ~/security/devel/ExampleWebApp/postgres-user)"

dotnet user-secrets set "ConnectionStrings:UnitTest" "$TEST_DB_CONN_STRING"

cd ..
```

- to run tests

```sh
dotnet test
```

or run specific test with ( replace `TEST` with one from `dotnet test -t` )

```sh
dotnet test --filter=TEST
```

#### configuration parameters

| param name                              | description                                                                                                                | example                                                                            |
| --------------------------------------- | -------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------- |
| AppServerName                           | Used to build [app url][10] for the reset password link.                                                                   | "dev-webapp-test.searchathing.com"                                                 |
| DbProvider                              | Used to [inject db provider service][12].                                                                                  | "Postgres"                                                                         |
| ConnectionStrings:Main                  | Used to build application [db context datasource][11].                                                                     | "Host=localhost; Database=ExampleWebApp; Username=postgres; Password=somepass"     |
| IsUnitTest                              | Used to build [unit test application datasource][11] in unit test mode. Will be set to `true` from the [test factory][13]. | false                                                                              |
| ConnectionStrings:UnitTest              | Need to be set in order to run unit tests. Warning: database referred by this conn string will be dropped during tests.    | "Host=localhost; Database=ExampleWebAppTest; Username=postgres; Password=somepass" |
| JwtSettings:Key                         | Symmetric key for JWT signature generation.                                                                                | (results from `openssl rand -hex 32` command)                                      |
| JwtSettings:Issuer                      | Issuer of the JWT access token.                                                                                            | "https://www.example.com"                                                          |
| JwtSettings:Audience                    | Audience of the JWT access token.                                                                                          | "https://www.example.com/app"                                                      |
| JwtSettings:AccessTokenDurationSeconds  | JWT access token duration (seconds)                                                                                        | 300                                                                                |
| JwtSettings:RefreshTokenDurationSeconds | JWT refresh token duration (seconds)                                                                                       | 1200                                                                               |
| JwtSettings:ClockSkewSeconds            | JWT access token clock skew (seconds)                                                                                      | 0                                                                                  |
| SeedUsers:Admin:UserName                | Default seeded admin username                                                                                              | admin                                                                              |
| SeedUsers:Admin:Password                | Default seeded admin password                                                                                              | SomePass1!                                                                         |
| SeedUsers:Admin:Email                   | Default seeded admin email                                                                                                 | admin@some.com                                                                     |
| EmailServer:Username                    | Email server config used in reset password ( account username )                                                            | server@some.com                                                                    |
| EmailServer:Password                    | Email server config used in reset password ( account password )                                                            |                                                                                    |
| EmailServer:SmtpServerName              | Email server config used in reset password ( account smtp server )                                                         | smtp@some.com                                                                      |
| EmailServer:SmtpServerPort              | Email server config used in reset password ( account smtp port )                                                           | 587                                                                                |
| EmailServer:Security                    | Email server config used in reset password ( account protocol security )                                                   | "Tls"                                                                              |
| EmailServer:FromDisplayName             | Email server config used in reset password ( account displayname of the sender )                                           | "Server"                                                                           |

The configuration is setup through [SetupAppSettings][14] method in order to evaluate:
- `appsettings.json`
- `appsettings.ENV.json` ( where ENV is the executing environment, ie. `Development`, `Production`, ... )
- environment variables replacing `:` with `__` ( used for [example][15] in the production environment )
- user secrets used in development environment

The configuration of appsettings json files will reapplied on change automatically even at runtime ( note that in debug environment you need to change appsettings json files that are inside `WebApiServer/bin/Debug/net8.0` folder )

#### add more migrations

```sh
./migr.sh add MIGRNAME
```

#### db dia gen

database diagram can be generated through [gen-db-dia.sh](doc/gen-db-dia.sh) script that uses schemacrawler ( [more][1] )

![](./doc/db/db.png)

### frontend

#### configuration parameters

Configuration parameters for the frontend can be set at compile-time through [.env.development](./clientapp/.env.development) and [.env.production](./clientapp/.env.production) files depending on the build mode.

| param name         | description                |
| ------------------ | -------------------------- |
| VITE_SERVERNAME    | used to build [api url][9] |
| VITE_GITCOMMIT     | git commit short sha       |
| VITE_GITCOMMITDATE | git commit date            |

note that `VITE_GITCOMMIT` and `VITE_GITCOMMITDATE` gets automatically updated by the [publish.sh](./publish.sh) script for the `.env.production` configuration file.

#### openapi usage

- start the backend

```sh
cd example-webapp-with-auth/WebApiServer
dotnet run
```

- generate Typescript/axios frontend api

```sh
cd example-webapp-with-auth
./gen-api.sh
```

- browse through swagger interface ( avail in development environment ) ie. https://dev-webapp-test.searchathing.com/swagger

#### invoke api

- foreach ControlleBase api will generated through `gen-api.sh`

- create a related api reference ( [example][4] )

- invoke with try, catch using [handleApiException][5] helper to report problem on the gui through snacks

```ts
try {
    const res = await someApi.apiSomeGet({
        param: value,
        param2: value2,        
    })
    
    console.log('successful api invocation')

} catch (_ex) {
    handleApiException(_ex as ResponseError)
}
```

### white papers

#### reset password

- on the login page there is a "Lost password ?" button

![](./doc/password-reset-step1.png)

- clicking on that button, having the email field filled with a previously registered user, cause the [frontend][6] to invoke the [ResetLostPassword][7] auth controller anonymous access api.

- this controller api method in turn uses the authentication service [ResetLostPasswordRequestAsync][8] method; this works as follow
  - retrieve existing user by given email
  - retrieve configuration parameters for mail server
  - retrieve configuration parameter for app servername in order to build a reset url like the follow
  `https://webapp-test.searchathing.com/app/Login/:from/RESET_TOKEN` ( `:from` parameter will considered null )
  - email with reset password link sent
  
- gui snack notification

![](./doc/password-reset-step1-feedback.png)

- email received

![](./doc/password-reset-step2.png)

- the mail link will open the browser at the login page with the [token param][16] and this cause the form to appears as follow

![](./doc/password-reset-step3.png)

- inserting the corresponding email address now and a new password this will be reset through the call of the [ResetLostPassword][7] auth controller anonymous access api again but within non null token and resetPassword.
- then the authentication service `ResetLostPasswordRequestAsync` will [finish the rule][17] this way
  - execute the auth service `LoginAsync` with username, resetPassword and optional argument token with a non null value in order to [reset the user passowrd][18]

#### user manager

- if the user current user has permission to create user with some specific role use the `Create` button from the user manager

![](./doc/create-user-form.png)

- to edit an existing user click on the `Edit` button

![](./doc/edit-user-form.png)

#### username and password criteria

- these can be overriden at compile time [here][19].
- the gui will inherit username and password rules through the [AuthOptions][20] service invoke by the sama name auth controller method. These will be evaluated during user editing [here][21].

#### predefined roles and permissions

| permission/role           | admin | advanced | normal |
| ------------------------- | ----- | -------- | ------ |
| ChangeUserRoles           | ■     |          |        |
| CreateAdminUser           | ■     |          |        |
| CreateAdvancedUser        | ■     |          |        |
| CreateNormalUser          | ■     | ■        |        |
| ChangeOwnEmail            | ■     | ■        | ■      |
| ChangeOwnPassword         | ■     | ■        | ■      |
| ChangeNormalUserEmail     | ■     | ■        |        |
| ChangeAdvancedUserEmail   | ■     |          |        |
| ChangeAdminUserEmail      | ■     |          |        |
| ResetNormalUserPassword   | ■     | ■        |        |
| ResetAdvancedUserPassword | ■     |          |        |
| ResetAdminUserPassword    | ■     |          |        |
| LockoutAdminUser          | ■     |          |        |
| LockoutAdvancedUser       | ■     |          |        |
| LockoutNormalUser         | ■     | ■        |        |
| DeleteAdminUser           | ■     |          |        |
| DeleteAdvancedUser        | ■     |          |        |
| DeleteNormalUser          | ■     | ■        |        |
| DisableAdminUser          | ■     |          |        |
| DisableAdvancedUser       | ■     |          |        |
| DisableNormalUser         | ■     | ■        |        |
| ResetLostPassword         | ■     | ■        | ■      |

## production deployment

- change `linux-x64` with target platform: `linux-x64`, `win-x64`, `osx-x64`

```sh
dotnet publish -c Release --runtime linux-x64 --sc
```

- note: option `--sc` makes self contained with all required runtimes ( ie. no need to install dotnet runtime on the target platform )

- published files will be in `WebApiServer/bin/Release/net8.0/linux-x64/publish/`

### db machine prerequisite

```sh
apt install postgres
su - postgres
psql
postgres=# CREATE USER webapp_test_user WITH ENCRYPTED PASSWORD 'DBPASS' CREATEDB;
CREATE ROLE
```

- tune postgres host allowed `/etc/postgresql/16/main/my.conf`

```sh
listen_addresses = '*'
```

- tune postgres db permissions `/etc/postgresql/16/main/pg_hba.conf` ( replace `TARGETMACHINEIP` with ip of the target machine where the app will run )

```sh
# TYPE  DATABASE        USER                  ADDRESS                 METHOD
host    webapp_test     webapp_test_user      TARGETMACHINEIP/32      scram-sha-256
host    postgres        webapp_test_user      TARGETMACHINEIP/32      scram-sha-256
```

### ssh config on development machine

```sh
Host main-test
  HostName TARGETMACHINEIP
  User root
  IdentityFile ~/.ssh/main-test.id_rsa
```

- append `~/.ssh/main-test.id_rsa.pub` content to the target machine `/root/.ssh/authorized_keys`

### target machine

- from target machine:

```sh
apt install openssh-server rsync nginx
useradd -m user
mkdir /root/secrets
```

### publish to target machine

the syntax is

```sh
devel0@tuf:~/opensource/example-webapp-with-auth$ ./publish.sh 
argmuments:
  -h <sshhost>        ssh host where to publish ( ie. main-test )
  -sn <servername>    nginx app servername ( ie. webapp-test.searchathing.com )
  -id <appid>         app identifier ( ie. webapp-test )
  -f                  force overwrite existing
```

then invoke

```sh
./publish.sh -h main-test -sn webapp-test.searchathing.com -id webapp-test
```

## how this project was built

- started from clone from [example web app](https://github.com/devel0/example-webapp/blob/e9328b16212f1d128518088bb8a2c4b620c2035e/readme.md#how-this-project-was-built)

- more frontend pkgs

```sh
mkdir example-webapp-with-auth
cd example-webapp-with-auth
git init
dotnet new gitignore
dotnet new webapi -n WebApiServer
npm create vite@latest clientapp -- --template react-ts
cd clientapp
npm i --save-dev @vitejs/plugin-react
npm i @mui/material @emotion/react @emotion/styled @mui/icons-material
npm i @reduxjs/toolkit react-redux react-router-dom axios linq-to-typescript usehooks-ts @fontsource/roboto
cd ..
dotnet new sln
dotnet sln add WebApiServer
dotnet build
```

- create app dbcontext

```sh
cd example-webapp-with-auth
dotnet new classlib -n AppDbContext
dotnet sln add AppDbContext
cd AppDbContext
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 8.0.5
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.5
```

- create app dbcontext migration

```sh
cd example-webapp-with-auth
dotnet new classlib -n AppDbMigrationsPsql
dotnet sln add AppDbMigrationsPsql
cd AppDbMigrationsPsql
dotnet add package Microsoft.EntityFrameworkCore.Relational --version 8.0.7
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.4
dotnet add reference ../AppDbContext
```

- add db pkgs to webapi sever

```sh
cd example-webapp-with-auth
cd WebApiServer
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.7
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.7
dotnet add package Microsoft.IdentityModel.Tokens --version 8.0.1
dotnet add package System.IdentityModel.Tokens.Jwt --version 8.0.1
dotnet add reference ../AppDbContext
dotnet add reference ../AppDbMigrationsPsql
```

- init ef tools

```sh
dotnet new tool-manifest
dotnet tool install dotnet-ef
```

- init secrets

```sh
openssl rand -hex 32 > ~/security/devel/ExampleWebApp/jwt.key

dotnet user-secrets init
dotnet user-secrets set "JwtSettings:Key" "$(cat ~/security/devel/ExampleWebApp/jwt.key)"

SEED_ADMIN_EMAIL=admin@admin.com
SEED_ADMIN_PASS=$(cat ~/security/devel/ExampleWebApp/admin)

DB_PROVIDER="Postgres"
DB_CONN_STRING="Host=localhost; Database=ExampleWebApp; Username=example_webapp_user; Password=$(cat ~/security/devel/ExampleWebApp/postgres-user)"

dotnet user-secrets set "SeedUsers:Admin:Email" "$SEED_ADMIN_EMAIL"
dotnet user-secrets set "SeedUsers:Admin:Password" "$SEED_ADMIN_PASS"
dotnet user-secrets set "DbProvider" "$DB_PROVIDER"
dotnet user-secrets set "ConnectionStrings:Main" "$DB_CONN_STRING"
```

- coding... ( create app db context and models )

- add initial migration

```sh
cd example-webapp-with-auth
cd WebApiServer
dotnet ef migrations add init --project ../AppDbMigrationsPsql -- --provider Postgres
```

- Add integration tests

```sh
cd example-webapp-with-auth
dotnet new xunit -n Test
cd Test
dotnet add package Microsoft.AspNetCore.Mvc.Testing --version 8.0.8
```

[1]: https://github.com/devel0/knowledge/blob/168e6cec6fdc0298b21d758c198d6f9210032ba8/doc/psql-schema-crawler.md
[2]: https://github.com/devel0/example-webapp-with-auth/blob/d3685ff088fde20254e385c5ebcc13cd3dda6f2e/WebApiServer/Types/UserPermission.cs#L124
[3]: https://github.com/devel0/example-webapp-with-auth/blob/5d10bf357e6e256df16b9a517c113043dd15750f/WebApiServer/DTOs/EditUserRequestDto.cs#L6
[4]: https://github.com/devel0/example-webapp-with-auth/blob/48959f45ddd2871ad2105cd8cf35128ef6136a72/clientapp/src/axios.manager.ts#L56
[5]: https://github.com/devel0/example-webapp-with-auth/blob/d3685ff088fde20254e385c5ebcc13cd3dda6f2e/clientapp/src/utils/utils.tsx#L17
[6]: https://github.com/devel0/example-webapp-with-auth/blob/d3685ff088fde20254e385c5ebcc13cd3dda6f2e/clientapp/src/pages/LoginPage.tsx#L182
[7]: https://github.com/devel0/example-webapp-with-auth/blob/d3685ff088fde20254e385c5ebcc13cd3dda6f2e/WebApiServer/Controllers/AuthController.cs#L193
[8]: https://github.com/devel0/example-webapp-with-auth/blob/31544b0b02a8be1211941416e70f3d6fb4cef44e/WebApiServer/Implementations/AuthService.cs#L778
[9]: https://github.com/devel0/example-webapp-with-auth/blob/d3685ff088fde20254e385c5ebcc13cd3dda6f2e/clientapp/src/constants/general.ts#L5
[10]: https://github.com/devel0/example-webapp-with-auth/blob/31544b0b02a8be1211941416e70f3d6fb4cef44e/WebApiServer/Implementations/AuthService.cs#L817
[11]: https://github.com/devel0/example-webapp-with-auth/blob/d3685ff088fde20254e385c5ebcc13cd3dda6f2e/WebApiServer/Extensions/DatabaseService.cs#L13
[12]: https://github.com/devel0/example-webapp-with-auth/blob/d3685ff088fde20254e385c5ebcc13cd3dda6f2e/WebApiServer/Extensions/DatabaseService.cs#L29
[13]: https://github.com/devel0/example-webapp-with-auth/blob/31544b0b02a8be1211941416e70f3d6fb4cef44e/Test/TestFactory.cs#L45
[14]: https://github.com/devel0/example-webapp-with-auth/blob/a04204f9014596509dcacd1af04a8579000d2fd6/WebApiServer/Extensions/AppSettingsService.cs#L13
[15]: https://github.com/devel0/example-webapp-with-auth/blob/23ecdb344e60008aab30c38c7a8c56357d6101ef/deploy/webapp-test.env#L4
[16]: https://github.com/devel0/example-webapp-with-auth/blob/d3685ff088fde20254e385c5ebcc13cd3dda6f2e/clientapp/src/pages/LoginPage.tsx#L38
[17]: https://github.com/devel0/example-webapp-with-auth/blob/31544b0b02a8be1211941416e70f3d6fb4cef44e/WebApiServer/Implementations/AuthService.cs#L788-L807
[18]: https://github.com/devel0/example-webapp-with-auth/blob/9a94258665f2314ea77b5c662344803cc4b8dc86/WebApiServer/Implementations/AuthService.cs#L100
[19]: https://github.com/devel0/example-webapp-with-auth/blob/5d10bf357e6e256df16b9a517c113043dd15750f/WebApiServer/Extensions/Auth.cs#L89
[20]: https://github.com/devel0/example-webapp-with-auth/blob/9a94258665f2314ea77b5c662344803cc4b8dc86/WebApiServer/Implementations/AuthService.cs#L41
[21]: https://github.com/devel0/example-webapp-with-auth/blob/9ba6d6599ad9f73548ced7335f945b59cb339e4f/clientapp/src/utils/password-validator.ts#L4
[22]: https://github.com/devel0/example-webapp-with-auth/blob/d3685ff088fde20254e385c5ebcc13cd3dda6f2e/WebApiServer/Types/UserPermission.cs#L128
[23]: https://github.com/devel0/example-webapp-with-auth/blob/adcabbb20a10091c56210183179f7bd7dd64359c/WebApiServer/Implementations/AuthService.cs#L151
[24]: https://github.com/devel0/example-webapp-with-auth/blob/a04204f9014596509dcacd1af04a8579000d2fd6/WebApiServer/Controllers/MainController.cs#L7
[25]: https://github.com/devel0/example-webapp-with-auth/blob/a04204f9014596509dcacd1af04a8579000d2fd6/WebApiServer/Controllers/MainController.cs#L27
[26]: https://github.com/devel0/example-webapp-with-auth/blob/adcabbb20a10091c56210183179f7bd7dd64359c/WebApiServer/Controllers/AuthController.cs#L62
[27]: https://github.com/devel0/example-webapp-with-auth/blob/4111643a52aa7f19c531ddcf88132d7d59c0b683/WebApiServer/Extensions/Auth.cs#L179
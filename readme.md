# example-webapp-with-auth

- [features](#features)
- [quickstart (dev)](#quickstart-dev)
  - [local db](#local-db)
  - [vscode debug](#vscode-debug)
- [prerequisites](#prerequisites)
  - [create selfsigned cert](#create-selfsigned-cert)
  - [setup development nginx](#setup-development-nginx)
  - [adjust local dns](#adjust-local-dns)
  - [install root ca for local development](#install-root-ca-for-local-development)
- [dev notes](#dev-notes)
  - [backend](#backend)
    - [run tests](#run-tests)
    - [add more migrations](#add-more-migrations)
    - [db dia gen](#db-dia-gen)
  - [frontend](#frontend)
    - [openapi usage](#openapi-usage)
    - [invoke api](#invoke-api)
- [production deployment](#production-deployment)
  - [db machine prerequisite](#db-machine-prerequisite)
  - [ssh config on development machine](#ssh-config-on-development-machine)
  - [target machine](#target-machine)
  - [publish to target machine](#publish-to-target-machine)
- [how this project was built](#how-this-project-was-built)

<hr/>

## features

- asp net core backend
- react frontend + vite tooling
- https self signed cert development
- backend and frontend debugging in a solution
- configuration user-secrets, environment variables, and appsettings.json, appsettings.[Environment].json with autoreload on change
- jwt auth secure, httponly, strict samesite
- user roles admin, advanced, normal with static [UserPermission][2] matrix
- auth controller [edit user][3] to create, edit username, password, email, roles, lockout
- react redux
- login public page and protected routes
- send email for lost password feature
- user manager gui with user role permissions related capabilities
- theme light/dark, snacks
- publish release with frontend webpacked available through server static files available directly from within backend

## quickstart (dev)

### local db

- see [prerequisites](#prerequisites) to setup self signed dev cert and nginx proxy

- clone repo

```sh
git clone https://github.com/devel0/example-webapp-with-auth.git
cd example-webapp-with-auth
```

- local db setup

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

- config user secrets replacing *REPL_* vars

```sh
cd example-webapp-with-auth/WebApiServer

SEED_ADMIN_EMAIL=REPL_ADMIN_EMAIL
SEED_ADMIN_PASS=REPL_ADMIN_PASS
DB_PROVIDER="Postgres"
DB_CONN_STRING="Host=localhost; Database=REPL_DBNAME; Username=REPL_DBUSER; Password=REPL_DBPASS"
JWTKEY="$(openssl rand -hex 32)"

dotnet user-secrets init
dotnet user-secrets set "SeedUsers:Admin:Email" "$SEED_ADMIN_EMAIL"
dotnet user-secrets set "SeedUsers:Admin:Password" "$SEED_ADMIN_PASS"
dotnet user-secrets set "DbProvider" "$DB_PROVIDER"
dotnet user-secrets set "ConnectionStrings:Main" "$DB_CONN_STRING"
dotnet user-secrets set "JwtSettings:Key" "$JWTKEY"
```

- to be able to use the reset password feature configure also the smtp server

```sh
dotnet user-secrets set "EmailServer:SmtpServerName" REPL_MAILSERVER_HOSTNAME
dotnet user-secrets set "EmailServer:SmtpServerPort" REPL_MAILSERVER_PORT
dotnet user-secrets set "EmailServer:Security" REPL_MAILSERVER_SECURITY
dotnet user-secrets set "EmailServer:Username" REPL_MAILSERVER_USER_EMAIL
dotnet user-secrets set "EmailServer:Password" REPL_MAILSERVER_USER_PASSWORD
```

accepted values for `EmailServer:Security` are `Tls`, `Ssl`, `Auto`, `None`.

- install postgres as docker and psql client in the host

```sh
docker volume create pgdata
docker run -e POSTGRES_PASSWORD=`cat ~/security/devel/postgres` --restart=unless-stopped --name postgres -v pgdata:/var/lib/postgresql/data -d -p 5432:5432/tcp postgres:latest
apt install postgresql-client-16
```  

- this will allow you to connect to localhost postgres db as postgres user ( test with `psql -h localhost -U postgres` if connects )

- create postgres `example_webapp_user` user with capability to createdb

```sh
echo "CREATE USER example_webapp_user WITH PASSWORD '$(cat ~/security/devel/ExampleWebApp/postgres-user)' CREATEDB" | psql -h localhost -U postgres
```

### vscode debug

```sh
git clone https://github.com/devel0/example-webapp-with-auth.git
cd example-webapp-with-auth
code .
```

- choose `.NET Core Launch (web)` from run and debug then hit F5 ( this will start asp net web server on `https://webapp-test.searchathing.com/swagger/index.html` )

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

### backend

#### run tests

- configure unit test db settings

```sh
cd WebApiServer

TEST_DB_CONN_STRING="Host=localhost; Database=REPL_TEST_DBNAME; Username=REPL_TEST_DBUSER; Password=REPL_TEST_DBPASS"

dotnet user-secrets set "ConnectionStrings:UnitTest" "$DB_CONN_STRING"

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

#### add more migrations

```sh
./migr.sh add MIGRNAME
```

#### db dia gen

database diagram can be generated through [gen-db-dia.sh](doc/gen-db-dia.sh) script that uses schemacrawler ( [more][1] )

![](./doc/db/db.png)

### frontend

#### openapi usage

- start the backend

```sh
cd example-webapp-with-auth/WebApiServer
dotnet run
```

- generate Typescript/fetch frontend api

```sh
cd example-webapp-with-auth
./gen-api.sh
```

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
[2]: https://github.com/devel0/example-webapp-with-auth/blob/a04204f9014596509dcacd1af04a8579000d2fd6/WebApiServer/Types/UserPermission.cs#L89
[3]: https://github.com/devel0/example-webapp-with-auth/blob/a04204f9014596509dcacd1af04a8579000d2fd6/WebApiServer/DTOs/EditUserRequestDto.cs#L6
[4]: https://github.com/devel0/example-webapp-with-auth/blob/d3685ff088fde20254e385c5ebcc13cd3dda6f2e/clientapp/src/fetch.manager.ts#L62-L65
[5]: https://github.com/devel0/example-webapp-with-auth/blob/d3685ff088fde20254e385c5ebcc13cd3dda6f2e/clientapp/src/utils/utils.tsx#L17
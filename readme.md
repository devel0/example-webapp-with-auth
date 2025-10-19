# example-webapp-with-auth

## features

- Security
  - development using https acme wildcard cert ( alternatively use [selfsigned][1] )
  - authorization through JWT cookie `secure`, `httponly`, `samesite strict`
  - webapi with roles authorization
- Developer friendly
  - breakpoints works for c# backend and typescript angular/react frontend from the same vscode session
- Backend
  - c# asp net core
  - configuration
    - development `user-secrets`
    - appsettings ( autoreload on change )
    - production environment variables
- Frontend
  - [angular][7]
    - openapi typescript/angular api services generated from backend swagger endpoint
    - layout with responsive menu
    - authguard protected pages routes
    - login / logout ( TODO: reset lost password through email link )
    - ( TODO: user manager )
    - light/dark theme
    - snacks    
  - [react][8]
    - openapi typescript/axios api generated from backend swagger endpoint
    - layout with responsive menu
    - protected pages with react router dom
    - zustand global services
    - login / logout / reset lost password through email link
    - light/dark theme
    - snacks    

## quickstart

- clone repository

```sh
git clone https://github.com/devel0/example-webapp-with-auth.git
cd example-webapp-with-auth
```

- option 1 : angular frontend

```sh
git checkout frontend/angular
```

- option 2 : react frontend

```sh
git checkout frontend/react
```

- start development environment

```sh
code .
```

- restore exec permissions on helper scripts

```sh
source misc/restore-permissions.sh
```

- configure backend development variables

```sh
cd src/backend

dotnet user-secrets init

# change following as needed
SEED_ADMIN_EMAIL=some@domain.com
SEED_ADMIN_PASS="SEED_ADMIN_SECRET"
DB_PROVIDER="Postgres"
DB_CONN_STRING="Host=localhost; Database=DBNAME; Username=DBUSER; Password=DBPASS"

JWTKEY="$(openssl rand -hex 32)"

dotnet user-secrets set "AppConfig:Auth:Jwt:Key" "$JWTKEY"

dotnet user-secrets set "AppConfig:Database:Seed:Users:0:Username" "admin"
dotnet user-secrets set "AppConfig:Database:Seed:Users:0:Email" "$SEED_ADMIN_EMAIL"
dotnet user-secrets set "AppConfig:Database:Seed:Users:0:Password" "$SEED_ADMIN_PASS"
dotnet user-secrets set "AppConfig:Database:Seed:Users:0:Roles:0" "admin"

dotnet user-secrets set "AppConfig:Database:Connections:0:Name" "Development"
dotnet user-secrets set "AppConfig:Database:Connections:0:ConnectionString" "$DB_CONN_STRING"
dotnet user-secrets set "AppConfig:Database:Connections:0:Provider" "$DB_PROVIDER"

dotnet user-secrets set "AppConfig:Database:ConnectionName" "Development"

cd ../..
```

- start backend choosing C-S-P `Debug: Select and Start Debugging` then `BACKEND`

- start frontend by issueing

```sh
./run-frontend.sh
```

- start frontend debugger C-S-P `Debug: Select and Start Debugging` then `FRONTEND` ; this will open chrome to the named url and attaches the debugger

- to make things works with https acme cert you need a domain to set a CNAME record and certbot as described [here][2]

- then edit hosts `sudo /etc/hosts` like following in order to resolve name locally

```
127.0.0.1   dev-webapp-test.searchathing.com
```

- finally set nginx `sudo apt install nginx` by linking the conf

```sh
cd /etc/nginx/conf.d
ln -s ~/opensource/example-webapp-with-auth/deploy/nginx/dev/webapp-test.conf
service nginx reload
```

## development settings for lost password recovery

```sh
cd src/backend

# MAILSERVER_SECURITY can be "Tls" or "Ssl" or "Auto" or "None"

dotnet user-secrets set "EmailServer:SmtpServerName" "$MAILSERVER_HOSTNAME"
dotnet user-secrets set "EmailServer:SmtpServerPort" "$MAILSERVER_PORT"
dotnet user-secrets set "EmailServer:Security" "$MAILSERVER_SECURITY"
dotnet user-secrets set "EmailServer:Username" "$MAILSERVER_USER_EMAIL"
dotnet user-secrets set "EmailServer:Password" "$MAILSERVER_USER_PASSWORD"
```

## development setting for unit tests

when run backend unit tests with `dotnet test` the system will search for a configured db connection named "UnitTest"

:warning: use a separate db because it will be destroyed when test executes

```sh
cd src/backend

dotnet user-secrets set "AppConfig:Database:Connections:1:Name" "UnitTest"
dotnet user-secrets set "AppConfig:Database:Connections:1:ConnectionString" "$UNIT_TEST_DB_CONN_STRING"
dotnet user-secrets set "AppConfig:Database:Connections:1:Provider" "$DB_PROVIDER"
```

## deployment

a script to automate publish on production server is available

```sh
./publish -h sshname -sn test.searchathing.com -id mytest -sd searchathing.com
```

where
- `-h` is a configured `~/.ssh/config` entry to allow to enter with a public key to a server within root user
- `-sn` is the application server hostname
- `-id` is a unique application id to allow more app on the same server
- `-sd` is the domain name where app run

published files and folders

| folder                                      | description                                                                                   |
| ------------------------------------------- | --------------------------------------------------------------------------------------------- |
| `/root/security/mytest.env`                 | copy (if not already exists) of [webapp.env][6]                                               |
| `/root/deploy/mytest`                       | rsync of [deploy][3] folder                                                                   |
| `/srv/mytest/bin`                           | rsync of self contained production `src/backend/bin/Release/net9.0/linux-x64/publish/` folder |
| `/etc/system/systemd/mytest-webapp.service` | copy (if not already exists) of [webapp.service][4]                                           |
| `/etc/nginx/conf.d/mytest-webapp.conf`      | copy (if not already exists) of [webapp.conf][5]                                              |

prerequisites on server side:

```sh
apt install openssh-server rsync nginx

# to run the backend service as user
useradd -m user
```

editing configuration and logging production

- edit `/root/security/mytest.conf` setting variables accordingly to your production setup ( if not used EmailServer variables comment out with `#` )

to view backend service log

```sh
journalctl -u mytest-webapp -f
```

to restart backend service

```sh
service mytest-webapp restart
```

## how this project was built

- [backend](./how-this-project-was-built-backend.md)
- [frontend](./how-this-project-was-built-frontend.md)

[1]: https://github.com/devel0/knowledge/blob/cf1f477a4ccf898d7299dab4daa71ebcf049172f/doc/self-signed-cert.md
[2]: https://github.com/devel0/knowledge/blob/cf1f477a4ccf898d7299dab4daa71ebcf049172f/doc/letsencrypt-acme-dns.md
[3]: ./deploy
[4]: ./deploy/service/webapp.service
[5]: ./deploy/nginx/prod/webapp.conf
[6]: ./deploy/webapp.env
[7]: https://github.com/devel0/example-webapp-with-auth/tree/frontend/angular/src/frontend
[8]: https://github.com/devel0/example-webapp-with-auth/tree/frontend/react/src/frontend
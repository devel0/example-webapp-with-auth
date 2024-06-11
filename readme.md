# example-webapp-with-auth

- [features](#features)
- [quickstart](#quickstart)
- [prerequisites](#prerequisites)
  - [local db](#local-db)
- [test it](#test-it)
- [how this project was built](#how-this-project-was-built)

<hr/>

## features

- started from clone from [example web app](https://github.com/devel0/example-webapp?tab=readme-ov-file)
  - asp net core backend
  - react frontend + vite tooling
  - https self signed cert development
  - backend and frontend debugging in a solution
  - publish release with frontend webpacked available through server static files available directly from within backend
  - step by step how this project was built by git commit
- jwt auth secure, httponly, strict samesite
- react redux
- login public page and protected routes
- theme light/dark, snacks

## quickstart

## prerequisites

### local db

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

## test it

```sh
git clone https://github.com/devel0/example-webapp-with-auth.git
cd example-webapp-with-auth
code .
```

- choose `.NET Core Launch (web)` from run and debug then hit F5 ( this will start asp net web server on `https://webapp-test.searchathing.com/swagger/index.html` )

- start vite

```sh
cd clientapp
npm run dev
```

- choose `Launch Chrome` from run and debug then click the play icon ( this will start browser )

- try to login/current user/logout/current user button from frontend

- login page

![](./doc/login.png)

- master page

![](./doc/mainpage.png)

## how this project was built

- started from clone from [example web app](https://github.com/devel0/example-webapp/blob/e9328b16212f1d128518088bb8a2c4b620c2035e/readme.md#how-this-project-was-built)

- more frontend pkgs

```sh
cd example-webapp-with-auth
cd clientapp
npm install @mui/material @emotion/react @emotion/styled @mui/icons-material
npm i @reduxjs/toolkit react-redux react-router-dom axios linq-to-typescript usehooks-ts @fontsource/roboto
```

- create auth dbcontext

```sh
cd example-webapp-with-auth
dotnet new classlib -n AuthDbContext
dotnet sln add AuthDbContext
cd AuthDbContext
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 8.0.5
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.5
```

- create auth dbcontext migration

```sh
cd example-webapp-with-auth
dotnet new classlib -n AuthDbMigrationsPsql
dotnet sln add AuthDbMigrationsPsql
cd AuthDbMigrationsPsql
dotnet add package Microsoft.EntityFrameworkCore.Relational --version 8.0.5
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.4
dotnet add reference ../AuthDbContext
```

- add db pkgs to webapi sever

```sh
cd example-webapp-with-auth
cd WebApiServer
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.5
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.5
dotnet add reference ../AuthDbContext
dotnet add reference ../AuthDbMigrationsPsql

openssl rand -hex 32 > ~/security/devel/ExampleWebApp/jwt.key

dotnet new tool-manifest
dotnet tool install dotnet-ef

dotnet user-secrets init
dotnet user-secrets set "JwtSettings:Key" "$(cat ~/security/devel/ExampleWebApp/jwt.key)"

SEED_ADMIN_EMAIL=admin@admin.com
SEED_ADMIN_PASS=$(cat ~/security/devel/ExampleWebApp/admin)

DB_PROVIDER="Postgres"
DB_CONN_STRING="Host=localhost; Database=ExampleWebApp; Username=example_webapp_user; Password=$(cat ~/security/devel/ExampleWebApp/postgres-user)"

dotnet user-secrets init
dotnet user-secrets set "SeedUsers:Admin:Email" "$SEED_ADMIN_EMAIL"
dotnet user-secrets set "SeedUsers:Admin:Password" "$SEED_ADMIN_PASS"
dotnet user-secrets set "Provider" "$DB_PROVIDER"
dotnet user-secrets set "DbConnString" "$DB_CONN_STRING"

dotnet add package Microsoft.IdentityModel.Tokens --version 7.5.2
dotnet add package System.IdentityModel.Tokens.Jwt --version 7.5.2
```

- Add files...

- add initial migration

```sh
cd example-webapp-with-auth
cd WebApiServer
dotnet ef migrations add init --project ../AuthDbMigrationsPsql -- --provider Postgres
```

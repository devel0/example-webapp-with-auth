# backend

[source code](../src/backend)

## developer notes

### common task

- extend db
  - [define table][6]
  - [alloc table dbset][7]
  - create migration `./migr.sh add SOME` with [migr.sh](../migr.sh) script ( migr autoapply on run )

- query db
  - 

- create a service
  - [abstraction][2]
  - [implementation][3]
  - [registration][4]
  - [injection in another service][5]    

- extend [appsettings typed config][1]

## how this project was built

```sh
mkdir example-webapp-with-auth
cd example-webapp-with-auth
git init
dotnet new gitignore
mkdir src

cd src

dotnet new webapi -n webapi
mv webapi backend
cd backend
# see the list of packages from webapi.csproj PackageReference
dotnet add package PACKAGE
dotnet new tool-manifest
dotnet tool install dotnet-ef
dotnet user-secrets init
# add initial migration
dotnet ef migrations add init
cd ..

# add integration tests
dotnet new xunit -n test
cd test
dotnet add package Microsoft.AspNetCore.Mvc.Testing
cd ..

cd ..

dotnet new sln
dotnet sln add src/webapi src/test
dotnet build
```

[1]: https://github.com/devel0/example-webapp-with-auth/blob/e0dfec4e37e72d8e8dbc555efebd3f5d7057be24/src/backend/Services/Abstractions/Config/AppConfig.cs#L6

[2]: https://github.com/devel0/example-webapp-with-auth/blob/e2c8c9045109d994604f569356250ce2d0c5f0c5/src/backend/Services/Abstractions/IUtilService.cs#L3

[3]: https://github.com/devel0/example-webapp-with-auth/blob/e0dfec4e37e72d8e8dbc555efebd3f5d7057be24/src/backend/Services/Implementations/UtilService.cs#L3

[4]: https://github.com/devel0/example-webapp-with-auth/blob/e1166d8cdba625f46c8c234454c7294b0bf55b40/src/backend/Program.cs#L35

[5]: https://github.com/devel0/example-webapp-with-auth/blob/e2c8c9045109d994604f569356250ce2d0c5f0c5/src/backend/Services/Implementations/Auth/AuthService.cs#L17

[6]: https://github.com/devel0/example-webapp-with-auth/blob/e0dfec4e37e72d8e8dbc555efebd3f5d7057be24/src/backend/Services/Abstractions/Auth/Data/UserRefreshToken.cs#L5

[7]: https://github.com/devel0/example-webapp-with-auth/blob/e0dfec4e37e72d8e8dbc555efebd3f5d7057be24/src/backend/Db/AppDbContext.cs#L16
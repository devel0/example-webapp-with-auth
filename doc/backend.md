# backend

[source code](../src/backend)

## developer notes

### common task

- extend db
  - [define table][6]
  - [alloc table dbset][7]
  - create migration `./migr.sh add SOME` with [migr.sh](../migr.sh) script ( migr autoapply on run )

- query db
  - [inject db context][8]
  - [consume db query][9]

- create a service
  - [abstraction][2]
  - [implementation][3]
  - [registration][4]
  - [injection in another service][5]    

- create [extension methods][12]

- create [toolkit methods][13] setting their global [usings][14]

- logger
  - [inject][15]
  - [usage][16]

- [create api controller][10]

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

[1]: https://github.com/devel0/example-webapp-with-auth/blob/b4ba4c5556e4b3739525b33600b2d6721dad6ecb/src/backend/Services/Config/AppConfig.cs#L5-L6

[2]: https://github.com/devel0/example-webapp-with-auth/blob/b4ba4c5556e4b3739525b33600b2d6721dad6ecb/src/backend/Services/IUtilService.cs#L2-L3

[3]: https://github.com/devel0/example-webapp-with-auth/blob/b4ba4c5556e4b3739525b33600b2d6721dad6ecb/src/backend/Services/UtilService.cs#L2-L3

[4]: https://github.com/devel0/example-webapp-with-auth/blob/e1166d8cdba625f46c8c234454c7294b0bf55b40/src/backend/Program.cs#L35

[5]: https://github.com/devel0/example-webapp-with-auth/blob/b4ba4c5556e4b3739525b33600b2d6721dad6ecb/src/backend/Services/Auth/AuthService.cs#L16-L17

[6]: https://github.com/devel0/example-webapp-with-auth/blob/b4ba4c5556e4b3739525b33600b2d6721dad6ecb/src/backend/Services/Auth/Data/UserRefreshToken.cs#L4-L5

[7]: https://github.com/devel0/example-webapp-with-auth/blob/e0dfec4e37e72d8e8dbc555efebd3f5d7057be24/src/backend/Db/AppDbContext.cs#L16

[8]: https://github.com/devel0/example-webapp-with-auth/blob/b4ba4c5556e4b3739525b33600b2d6721dad6ecb/src/backend/Services/Fake/FakeService.cs#L14-L15

[9]: https://github.com/devel0/example-webapp-with-auth/blob/b4ba4c5556e4b3739525b33600b2d6721dad6ecb/src/backend/Services/Fake/FakeService.cs#L34

[10]: https://github.com/devel0/example-webapp-with-auth/blob/e6c698a4f4b5636e5d925952047e57dc66bb3022/src/backend/Controllers/FakeDataController.cs#L5-L6

[11]: https://github.com/devel0/example-webapp-with-auth/blob/3489ad0f978d257654d47feadeb0b203b6584aef/src/backend/Extensions/Utils.cs#L3-L12

[12]: https://github.com/devel0/example-webapp-with-auth/blob/3489ad0f978d257654d47feadeb0b203b6584aef/src/backend/Extensions/Utils.cs#L3-L12

[13]: https://github.com/devel0/example-webapp-with-auth/blob/3489ad0f978d257654d47feadeb0b203b6584aef/src/backend/Extensions/Utils.cs#L58-L62

[14]: https://github.com/devel0/example-webapp-with-auth/blob/3489ad0f978d257654d47feadeb0b203b6584aef/src/backend/Usings.cs#L57

[15]: https://github.com/devel0/example-webapp-with-auth/blob/e2c8c9045109d994604f569356250ce2d0c5f0c5/src/backend/Controllers/MainController.cs#L16

[16]: https://github.com/devel0/example-webapp-with-auth/blob/e2c8c9045109d994604f569356250ce2d0c5f0c5/src/backend/Controllers/MainController.cs#L30
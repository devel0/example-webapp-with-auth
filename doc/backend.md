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

[1]: https://github.com/devel0/example-webapp-with-auth/blob/f3c195e156a53be3dc7ef35fbd03f1b99f6a32f8/src/backend/Types/Config/AppConfig.cs#L11-L12

[2]: https://github.com/devel0/example-webapp-with-auth/blob/f3c195e156a53be3dc7ef35fbd03f1b99f6a32f8/src/backend/Services/IUtilService.cs#L3

[3]: https://github.com/devel0/example-webapp-with-auth/blob/b4ba4c5556e4b3739525b33600b2d6721dad6ecb/src/backend/Services/UtilService.cs#L3

[4]: https://github.com/devel0/example-webapp-with-auth/blob/0f6f274ced87be6df02de149ea81f11bba7c44e0/src/backend/Program.cs#L37

[5]: https://github.com/devel0/example-webapp-with-auth/blob/b4ba4c5556e4b3739525b33600b2d6721dad6ecb/src/backend/Services/Auth/AuthService.cs#L16

[6]: https://github.com/devel0/example-webapp-with-auth/blob/f3c195e156a53be3dc7ef35fbd03f1b99f6a32f8/src/backend/Services/Auth/Data/UserRefreshToken.cs#L5

[7]: https://github.com/devel0/example-webapp-with-auth/blob/f3c195e156a53be3dc7ef35fbd03f1b99f6a32f8/src/backend/Services/Db/AppDbContext.cs#L16

[8]: https://github.com/devel0/example-webapp-with-auth/blob/b4ba4c5556e4b3739525b33600b2d6721dad6ecb/src/backend/Services/Fake/FakeService.cs#L17

[9]: https://github.com/devel0/example-webapp-with-auth/blob/b4ba4c5556e4b3739525b33600b2d6721dad6ecb/src/backend/Services/Fake/FakeService.cs#L34

[10]: https://github.com/devel0/example-webapp-with-auth/blob/e6c698a4f4b5636e5d925952047e57dc66bb3022/src/backend/Controllers/FakeDataController.cs#L5-L6

[12]: https://github.com/devel0/example-webapp-with-auth/blob/3489ad0f978d257654d47feadeb0b203b6584aef/src/backend/Extensions/Utils.cs#L6-L12

[13]: https://github.com/devel0/example-webapp-with-auth/blob/3489ad0f978d257654d47feadeb0b203b6584aef/src/backend/Extensions/Utils.cs#L61-L62

[14]: https://github.com/devel0/example-webapp-with-auth/blob/f3c195e156a53be3dc7ef35fbd03f1b99f6a32f8/src/backend/Usings.cs#L62

[15]: https://github.com/devel0/example-webapp-with-auth/blob/0f6f274ced87be6df02de149ea81f11bba7c44e0/src/backend/Controllers/MainController.cs#L16

[16]: https://github.com/devel0/example-webapp-with-auth/blob/0f6f274ced87be6df02de149ea81f11bba7c44e0/src/backend/Controllers/MainController.cs#L30
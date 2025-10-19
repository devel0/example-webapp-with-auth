# how this project was built ( backend )

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

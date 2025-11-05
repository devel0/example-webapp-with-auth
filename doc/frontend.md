# frontend

[source code](../src/frontend)

## developer notes

### common task

from `src/frontend`:

- create component

```sh
ng g c --skip-tests components/some
```

- create service

```sh
ng g s --skip-tests services/some-service
```

- [inject a service][1]

from the component (.ts file) constructor declare a variable private or public depending the need to use directly from the `html` in readonly mode to ensure service reference can't changed.

note: using the `public` or `private` modifier the variable will available outside the constructor also.

- [add route to a protected page][2]

note: `canActivate` with `AuthGuard` ensure the route activation only if authorization valid

- [extend application menu][5]

- [inject api services][9]

- [global service providers][3]

- [intercept network errors][4]

- [show snackbar][7] ( [inject service][6] )

- [environment dev and prod variables][9] and definition are in [environments](../src/frontend/src/environments)

## how this project was built ( frontend )

```sh
pnpm i -g @angular/cli

cd src

ng new frontend
cd frontend
ng add @angular/material
ng generate environments
ng g c --skip-tests components/main-layout
ng g guard auth --skip-tests --functional=false
ng g interceptor interceptors/api --skip-tests --functional=false
ng g s --skip-tests services/global-service
```

[1]: https://github.com/devel0/example-webapp-with-auth/blob/167442ce4aa01cb366d74bba4353dbd31de64d0b/src/frontend/src/app/components/main-layout/main-layout.ts#L39

[2]: https://github.com/devel0/example-webapp-with-auth/blob/a56f1c25266f1bfab8acbc38381cf4ec11651c60/src/frontend/src/app/app.routes.ts#L10

[3]: https://github.com/devel0/example-webapp-with-auth/blob/585cfcaf7c06e8d2ea9ea936a4e34085f7fc9eb7/src/frontend/src/app/app.config.ts#L9

[4]: https://github.com/devel0/example-webapp-with-auth/blob/d70e737d35fc62c4ad10d8ff7143b7e37208ba16/src/frontend/src/app/interceptors/api-interceptor.ts#L37

[5]: https://github.com/devel0/example-webapp-with-auth/blob/585cfcaf7c06e8d2ea9ea936a4e34085f7fc9eb7/src/frontend/src/app/services/menu-service.ts#L34

[6]: https://github.com/devel0/example-webapp-with-auth/blob/d70e737d35fc62c4ad10d8ff7143b7e37208ba16/src/frontend/src/app/interceptors/api-interceptor.ts#L15

[7]: https://github.com/devel0/example-webapp-with-auth/blob/d70e737d35fc62c4ad10d8ff7143b7e37208ba16/src/frontend/src/app/interceptors/api-interceptor.ts#L48

[8]: https://github.com/devel0/example-webapp-with-auth/blob/a86108534117cf58d2a9cc66d07a694114444d38/src/frontend/src/app/components/about/about.ts#L21

[9]: https://github.com/devel0/example-webapp-with-auth/blob/a56f1c25266f1bfab8acbc38381cf4ec11651c60/src/frontend/src/app/pages/home/home.ts#L26
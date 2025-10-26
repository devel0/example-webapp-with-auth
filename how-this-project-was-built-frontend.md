# how this project was built ( frontend )

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

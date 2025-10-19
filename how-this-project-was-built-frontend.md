# how this project was built ( frontend )

```sh
pnpm i -g @angular/cli

cd src

ng new frontend
cd frontend
ng add @angular/material
ng generate environments
ng g c components/main-layout
ng g guard auth --functional=false
ng g interceptor interceptors/api --functional=false
ng g s services/global-service
```

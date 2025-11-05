# frontend

[source code](../src/frontend)

## developer notes

### common task

- to create a service, create a folder in [services](../src/frontend/src/services) with
  - [IData.ts][1] : data structure declaration
  - [IActions.ts][2] : methods declaration that can operate on service data
  - [Service.ts][3] : service global hook with initial data and method definition

- consume a service
  - use the service hook defined from `Service.ts` using the [selector][4] to avoid [unnecessary render][5]

- [extends menu][6]

- [intercept network][7]

- [connect generated axios api][8]

- [consume api][9] and [handle api error][10]

- [create paged datagrid](../src/frontend/src/pages/FakeDataPage.tsx)
  - [define columns][11]
  - [pagination rows loader][12]
  - [datagrid component][13]

- [scss theme global variables][14]

- use scss styles with intellisense
  - [import][15]
  - [set classname][16]
  - note: the filename must end with `.module.scss`

- use svg icon
  - copy file into [src/images](../src/frontend/src/images)
  - [import][17] ( note `?react` ending on the import )
  - [usage][18] ( use `fill` argument to change color )

## how this project was built ( frontend )

```sh
cd src

pnpm create vite@latest frontend -- --template react-ts
cd frontend
pnpm i --save-dev @vitejs/plugin-react
pnpm i @mui/material @emotion/react @emotion/styled @mui/icons-material
pnpm i zustand react-router-dom axios linq-to-typescript usehooks-ts @fontsource/roboto
cd ..
```

## vscode extensions

- `viijay-kr.react-ts-css` ( for scss styles intellisense )

[1]: https://github.com/devel0/example-webapp-with-auth/blob/3489ad0f978d257654d47feadeb0b203b6584aef/src/frontend/src/services/global/IData.ts#L1

[2]: https://github.com/devel0/example-webapp-with-auth/blob/3489ad0f978d257654d47feadeb0b203b6584aef/src/frontend/src/services/global/IActions.ts#L1

[3]: https://github.com/devel0/example-webapp-with-auth/blob/3489ad0f978d257654d47feadeb0b203b6584aef/src/frontend/src/services/global/Service.ts#L6

[4]: https://github.com/devel0/example-webapp-with-auth/blob/3489ad0f978d257654d47feadeb0b203b6584aef/src/frontend/src/components/Layout.tsx#L19

[5]: https://github.com/devel0/examples-react/tree/main/react-pitfalls

[6]: https://github.com/devel0/example-webapp-with-auth/blob/3489ad0f978d257654d47feadeb0b203b6584aef/src/frontend/src/components/Layout.tsx#L30

[7]: https://github.com/devel0/example-webapp-with-auth/blob/3489ad0f978d257654d47feadeb0b203b6584aef/src/frontend/src/axios.manager.ts#L19

[8]: https://github.com/devel0/example-webapp-with-auth/blob/3489ad0f978d257654d47feadeb0b203b6584aef/src/frontend/src/axios.manager.ts#L69

[9]: https://github.com/devel0/example-webapp-with-auth/blob/3489ad0f978d257654d47feadeb0b203b6584aef/src/frontend/src/pages/FakeDataPage.tsx#L126

[10]: https://github.com/devel0/example-webapp-with-auth/blob/3489ad0f978d257654d47feadeb0b203b6584aef/src/frontend/src/pages/FakeDataPage.tsx#L137

[11]: https://github.com/devel0/example-webapp-with-auth/blob/3489ad0f978d257654d47feadeb0b203b6584aef/src/frontend/src/pages/FakeDataPage.tsx#L34-L76

[12]: https://github.com/devel0/example-webapp-with-auth/blob/3489ad0f978d257654d47feadeb0b203b6584aef/src/frontend/src/pages/FakeDataPage.tsx#L78-L145

[13]: https://github.com/devel0/example-webapp-with-auth/blob/3489ad0f978d257654d47feadeb0b203b6584aef/src/frontend/src/pages/FakeDataPage.tsx#L179-L192

[14]: https://github.com/devel0/example-webapp-with-auth/blob/3489ad0f978d257654d47feadeb0b203b6584aef/src/frontend/src/index.scss#L13-L31

[15]: https://github.com/devel0/example-webapp-with-auth/blob/3489ad0f978d257654d47feadeb0b203b6584aef/src/frontend/src/components/DataGrid/DataGridColumnWidthHandler.tsx#L2

[16]: https://github.com/devel0/example-webapp-with-auth/blob/3489ad0f978d257654d47feadeb0b203b6584aef/src/frontend/src/components/DataGrid/DataGridColumnWidthHandler.tsx#L52

[17]: https://github.com/devel0/example-webapp-with-auth/blob/3489ad0f978d257654d47feadeb0b203b6584aef/src/frontend/src/pages/LoginPage.tsx#L11

[18]: https://github.com/devel0/example-webapp-with-auth/blob/3489ad0f978d257654d47feadeb0b203b6584aef/src/frontend/src/pages/LoginPage.tsx#L113
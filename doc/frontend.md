# frontend

[source code](../src/frontend)

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

/// <reference types="vite-plugin-svgr/client" />
import './index.css'
import { APP_TITLE } from './constants/general.ts'
import { blue, green, orange, red } from '@mui/material/colors'
import { createRoot } from 'react-dom/client'
import { MaterialDesignContent, SnackbarProvider } from 'notistack'
import { Provider } from 'react-redux'
import { store } from './redux/stores/store.ts'
import { StrictMode } from 'react'
import { styled } from '@mui/material'
import App from './App.tsx'

document.title = APP_TITLE

const StyledMaterialDesignContent = styled(MaterialDesignContent)(() => ({
  '&.notistack-MuiContent-error': {
    backgroundColor: red[900],
  },
  '&.notistack-MuiContent-success': {
    backgroundColor: green[900],
  },
  '&.notistack-MuiContent-warning': {
    backgroundColor: orange[900],
  },
  '&.notistack-MuiContent-info': {
    backgroundColor: blue[900],
  },
}));

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <Provider store={store}>
      <SnackbarProvider Components={{
        success: StyledMaterialDesignContent,
        error: StyledMaterialDesignContent,
        warning: StyledMaterialDesignContent,
        info: StyledMaterialDesignContent
      }}>
        <App />
      </SnackbarProvider>
    </Provider>
  </StrictMode>,
)
// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://cra.link/PWA
// serviceWorkerRegistration.unregister();

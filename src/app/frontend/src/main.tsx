/// <reference types="vite-plugin-svgr/client" />
import ReactDOM from 'react-dom/client'
import App from './App.tsx'
import './index.css'
import { Provider } from 'react-redux'
import { store } from './redux/stores/store.ts'
import { APP_TITLE } from './constants/general.ts'
import styled from '@emotion/styled'
import { MaterialDesignContent, SnackbarProvider } from 'notistack'
import { red, green, orange, blue } from '@mui/material/colors'

import * as serviceWorkerRegistration from './serviceWorkerRegistration';
import reportWebVitals from './reportWebVitals';

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

ReactDOM.createRoot(document.getElementById('root')!).render(
  // <React.StrictMode>

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

  // </React.StrictMode>,
)

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://cra.link/PWA
serviceWorkerRegistration.unregister();

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();

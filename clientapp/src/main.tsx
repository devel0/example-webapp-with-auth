/// <reference types="vite-plugin-svgr/client" />
import ReactDOM from 'react-dom/client'
import App from './App.tsx'
import './index.css'
import { Provider } from 'react-redux'
import { store } from './redux/stores/store'
import { APP_TITLE } from './constants/general.ts'

document.title = APP_TITLE

ReactDOM.createRoot(document.getElementById('root')!).render(
  // <React.StrictMode>

  <Provider store={store}>
    <App />
  </Provider>

  // </React.StrictMode>,
)

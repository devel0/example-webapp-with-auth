import { useEffect } from 'react'
import { ThemeProvider } from '@emotion/react'
import { useAppDispatch, useAppSelector } from './redux/hooks/hooks'
import './App.css'
import { ConfigAxios } from './axios.manager'
import { RouterProvider, createBrowserRouter } from 'react-router-dom'
import { APP_URL_Home, APP_URL_Login } from './constants/general'
import { LoginPage } from './components/LoginPage'
import ProtectedRoutes from './components/ProtectedRoutes'
import { RouteNotFound } from './components/RouteNotFound'
import { MainPage } from './components/MainPage'
import { GlobalState } from './redux/states/GlobalState'
import { evalThemeChanged } from './styles/Theme'
import React from 'react'
import { CssBaseline } from '@mui/material'

const router = createBrowserRouter(
  [
    //--------------------------------------------------------
    // PUBLIC ROUTES
    //--------------------------------------------------------

    // login
    {
      path: APP_URL_Login,
      element: <LoginPage />
    },

    //--------------------------------------------------------
    // PROTECTED ROUTES
    //--------------------------------------------------------
    {

      element: <ProtectedRoutes />,
      errorElement: <RouteNotFound />,
      children: [

        // home
        {
          path: APP_URL_Home,
          element: <MainPage />
        },

      ]
    },
  ],
  {
    // basename: 'https://localhost:5000'
  }
)


function App() {
  const global = useAppSelector<GlobalState>((state) => state.global)
  const dispatch = useAppDispatch()
  const theme = React.useMemo(() => evalThemeChanged(global), [global.theme])

  useEffect(() => {
    ConfigAxios(dispatch)
  }, [dispatch])

  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />

      <RouterProvider router={router} fallbackElement={<p>Loading...</p>} />
    </ThemeProvider>

  )
}

export default App

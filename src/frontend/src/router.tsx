import './App.css'
import { APP_URL_Home, APP_URL_Login, APP_URL_Users } from './constants/general'
import { createBrowserRouter } from 'react-router-dom'
import { LoginPage } from './pages/LoginPage'
import { MainPage } from './pages/MainPage'
import { RouteNotFound } from './components/RouteNotFound'
import { UsersPage } from './pages/UsersPage'
import ProtectedRoutes from './components/ProtectedRoutes'

export const router = createBrowserRouter(
  [
    //--------------------------------------------------------
    // PUBLIC ROUTES
    //--------------------------------------------------------

    // login
    {
      path: APP_URL_Login(),
      element: <LoginPage />
    },

    //--------------------------------------------------------
    // PROTECTED ROUTES
    //--------------------------------------------------------
    {

      element: <ProtectedRoutes />,

      errorElement: import.meta.env.PROD ? <RouteNotFound /> : undefined,

      children: [

        // home
        {
          path: APP_URL_Home,
          element: <MainPage />
        },

        // users
        {
          path: APP_URL_Users,
          element: <UsersPage />
        },

      ]
    },
  ],
  {
    // basename: 'https://localhost:5000'
  }
)

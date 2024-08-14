import './App.css'
import { createBrowserRouter } from 'react-router-dom'
import { APP_URL_Home, APP_URL_Login, APP_URL_Users } from './constants/general'
import { LoginPage } from './pages/LoginPage'
import ProtectedRoutes from './components/ProtectedRoutes'
import { RouteNotFound } from './components/RouteNotFound'
import { MainPage } from './pages/MainPage'
import { UsersPage } from './pages/UsersPage'

export const router = createBrowserRouter(
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

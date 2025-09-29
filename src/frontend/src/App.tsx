import './App.css'
import { CssBaseline, ThemeProvider } from '@mui/material'
import { router } from './router'
import { RouterProvider } from 'react-router-dom'
import { useEffect, useMemo } from 'react'
import { useThemeFollower } from './hooks/useThemeFollower'
import { useAxiosConfig } from './axios.manager'
import { useGlobalPersistService } from './services/globalPersistService'
import { useGlobalService } from './services/globalService'

function App() {
  const globalState = useGlobalService()
  const globalPersistState = useGlobalPersistService()

  const theme = useThemeFollower()
  useAxiosConfig()

  return (
    <div>
      <ThemeProvider theme={theme}>
        <CssBaseline />

        <RouterProvider router={router} />
      </ThemeProvider>
    </div>
  )
}

export default App

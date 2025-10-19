import './App.css'
import { CssBaseline, ThemeProvider } from '@mui/material'
import { router } from './router'
import { RouterProvider } from 'react-router-dom'
import { useThemeFollower } from './hooks/useThemeFollower'
import { useAxiosConfig } from './axios.manager'

function App() {  
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

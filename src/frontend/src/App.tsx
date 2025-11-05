import './App.scss'
import { CssBaseline, ThemeProvider } from '@mui/material'
import { router } from './router'
import { RouterProvider } from 'react-router-dom'
import { useThemeFollower } from './hooks/useThemeFollower'
import { useAxiosConfig } from './axios.manager'
import { LocalizationProvider } from '@mui/x-date-pickers';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';

function App() {
  const theme = useThemeFollower()
  useAxiosConfig()

  return (
    <div className='rootDiv'>
      <LocalizationProvider dateAdapter={AdapterDayjs}>
        <ThemeProvider theme={theme}>
          <CssBaseline />

          <RouterProvider router={router} />
        </ThemeProvider>
      </LocalizationProvider>
    </div>
  )
}

export default App

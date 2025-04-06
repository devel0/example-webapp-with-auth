import './App.css'
import { ConfigAxios } from './axios.manager'
import { CssBaseline, ThemeProvider } from '@mui/material'
import { evalThemeChanged } from './styles/Theme'
import { GlobalState } from './redux/states/GlobalState'
import { router } from './router'
import { RouterProvider } from 'react-router-dom'
import { useAppDispatch, useAppSelector } from './redux/hooks/hooks'
import { useEffect, useMemo } from 'react'

function App() {
  const global = useAppSelector<GlobalState>((state) => state.global)
  const dispatch = useAppDispatch()
  const theme = useMemo(() => evalThemeChanged(global), [global.theme])

  useEffect(() => {
    ConfigAxios()
  }, [])

  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />

      <RouterProvider router={router} />
    </ThemeProvider>
  )
}

export default App

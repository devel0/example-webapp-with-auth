import { useEffect } from 'react'
import { ThemeProvider } from '@emotion/react'
import { useAppDispatch, useAppSelector } from './redux/hooks/hooks'
import './App.css'
import { RouterProvider } from 'react-router-dom'
import { GlobalState } from './redux/states/GlobalState'
import { evalThemeChanged } from './styles/Theme'
import React from 'react'
import { CssBaseline } from '@mui/material'
import { router } from './router'

function App() {
  const global = useAppSelector<GlobalState>((state) => state.global)
  const dispatch = useAppDispatch()
  const theme = React.useMemo(() => evalThemeChanged(global), [global.theme])

  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />

      <RouterProvider router={router} fallbackElement={<p>Loading...</p>} />
    </ThemeProvider>

  )
}

export default App

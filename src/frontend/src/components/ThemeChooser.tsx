import { IconButton, Tooltip } from '@mui/material'
import NightlightIcon from '@mui/icons-material/Nightlight';
import LightModeIcon from '@mui/icons-material/LightMode';
import { useAppDispatch, useAppSelector } from '../redux/hooks/hooks'
import { GlobalState } from '../redux/states/GlobalState'
import { THEME_DARK, THEME_LIGHT } from '../constants/general';
import { setTheme } from '../redux/slices/globalSlice';

export default function ThemeChooser() {
    const global = useAppSelector<GlobalState>((state) => state.global)
    const dispatch = useAppDispatch()

    return (

        <Tooltip title="Theme Light / Dark">
            <IconButton sx={{ color: 'inherit' }} onClick={() => {
                dispatch(setTheme(global.theme === 'dark' ? THEME_LIGHT : THEME_DARK))
            }}>
                {global.theme === THEME_DARK ? <NightlightIcon /> : <LightModeIcon />}
            </IconButton>
        </Tooltip>
    )
}
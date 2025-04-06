import { GlobalState } from '../redux/states/GlobalState'
import { IconButton, Tooltip } from '@mui/material'
import { setTheme } from '../redux/slices/globalSlice';
import { THEME_DARK, THEME_LIGHT } from '../constants/general';
import { useAppDispatch, useAppSelector } from '../redux/hooks/hooks'
import LightModeIcon from '@mui/icons-material/LightMode';
import NightlightIcon from '@mui/icons-material/Nightlight';

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
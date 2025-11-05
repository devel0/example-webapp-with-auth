import { IconButton, Tooltip } from '@mui/material'
import { THEME_DARK, THEME_LIGHT } from '../constants/general';
import { useGlobalPersistService } from '../services/global-persist/Service';
import LightModeIcon from '@mui/icons-material/LightMode';
import NightlightIcon from '@mui/icons-material/Nightlight';

export default function ThemeChooser() {
    const themePalette = useGlobalPersistService(x => x.themePalette)
    const setThemePalette = useGlobalPersistService(x => x.setThemePalette)

    return (

        <Tooltip title="Theme Light / Dark">
            <IconButton sx={{ color: 'inherit' }} onClick={() => {
                setThemePalette(themePalette === 'dark' ? THEME_LIGHT : THEME_DARK)
            }}>
                {themePalette === THEME_DARK ? <NightlightIcon /> : <LightModeIcon />}
            </IconButton>
        </Tooltip>
    )
}
import { IconButton, Tooltip } from '@mui/material'
import { THEME_DARK, THEME_LIGHT } from '../constants/general';
import { useGlobalPersistService } from '../services/globalPersistService';
import { useGlobalService } from '../services/globalService';
import LightModeIcon from '@mui/icons-material/LightMode';
import NightlightIcon from '@mui/icons-material/Nightlight';

export default function ThemeChooser() {
    const globalState = useGlobalService()
    const globalPersistState = useGlobalPersistService()

    return (

        <Tooltip title="Theme Light / Dark">
            <IconButton sx={{ color: 'inherit' }} onClick={() => {
                globalPersistState.setThemePalette(globalPersistState.themePalette === 'dark' ? THEME_LIGHT : THEME_DARK)
            }}>
                {globalPersistState.themePalette === THEME_DARK ? <NightlightIcon /> : <LightModeIcon />}
            </IconButton>
        </Tooltip>
    )
}
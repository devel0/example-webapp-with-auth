import * as React from 'react';
import AppBar from '@mui/material/AppBar';
import Box from '@mui/material/Box';
import Toolbar from '@mui/material/Toolbar';
import IconButton from '@mui/material/IconButton';
import Typography from '@mui/material/Typography';
import Menu from '@mui/material/Menu';
import MenuIcon from '@mui/icons-material/Menu';
import Container from '@mui/material/Container';
import Avatar from '@mui/material/Avatar';
import Button from '@mui/material/Button';
import Tooltip from '@mui/material/Tooltip';
import MenuItem from '@mui/material/MenuItem';
import AdbIcon from '@mui/icons-material/Adb';
import AppLogo from '../images/app-icon.svg?react'
import { APP_LOGO_TEXT, APP_URL_Home, DEFAULT_SIZE_1_REM, DEFAULT_SIZE_0_5_REM } from '../constants/general';
import { useAppDispatch, useAppSelector } from '../redux/hooks/hooks';
import { GlobalState } from '../redux/states/GlobalState';
import { firstLetter } from '../utils/utils';
import ThemeChooser from './ThemeChooser';
import { useNavigate } from 'react-router-dom';
import { useTheme } from '@mui/material';

export interface AppBarItem {
    hidden?: boolean,
    label: string,
    icon?: JSX.Element,
    onClick?: () => void,
}

function ResponsiveAppBar(props: {
    pages: AppBarItem[],
    settings: AppBarItem[]
}) {
    const global = useAppSelector<GlobalState>((state) => state.global)
    const dispatch = useAppDispatch()
    const navigate = useNavigate()
    const theme = useTheme()
    const [anchorElNav, setAnchorElNav] = React.useState<null | HTMLElement>(null);
    const [anchorElUser, setAnchorElUser] = React.useState<null | HTMLElement>(null);
    const { pages, settings } = props

    const handleOpenNavMenu = (event: React.MouseEvent<HTMLElement>) => {
        setAnchorElNav(event.currentTarget);
    };
    const handleOpenUserMenu = (event: React.MouseEvent<HTMLElement>) => {
        setAnchorElUser(event.currentTarget);
    };

    const handleCloseNavMenu = () => {
        setAnchorElNav(null);
    };

    const handleCloseUserMenu = () => {
        setAnchorElUser(null);
    };

    return (
        <AppBar position="sticky" sx={{}}>
            <Box sx={{
                ml: DEFAULT_SIZE_1_REM,
                mr: DEFAULT_SIZE_1_REM
            }}>
                <Toolbar disableGutters>
                    {/* 
                    -----------------------------------------------------------------------
                     DESKTOP LOGO                     
                    -----------------------------------------------------------------------
                    */}
                    <div
                        onClick={() => navigate(APP_URL_Home)}
                        style={{
                            display: 'flex',
                            cursor: 'pointer'
                        }}>
                        <Box sx={{
                            display: { xs: 'none', md: 'flex' },
                            mr: 2
                        }} >
                            <AppLogo width='100%' height='50px' />
                        </Box>

                        {APP_LOGO_TEXT && <Typography
                            variant="h6"
                            noWrap
                            sx={{
                                mr: 2,
                                display: { xs: 'none', md: 'flex' },
                                alignSelf: 'center',
                                fontFamily: 'monospace',
                                fontWeight: 700,
                                letterSpacing: '.3rem',
                                color: 'inherit',
                                textDecoration: 'none',
                            }}
                        >
                            {APP_LOGO_TEXT}
                        </Typography>}
                    </div>

                    {/* 
                    -----------------------------------------------------------------------
                     DESKTOP MENU                     
                    -----------------------------------------------------------------------
                    */}
                    <Box sx={{
                        flexGrow: 1,
                        display: { xs: 'none', md: 'flex' }
                    }}>
                        {pages.filter(w => w.hidden !== true).map((page, pageIdx) => (
                            <Button
                                key={`dsk-page-${pageIdx}`}
                                onClick={() => {
                                    page.onClick?.()
                                    handleCloseNavMenu()
                                }}
                                sx={{ my: 2, color: 'white', display: 'block' }}
                            >
                                {page.label}
                            </Button>
                        ))}
                    </Box>

                    {/* 
                    -----------------------------------------------------------------------
                     MOBILE MENU                     
                    -----------------------------------------------------------------------
                    */}
                    <Box sx={{ flexGrow: 0, display: { xs: 'flex', md: 'none' } }}>
                        <IconButton
                            size="large"
                            aria-label="account of current user"
                            aria-controls="menu-appbar"
                            aria-haspopup="true"
                            onClick={handleOpenNavMenu}
                            color="inherit"
                        >
                            <MenuIcon />
                        </IconButton>
                        <Menu
                            id="menu-appbar"
                            anchorEl={anchorElNav}
                            anchorOrigin={{
                                vertical: 'bottom',
                                horizontal: 'left',
                            }}
                            keepMounted
                            transformOrigin={{
                                vertical: 'top',
                                horizontal: 'left',
                            }}
                            open={Boolean(anchorElNav)}
                            onClose={handleCloseNavMenu}
                            sx={{
                                display: { xs: 'block', md: 'none' },
                            }}
                        >
                            {pages.filter(w => w.hidden !== true).map((page, pageIdx) => (
                                <MenuItem
                                    key={`mob-page-${pageIdx}`}
                                    onClick={() => {
                                        page.onClick?.()
                                        handleCloseNavMenu()
                                    }}>
                                    <Typography textAlign="center">{page.label}</Typography>
                                </MenuItem>
                            ))}
                        </Menu>
                    </Box>

                    {/* 
                    -----------------------------------------------------------------------
                     MOBILE LOGO                     
                    -----------------------------------------------------------------------
                    */}
                    <div
                        onClick={() => navigate(APP_URL_Home)}
                        style={{
                            display: 'flex',
                            flexGrow: 1,
                            cursor: 'pointer'
                        }}>

                        <Box sx={{
                            display: 'flex',
                            width: '100%',
                            justifyContent: 'center'
                        }}>

                            <Box sx={{ display: 'flex' }}>

                                <Box mr={1} sx={{ display: { xs: 'flex', md: 'none' }, mr: 1 }}>
                                    <AppLogo width='100%' height='50px' />
                                </Box>

                                {APP_LOGO_TEXT && <Typography
                                    variant="h5"
                                    noWrap
                                    sx={{
                                        mr: 2,
                                        display: { xs: 'flex', md: 'none' },
                                        alignSelf: 'center',
                                        fontFamily: 'monospace',
                                        fontWeight: 700,
                                        letterSpacing: '.3rem',
                                        color: 'inherit',
                                        textDecoration: 'none',
                                    }}
                                >
                                    {APP_LOGO_TEXT}
                                </Typography>}

                            </Box>

                        </Box>
                    </div>

                    {/* 
                    -----------------------------------------------------------------------
                     USER MENU
                    -----------------------------------------------------------------------
                    */}
                    <Box sx={{ flexGrow: 0 }}>
                        <ThemeChooser />
                        <Tooltip title="Open settings">
                            <IconButton onClick={handleOpenUserMenu} sx={{ p: 0 }}>
                                <Avatar>
                                    {firstLetter(global.currentUser?.userName, true)}
                                </Avatar>
                            </IconButton>
                        </Tooltip>
                        <Menu
                            // sx={{ mt: '45px' }}
                            id="menu-appbar"
                            anchorEl={anchorElUser}
                            anchorOrigin={{
                                vertical: 'bottom',
                                horizontal: 'right',
                            }}
                            keepMounted
                            transformOrigin={{
                                vertical: 'top',
                                horizontal: 'right',
                            }}
                            open={Boolean(anchorElUser)}
                            onClose={handleCloseUserMenu}
                        >
                            {settings.map((setting, settingIdx) => (
                                <MenuItem
                                    key={`setting-${settingIdx}`}
                                    onClick={() => {
                                        setting.onClick?.()
                                        handleCloseUserMenu()
                                    }}>
                                    <Box sx={{ display: 'flex' }}>
                                        {setting.icon && <Box mr={DEFAULT_SIZE_0_5_REM}>{setting.icon}</Box>}
                                        <Typography textAlign="center">{setting.label}</Typography>
                                    </Box>
                                </MenuItem>
                            ))}
                        </Menu>
                    </Box>
                </Toolbar>
            </Box>
        </AppBar>
    );
}
export default ResponsiveAppBar;
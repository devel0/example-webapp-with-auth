import { APP_LOGO_TEXT, DEFAULT_SIZE_1_REM, DEFAULT_SIZE_0_5_REM, APP_URL_Home } from '../constants/general';
import { firstLetter, LinkButton } from '../utils/utils';
import { Link } from "react-router-dom"
import { useGlobalPersistService } from '../services/global-persist/Service';
import { useGlobalService } from '../services/global/Service';
import { useNavigate } from 'react-router-dom';
import { useTheme } from '@mui/material';
import * as React from 'react';
import AppBar from '@mui/material/AppBar';
import AppLogo from '../images/app-icon.svg?react'
import Avatar from '@mui/material/Avatar';
import Box from '@mui/material/Box';
import IconButton from '@mui/material/IconButton';
import Menu from '@mui/material/Menu';
import MenuIcon from '@mui/icons-material/Menu';
import MenuItem from '@mui/material/MenuItem';
import ThemeChooser from './ThemeChooser';
import Toolbar from '@mui/material/Toolbar';
import Tooltip from '@mui/material/Tooltip';
import Typography from '@mui/material/Typography';
import { useEffect } from 'react';

export interface AppBarItem {
    hidden?: boolean,
    label: string,
    selected?: boolean,
    icon?: React.ReactElement,
    url?: string,
    onClick?: () => void,
}

function ResponsiveAppBar(props: {
    pages: AppBarItem[],
    settings: AppBarItem[]
}) {
    const isMobile = useGlobalService(x => x.isMobile)
    const currentUser = useGlobalPersistService(x => x.currentUser)
    const appBarRef = React.useRef<HTMLDivElement>(null)

    const navigate = useNavigate()
    const theme = useTheme()
    const [anchorElNav, setAnchorElNav] = React.useState<null | HTMLElement>(null);
    const [anchorElUser, setAnchorElUser] = React.useState<null | HTMLElement>(null);
    const { pages, settings } = props
    const setAppBarHeight = useGlobalService(x => x.setAppBarHeight)

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

    useEffect(() => {
        if (appBarRef.current) {
            setAppBarHeight(appBarRef.current.clientHeight)
        }
    }, [appBarRef])

    return (
        <AppBar ref={appBarRef} position="sticky" sx={{}}>
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
                    {isMobile === false && <div
                        onClick={() => navigate(APP_URL_Home)}
                        style={{
                            display: 'flex',
                            cursor: 'pointer'
                        }}>

                        <Link
                            style={{
                                // display: { xs: 'none', md: 'flex' },
                                marginRight: DEFAULT_SIZE_0_5_REM
                            }}
                            to={APP_URL_Home}>
                            <AppLogo width='100%' height='50px' />
                        </Link>

                        {APP_LOGO_TEXT && <Typography
                            variant="h6"
                            noWrap
                            sx={{
                                mr: 2,
                                display: 'flex',
                                // display: { xs: 'none', md: 'flex' },
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
                    </div>}

                    {/* 
                    -----------------------------------------------------------------------
                     DESKTOP MENU                     
                    -----------------------------------------------------------------------
                    */}
                    {isMobile === false && <Box sx={{
                        flexGrow: 1,
                        display: 'flex'
                    }}>
                        {pages.filter(w => w.hidden !== true).map((page, pageIdx) => (

                            <LinkButton
                                LinkComponent={Link}
                                to={page.url ?? ''}
                                key={`dsk-page-${pageIdx}`}
                                onClick={() => {
                                    page.onClick?.()
                                    handleCloseNavMenu()
                                }}
                                sx={{ my: 2, color: 'white', display: 'block' }}
                            >
                                <Typography sx={{
                                    textDecoration: page.selected === true ? 'underline' : null
                                }}>
                                    {page.label}
                                </Typography>
                            </LinkButton>
                        ))}
                    </Box>}

                    {/* 
                    -----------------------------------------------------------------------
                     MOBILE MENU                     
                    -----------------------------------------------------------------------
                    */}
                    {isMobile && <Box sx={{ flexGrow: 0, display: 'flex', gap: DEFAULT_SIZE_0_5_REM, alignItems: 'center' }}>
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

                        <IconButton
                            size="large"
                            aria-label="account of current user"
                            aria-controls="menu-appbar"
                            aria-haspopup="true"
                            onClick={() => navigate(APP_URL_Home)}
                            color="inherit"
                        >
                            <Box sx={{
                                display: 'flex', gap: DEFAULT_SIZE_0_5_REM
                            }}>
                                <AppLogo width='100%' height='50px' />
                            </Box>
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
                                display: 'block'
                                // display: { xs: 'block', md: 'none' },
                            }}
                        >
                            {pages.filter(w => w.hidden !== true).map((page, pageIdx) => (
                                <MenuItem
                                    key={`mob-page-${pageIdx}`}
                                    onClick={() => {
                                        if (page.url)
                                            navigate(page.url ?? '')
                                        page.onClick?.()
                                        handleCloseNavMenu()
                                    }}>
                                    <Box sx={{ display: 'flex', gap: DEFAULT_SIZE_0_5_REM }}>
                                        {page.icon}
                                        <Typography textAlign="center">{page.label}</Typography>
                                    </Box>
                                </MenuItem>
                            ))}
                        </Menu>
                    </Box>}

                    {/* 
                    -----------------------------------------------------------------------
                     MOBILE LOGO                     
                    -----------------------------------------------------------------------
                    */}
                    {isMobile && <div
                        onClick={() => navigate(APP_URL_Home)}
                        style={{
                            display: 'flex',
                            flexGrow: 1,
                            cursor: 'pointer'
                        }}>

                        <Box sx={{
                            display: 'flex',
                            width: '100%',
                            justifyContent: 'flex-start'
                        }}>

                            <Box sx={{ display: 'flex' }}>


                            </Box>

                        </Box>
                    </div>}

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
                                    {firstLetter(currentUser?.userName, true)}
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
        </AppBar >
    );
}
export default ResponsiveAppBar;
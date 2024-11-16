//------------------------------------------------------------------
// API
//------------------------------------------------------------------

export const API_URL = () => `https://${import.meta.env.VITE_SERVERNAME}`

//------------------------------------------------------------------
// APP
//------------------------------------------------------------------

export const APP_TITLE = "WebAppTest";
export const APP_LOGO_TEXT: string | undefined = "WEBAPP";

import { PaletteMode } from "@mui/material";
import { green } from "@mui/material/colors"

export const APP_URL_BASE = "/app";

export const APP_URL_Home = `${APP_URL_BASE}`
export const APP_URL_Login = `${APP_URL_BASE}/login/:from?/:token?`
export const APP_URL_Users = `${APP_URL_BASE}/users`

export const LOCAL_STORAGE_CURRENT_USER_NFO = "currentUserNfo";
export const LOCAL_STORAGE_THEME = "theme";
export const LOCAL_STORAGE_REFRESH_TOKEN_EXPIRE = "refreshTokenExpire";

/** invoke RenewRefreshToken api before expiration */
export const RENEW_REFRESH_TOKEN_BEFORE_EXPIRE_SEC = import.meta.env.DEV ? 10 : 30;

//------------------------------------------------------------------
// GUI
//------------------------------------------------------------------

export const CSS_VAR_TOOLBAR_TEXT_COLOR = "--toolbar-text-color";
export const CSS_VAR_APP_BUTTON_FG = "--app-button-fg";
export const CSS_VAR_APP_BUTTON_BG = "--app-button-bg";
export const CSS_VAR_APP_CONTROL_BG = "--app-control-bg";
export const light_bg_paper = "#c0c0c0";

export const THEME_DARK = "dark";
export const THEME_LIGHT = "light";

export const THEME_INITIAL: PaletteMode = THEME_DARK;

/** XSMALL */
export const DEFAULT_SIZE_0_5_REM = '0.5rem'

/** SMALL */
export const DEFAULT_SIZE_1_REM = '1rem'

/** SMALL2 */
export const DEFAULT_SIZE_1_25_REM = '1.25rem'

/** LARGE */
export const DEFAULT_SIZE_2_REM = '2rem'

/** XLARGE */
export const DEFAULT_SIZE_4_REM = '4rem'

/** SEMIBOLD */
export const DEFAULT_FONTWEIGHT_500 = 500

/** BOLD */
export const DEFAULT_FONTWEIGHT_600 = 600

/** XBOLD */
export const DEFAULT_FONTWEIGHT_900 = 900

export const DEFAULT_COLOR_TIPS = green[400]
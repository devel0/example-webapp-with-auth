import { generateUrl } from "../utils/utils";
import { green } from "@mui/material/colors"
import { PaletteMode } from "@mui/material";

//------------------------------------------------------------------
// API
//------------------------------------------------------------------

export const API_URL = () => `https://${import.meta.env.VITE_SERVERNAME}`

export const WSS_URL = () => `wss://${import.meta.env.VITE_SERVERNAME}/api/Main/AliveWebSocket`

/** interval between each ping to state ws alive */
export const WS_PING_MS = 10000

/** expected interval max between ping and pong */
export const WS_PONG_EXPECTED_MS = 2000

//------------------------------------------------------------------
// APP
//------------------------------------------------------------------

export const APP_TITLE = "WebAppTest";
export const APP_LOGO_TEXT: string | undefined = "WEBAPP";

//------------------------------------------ urls

export const APP_URL_BASE =
    "/app";

export const APP_URL_Home =
    `${APP_URL_BASE}`

export const APP_URL_Login = (from?: string, token?: string) => generateUrl(
    `${APP_URL_BASE}/login/:from/:token`, { from, token })

export const APP_URL_FakeDatas =
    `${APP_URL_BASE}/fake-datas`

export const APP_URL_Users =
    `${APP_URL_BASE}/users`

//------------------------------------------ urls (end)

export const LOCAL_STORAGE_DATA = "data"

/** invoke RenewRefreshToken api before expiration */
export const RENEW_REFRESH_TOKEN_BEFORE_EXPIRE_SEC = import.meta.env.DEV ? 10 : 30;

//------------------------------------------------------------------
// GUI
//------------------------------------------------------------------

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
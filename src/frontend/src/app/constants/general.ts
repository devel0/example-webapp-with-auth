import { isDevMode } from "@angular/core";

export const APP_NAME = "WebAppTest"

/** key for local storage of current user nfo */
export const LOCAL_STORAGE_KEY_DATA = "data"

/** invoke RenewRefreshToken api before expiration */
export const RENEW_REFRESH_TOKEN_BEFORE_EXPIRE_SEC = isDevMode() ? 10 : 30;

export const ROUTEPATH_LOGIN = "login"
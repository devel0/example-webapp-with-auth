import { Injectable, isDevMode } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ConstantsService {

  readonly APP_NAME = "WebAppTest"

  /** key for local storage of current user nfo */
  readonly LOCAL_STORAGE_KEY_DATA = "data"

  /** invoke RenewRefreshToken api before expiration */
  readonly RENEW_REFRESH_TOKEN_BEFORE_EXPIRE_SEC = isDevMode() ? 10 : 30;  

}

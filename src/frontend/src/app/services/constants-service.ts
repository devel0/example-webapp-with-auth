import { Injectable, isDevMode } from '@angular/core';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class ConstantsService {

  readonly APP_NAME = "WebAppTest"

  /** key for local storage of current user nfo */
  readonly LOCAL_STORAGE_KEY_DATA = "data"

  readonly RESET_LOST_PASS_VERSION = 2

  /** invoke RenewRefreshToken api before expiration */
  readonly RENEW_REFRESH_TOKEN_BEFORE_EXPIRE_SEC = isDevMode() ? 10 : 30;  

  readonly EXAMPLE_WEBSOCKET_URL = `wss://${environment.serverName}/api/Main/ExampleWebSocket`

  /** ms between each ping alive test */
  readonly WEBSOCKET_PING_MS = 10000

  /** max ms after each ping to expect receive a pong */
  readonly WEBSOCKET_PONG_MS = 2000

}

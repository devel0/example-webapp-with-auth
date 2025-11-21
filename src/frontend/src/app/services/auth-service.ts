import { AuthApiService, CurrentUserResponseDto } from '../../api';
import { firstValueFrom } from 'rxjs';
import { HttpErrorResponse, HttpStatusCode } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { LocalStorageService } from './local-storage-service';
import { RENEW_REFRESH_TOKEN_BEFORE_EXPIRE_SEC } from '../constants/general';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserInitialized = false

  constructor(
    private readonly localStorageService: LocalStorageService,
    private readonly authApiService: AuthApiService,
    private readonly router: Router,    
  ) {

  }

  get currentUser() {
    return this.localStorageService.data.currentUser
  }

  /** remove currentUser from local storage and invoke logout so that the server will response with
   * cookie clear
   */
  async logout() {
    try {
      this.localStorageService.data.currentUser = null
      this.localStorageService.save()

      await firstValueFrom(this.authApiService.apiAuthLogoutGet())
    }
    catch (error) {
      if (error instanceof HttpErrorResponse) {
        console.error(error)
      }
    }

    this.router.navigate(["login"])
  }

  /** execute login so that the server will respond with httponly secure samesite cookie upon successful auth */
  async login(username: string, password: string) {
    try {
      const q = await firstValueFrom(this.authApiService.apiAuthLoginPost({
        usernameOrEmail: username,
        password: password
      }))

      if (q.status === 'OK' && q.userName && q.email && q.permissions && q.roles && q.refreshTokenExpiration) {
        this.localStorageService.data.currentUser = {
          userName: q.userName,
          email: q.email,
          permissions: [...q.permissions],
          roles: q.roles,
          refreshTokenExpiration: q.refreshTokenExpiration
        }
        this.currentUserInitialized = true

        this.setupTokenRefresher()

        this.localStorageService.save()

        return true
      }
    }
    catch (error) {
      if (error instanceof HttpErrorResponse) {      
        console.error(error)
      }
    }

    return false
  }

  /** start an autorefresh auth token using refreshtoken */
  setupTokenRefresher() {
    const act = () => {
      const q = this.localStorageService.data.currentUser?.refreshTokenExpiration
      if (q != null) {
        const refreshTokenExpire = new Date(q);

        const renewAt = new Date(refreshTokenExpire.getTime() - RENEW_REFRESH_TOKEN_BEFORE_EXPIRE_SEC * 1e3)
        const now = new Date()
        if (now.getTime() < renewAt.getTime()) {
          console.debug(`Renew refresh token at ${renewAt}`)
          setTimeout(async () => {
            try {
              const res = await firstValueFrom(this.authApiService.apiAuthRenewRefreshTokenPost())
              if (this.localStorageService.data.currentUser && res.refreshTokenNfo?.expiration) {
                this.localStorageService.data.currentUser.refreshTokenExpiration = res.refreshTokenNfo.expiration
                this.localStorageService.save()

                act()
              }
            }
            catch (error) {
              if (error instanceof HttpErrorResponse) {
                if (error.status === HttpStatusCode.Unauthorized) {                  
                  console.error(error)                  
                }
              }
            }
          }, renewAt.getTime() - now.getTime());
        }
      }
    }

    act()
  }

  private async tryCurrentUserApi() {
    let res: CurrentUserResponseDto | null = null

    try {
      res = await firstValueFrom(this.authApiService.apiAuthCurrentUserGet())
    }
    catch (error) {
      if (error instanceof HttpErrorResponse) {
        if (error.status === HttpStatusCode.Unauthorized) {
          // console.debug('unauth')
        }
      }
    }

    return res
  }

  /** states is current user as declared from local storage has been initialized
   * verifying auth versus server
   */
  async isAuth() {
    if (this.localStorageService.data.currentUser == null) return false

    if (!this.currentUserInitialized) {
      const q = await this.tryCurrentUserApi()

      this.currentUserInitialized = true

      this.setupTokenRefresher()

      if (q?.status === 'OK' && q?.userName && q.email && q.permissions && q.roles) {
        this.localStorageService.data.currentUser = {
          userName: q.userName,
          email: q.email,
          permissions: [...q.permissions],
          roles: [...q.roles],
          refreshTokenExpiration: this.localStorageService.data.currentUser.refreshTokenExpiration
        }
      }
      else
        this.localStorageService.data.currentUser = null

      this.localStorageService.save()

      return q != null
    }

    return true
  }

}

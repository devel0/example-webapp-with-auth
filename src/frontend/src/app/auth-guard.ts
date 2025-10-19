import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, GuardResult, Router, RouterStateSnapshot } from '@angular/router';
import { AuthService } from './services/auth-service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(
    private readonly authService: AuthService,
    private readonly router: Router
  ) {

  }

  async canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Promise<GuardResult> {

    const authValid = await this.authService.isAuth()

    if (!authValid) {
      this.router.navigate(['login'], { queryParams: { returnUrl: state.url } })
    }

    return authValid
  }

}

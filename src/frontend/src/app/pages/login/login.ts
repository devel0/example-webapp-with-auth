import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth-service';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { SnackService } from '../../services/snack-service';
import { StackedSnackbar } from "../../components/stacked-snackbar/stacked-snackbar";
import { BasicModule } from '../../modules/basic/basic-module';
import { ConstantsService } from '../../services/constants-service';
import { emptyString } from '../../utils/utils';
import { AuthApiService } from '../../../api';
import { firstValueFrom } from 'rxjs';
import { ROUTEPATH_LOGIN } from '../../constants/general';

@Component({
  selector: 'app-login',
  imports: [
    BasicModule, StackedSnackbar,
    MatCardModule,
  ],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class Login implements OnInit {

  constructor(
    private readonly constantsService: ConstantsService,
    private readonly authService: AuthService,
    private readonly authApiService: AuthApiService,
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    public readonly snackService: SnackService
  ) {

  }

  get appName() { return this.constantsService.APP_NAME }

  token: string | null = null

  ngOnInit() {
    const token: string | undefined = (this.route.snapshot.queryParams as any).token

    if (token != null)
      this.token = token
  }

  async doLostPassword(usernameOrEmail: string) {
    if (emptyString(usernameOrEmail)) {
      this.snackService.showInfo("Field required", "Specify email to receive reset password token.")
      return
    }

    const res = await firstValueFrom(
      this.authApiService.apiAuthResetLostPasswordGet(
        usernameOrEmail, undefined, undefined, this.constantsService.RESET_LOST_PASS_VERSION))

    this.snackService.showInfo("Reset password email sent", "Check your inbox for reset password link.")
  }

  async doLoginOrReset(username: string, password: string) {
    if (this.token != null) {
      const res = await firstValueFrom(this.authApiService.apiAuthResetLostPasswordGet(
        username, this.token, password, this.constantsService.RESET_LOST_PASS_VERSION))

      this.token = null

      this.router.navigate([ROUTEPATH_LOGIN])
    }
    else {
      if (await this.authService.login(username, password)) {
        const returnUrl = (this.route.snapshot.queryParams as any).returnUrl ?? '/home'
        this.router.navigate([returnUrl])
      }
      else
        this.snackService.showWarning('Invalid login')
    }
  }

}

import { Component } from '@angular/core';
import { AuthService } from '../../services/auth-service';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { SnackService } from '../../services/snack-service';
import { StackedSnackbar } from "../../components/stacked-snackbar/stacked-snackbar";
import { BasicModule } from '../../modules/basic/basic-module';
import { ConstantsService } from '../../services/constants-service';

@Component({
  selector: 'app-login',
  imports: [
    BasicModule, StackedSnackbar,
    MatCardModule,    
  ],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class Login {

  constructor(
    private readonly constantsService: ConstantsService,
    private readonly authService: AuthService,
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    public readonly snackService: SnackService
  ) {
  }

  get appName() { return this.constantsService.APP_NAME }

  async doLogin(username: string, password: string) {
    if (await this.authService.login(username, password)) {
      const returnUrl = (this.route.snapshot.queryParams as any).returnUrl ?? '/home'
      this.router.navigate([returnUrl])
    }
    else
      this.snackService.showWarning('Invalid login')

  }

}

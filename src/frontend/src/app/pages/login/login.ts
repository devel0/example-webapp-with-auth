import { Component } from '@angular/core';
import { AuthService } from '../../services/auth-service';
import { ActivatedRoute, Route, Router } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { FormsModule } from '@angular/forms';
import { APP_NAME } from '../../constants/general';
import { SnackService } from '../../services/snack-service';

@Component({
  selector: 'app-login',
  imports: [
    MatFormFieldModule, MatInputModule, MatCardModule, MatButtonModule, FormsModule
  ],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class Login {

  constructor(
    private readonly authService: AuthService,
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly snackService: SnackService
  ) {
  }

  get appName() { return APP_NAME }

  async doLogin(username: string, password: string) {
    if (await this.authService.login(username, password)) {
      const returnUrl = (this.route.snapshot.queryParams as any).returnUrl ?? '/home'
      this.router.navigate([returnUrl])
    }
    else
      this.snackService.showWarning('Invalid login')

  }

}

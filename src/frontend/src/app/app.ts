import { HTTP_INTERCEPTORS, provideHttpClient } from '@angular/common/http';
import { Component, signal } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { ApiInterceptor } from './interceptors/api-interceptor';
import { AuthApiService, provideApi } from '../api';
import { environment } from '../environments/environment';
import { AuthService } from './services/auth-service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.scss',
  providers: [

  ]
})
export class App {
  protected readonly title = signal('frontend-ng');

  constructor(
    private readonly authApiService: AuthApiService,
    private readonly authService: AuthService,
    private readonly router: Router
  ) {
  }

  async ngOnInit() {
    if (await this.authService.isAuth())
      this.router.navigate(["home"])
  }

}

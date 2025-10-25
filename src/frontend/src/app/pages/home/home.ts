import { Component } from '@angular/core';
import { firstValueFrom, Subscription } from 'rxjs';
import { GlobalService } from '../../services/global-service';
import { HttpErrorResponse } from '@angular/common/http';
import { MainApiService } from '../../../api';
import { MainLayout } from "../../components/main-layout/main-layout";
import { MatButtonModule } from '@angular/material/button';
import { SnackService } from '../../services/snack-service';
import { AuthService } from '../../services/auth-service';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-home',
  imports: [
    MainLayout,
    MatButtonModule
  ],
  templateUrl: './home.html',
  styleUrl: './home.scss',
})
export class Home {

  working: boolean = false

  constructor(
    private readonly mainApiService: MainApiService,
    private readonly snackService: SnackService,
    public readonly globalService: GlobalService,
    public readonly authService: AuthService
  ) {
  }

  ngOnInit() {
  }

  ngOnDestroy() {
  }

  async doLongRunningOp() {
    try {
      await firstValueFrom(this.mainApiService.apiMainLongRunningGet())

      this.snackService.showSuccess("completed")
    }
    catch (error) {
      if (error instanceof HttpErrorResponse) {
      }
    }
  }

  async doTestException() {
    try {
      await firstValueFrom(this.mainApiService.apiMainTestExceptionGet())

      this.snackService.showSuccess("completed")
    }
    catch (error) {
      if (error instanceof HttpErrorResponse) {
      }
    }
  }  

}

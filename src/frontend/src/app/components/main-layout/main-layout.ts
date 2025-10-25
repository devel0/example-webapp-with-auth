import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSlideToggleModule } from '@angular/material/slide-toggle'
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatMenuModule } from '@angular/material/menu';
import { StackedSnackbar } from "../stacked-snackbar/stacked-snackbar";
import { SnackService } from '../../services/snack-service';
import { ThemeService } from '../../services/theme-service';
import { AuthService } from '../../services/auth-service';
import { ApiInterceptor } from '../../interceptors/api-interceptor';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { GlobalService } from '../../services/global-service';
import { Subscription } from 'rxjs';
import { BreakpointObserver } from '@angular/cdk/layout';
import { MenuService } from '../../services/menu-service';

@Component({
  selector: 'app-main-layout',
  imports: [
    MatToolbarModule, MatButtonModule, MatIconModule, MatMenuModule,
    StackedSnackbar, MatProgressBarModule
  ],
  templateUrl: './main-layout.html',
  styleUrl: './main-layout.scss'
})
export class MainLayout {

  private breakpointObserverSub!: Subscription

  private networkErrorSub!: Subscription

  constructor(
    public readonly snackService: SnackService,
    public readonly themeService: ThemeService,
    public readonly authService: AuthService,
    public readonly globalService: GlobalService,
    private readonly breakpointObserver: BreakpointObserver,
    public readonly menuService: MenuService
  ) {

  }

  ngOnInit() {
    this.networkErrorSub = this.globalService.networkError$.subscribe(x => {
      if (x != null)
        this.snackService.showError("network err", `${x?.statusText}`)
    })

    this.breakpointObserverSub = this.breakpointObserver.observe(['(max-width: 768px)']).subscribe(result => {
      this.globalService.isMobile = result.matches
    });
  }

  ngOnDestroy() {
    this.networkErrorSub.unsubscribe()
    this.breakpointObserverSub.unsubscribe()
  }

  get currentTheme() {
    return this.themeService.currentTheme
  }

  get userName() {
    return this.authService.currentUser?.userName
  }

}

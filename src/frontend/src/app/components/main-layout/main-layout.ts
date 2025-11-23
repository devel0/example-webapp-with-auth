import { Component } from '@angular/core';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatMenuModule } from '@angular/material/menu';
import { StackedSnackbar } from "../stacked-snackbar/stacked-snackbar";
import { SnackService } from '../../services/snack-service';
import { ThemeService } from '../../services/theme-service';
import { AuthService } from '../../services/auth-service';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { GlobalService } from '../../services/global-service';
import { Subscription } from 'rxjs';
import { BreakpointObserver } from '@angular/cdk/layout';
import { MenuService } from '../../services/menu-service';
import { BasicModule } from '../../modules/basic/basic-module';
import { Router } from '@angular/router';
import { AliveWebsocketService } from '../../services/websocket/alive-websocket-service';

@Component({
  selector: 'app-main-layout',
  imports: [
    BasicModule, StackedSnackbar,
    MatToolbarModule, MatMenuModule, MatProgressBarModule
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
    public readonly menuService: MenuService,
    private readonly router: Router,
    public readonly aliveWebsocketService: AliveWebsocketService
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

  goHome() {
    this.router.navigate(['/'])
  }

  get currentTheme() {
    return this.themeService.currentTheme
  }

  get userName() {
    return this.authService.currentUser?.userName
  }

}

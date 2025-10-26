import { Routes } from '@angular/router';
import { App } from './app';
import { AuthGuard } from './auth-guard';
import { Login } from './pages/login/login';
import { Home } from './pages/home/home';
import { ROUTEPATH_LOGIN } from './constants/general';

export const routes: Routes = [
    { path: '', component: App, canActivate: [AuthGuard] },
    { path: 'home', component: Home, canActivate: [AuthGuard] },
    { path: ROUTEPATH_LOGIN, component: Login },
    { path: '**', redirectTo: '' }
];

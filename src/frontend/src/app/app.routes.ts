import { Routes } from '@angular/router';
import { App } from './app';
import { AuthGuard } from './auth-guard';
import { Login } from './pages/login/login';
import { Home } from './pages/home/home';
import { FakeData } from './pages/fake-data/fake-data';
import { ROUTEPATH_LOGIN } from './constants/general';
import { UsersManager } from './pages/users-manager/users-manager';

export const routes: Routes = [
    // { path: '', component: App, canActivate: [AuthGuard] },
    { path: 'home', component: Home, canActivate: [AuthGuard] },
    { path: 'fake-data', component: FakeData, canActivate: [AuthGuard] },
    { path: 'users-manager', component: UsersManager, canActivate: [AuthGuard] },
    { path: ROUTEPATH_LOGIN, component: Login },    
    { path: '**', redirectTo: '/home' }
];

import { Routes } from '@angular/router';
import { App } from './app';
import { AuthGuard } from './auth-guard';
import { Login } from './pages/login/login';
import { Home } from './pages/home/home';
import { Some } from './pages/some/some';

export const routes: Routes = [
    { path: '', component: App, canActivate: [AuthGuard] },    
    { path: 'home', component: Home, canActivate: [AuthGuard] },    
    { path: 'some', component: Some, canActivate: [AuthGuard] },    
    { path: 'login', component: Login },
    { path: '**', redirectTo: '' }
];

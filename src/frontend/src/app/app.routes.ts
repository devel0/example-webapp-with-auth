import { Routes } from '@angular/router';
import { App } from './app';
import { AuthGuard } from './auth-guard';
import { Login } from './pages/login/login';
import { Home } from './pages/home/home';

export const routes: Routes = [
    { path: '', component: App, canActivate: [AuthGuard] },    
    { path: 'home', component: Home, canActivate: [AuthGuard] },        
    { path: 'login', component: Login },
    { path: '**', redirectTo: '' }
];

export * from './auth.service';
import { AuthApiService } from './auth.service';
export * from './main.service';
import { MainApiService } from './main.service';
export const APIS = [AuthApiService, MainApiService];

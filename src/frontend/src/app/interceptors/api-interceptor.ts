import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpResponse, HttpErrorResponse } from '@angular/common/http';
import { finalize, Observable, tap } from 'rxjs';
import { GlobalService } from '../services/global-service';
import { SnackService } from '../services/snack-service';

@Injectable()
export class ApiInterceptor implements HttpInterceptor {

  constructor(
    private readonly globalService: GlobalService,
    private readonly snackService: SnackService
  ) {

  }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    let statusCode: number | null = null
    let gotError: HttpErrorResponse | null = null

    this.globalService.incGeneralNetwork()

    return next.handle(request).pipe(

      tap({
        next: (event) => {
          if (event instanceof HttpResponse) {
            statusCode = event.status;
          }
        },
        error: (error: any) => {
          if (error instanceof HttpErrorResponse) {
            statusCode = error.status;
            gotError = error
            this.snackService.showError(`Network error`, `${error.statusText}`)
          }
        }
      }),

      finalize(() => {
        if (gotError) this.globalService.gotNetworkError(gotError)

        this.globalService.decGeneralNetwork()
      }))
  }
}

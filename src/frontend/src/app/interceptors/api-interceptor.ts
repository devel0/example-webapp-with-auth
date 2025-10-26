import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpResponse, HttpErrorResponse, HttpStatusCode } from '@angular/common/http';
import { finalize, Observable, tap } from 'rxjs';
import { GlobalService } from '../services/global-service';
import { SnackService } from '../services/snack-service';
import { PlatformLocation } from '@angular/common';
import { ROUTEPATH_LOGIN } from '../constants/general';

@Injectable()
export class ApiInterceptor implements HttpInterceptor {

  constructor(
    private readonly globalService: GlobalService,
    private readonly snackService: SnackService,    
    private readonly platformLocation: PlatformLocation
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
            // avoid to intercept and show api error if in login page
            const baseHref = this.platformLocation.getBaseHrefFromDOM()            
            if (document.location.pathname != `${baseHref}${ROUTEPATH_LOGIN}` && error.status != HttpStatusCode.Unauthorized) {
              statusCode = error.status;
              if (error.status == HttpStatusCode.InternalServerError) {
                if (error.error?.title != null) {
                  const title = error.error.title
                  const message = error.error.detail

                  this.snackService.showError(title, message)
                }
                else
                  this.snackService.showError(error.statusText)
              }
              else {
                gotError = error
                this.snackService.showError(`Network error`, `${error.statusText}`)
              }
            }
          }
        }
      }),

      finalize(() => {
        if (gotError) this.globalService.gotNetworkError(gotError)

        this.globalService.decGeneralNetwork()
      }))
  }
}

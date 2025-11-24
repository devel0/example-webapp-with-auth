import { ActivatedRoute, Router } from '@angular/router';
import { finalize, Observable, tap } from 'rxjs';
import { GlobalService } from '../services/global-service';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpResponse, HttpErrorResponse, HttpStatusCode } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { PlatformLocation } from '@angular/common';
import { SnackService } from '../services/snack-service';
import { ConstantsService } from '../services/constants-service';
import { ROUTEPATH_LOGIN } from '../constants/general';

@Injectable()
export class ApiInterceptor implements HttpInterceptor {

  constructor(
    private readonly constantsService: ConstantsService,
    private readonly globalService: GlobalService,
    private readonly snackService: SnackService,
    private readonly platformLocation: PlatformLocation,
    private readonly router: Router,
    private readonly route: ActivatedRoute
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

            statusCode = error.status;
            if (statusCode === HttpStatusCode.InternalServerError || statusCode === HttpStatusCode.Forbidden) {
              if (error.error?.title != null) {
                const title = error.error.title
                const message = error.error.detail

                this.snackService.showError(title, message)
              }
              else
                this.snackService.showError(error.statusText)
            }
            else if (statusCode === HttpStatusCode.Unauthorized) {
              if (document.location.pathname != `${baseHref}${ROUTEPATH_LOGIN}`)
                this.router.navigate(['login'], { queryParams: { returnUrl: this.route.snapshot.url } })
            }
            else {
              gotError = error
              this.snackService.showError(`Network error`, `${error.statusText}`)
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

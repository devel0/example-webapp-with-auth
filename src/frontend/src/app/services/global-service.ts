import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class GlobalService {

  private _generalNetwork = 0
  get generalNetwork() { return this._generalNetwork }

  private _isMobile = false
  get isMobile() { return this._isMobile }
  set isMobile(x: boolean) { this._isMobile = x }

  private networkError = new BehaviorSubject<HttpErrorResponse | null>(null)
  networkError$ = this.networkError.asObservable()
  incGeneralNetwork() { this._generalNetwork++ }
  decGeneralNetwork() { this._generalNetwork-- }

  constructor() {
  }

  gotNetworkError(gotError: HttpErrorResponse) {
    this.networkError.next(gotError)
  }

}

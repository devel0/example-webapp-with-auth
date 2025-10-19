import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class GlobalService {

  private generalNetwork = new BehaviorSubject(0)
  generalNetwork$ = this.generalNetwork.asObservable()

  private isMobile = new BehaviorSubject(false)
  isMobile$ = this.isMobile.asObservable()

  private networkError = new BehaviorSubject<HttpErrorResponse | null>(null)
  networkError$ = this.networkError.asObservable()

  constructor() {
  }

  setIsMobile(value: boolean) {
    this.isMobile.next(value)
  }

  incGeneralNetwork() {
    this.generalNetwork.next(this.generalNetwork.value + 1)
  }

  decGeneralNetwork() {
    this.generalNetwork.next(this.generalNetwork.value - 1)
  }

  gotNetworkError(gotError: HttpErrorResponse) {
    this.networkError.next(gotError)
  }

}

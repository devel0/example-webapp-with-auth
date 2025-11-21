import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class WindowSizeService {
  private heightSource = new BehaviorSubject<number>(window.innerHeight)
  height$ = this.heightSource.asObservable()

  currentHeight: number = window.innerHeight

  constructor() {
    window.addEventListener('resize', () => {
      this.currentHeight = window.innerHeight
      this.heightSource.next(window.innerHeight);
    })
  }

}

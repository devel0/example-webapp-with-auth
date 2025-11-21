import { AfterViewInit, Component, ElementRef, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { TrackOffsetTop } from "../../directives/track-offset-top";
import { WindowSizeService } from '../../services/window-size-service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-full-height-div',
  imports: [TrackOffsetTop],
  templateUrl: './full-height-div.html',
  styleUrl: './full-height-div.scss',
})
export class FullHeightDiv implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('divMain') divMain!: ElementRef<HTMLDivElement>
  divMainSub!: Subscription

  refDivOffsetTop!: number

  constructor(
    private readonly windowSize: WindowSizeService
  ) {
    this.divMainSub = windowSize.height$.subscribe(wh => this.resizeDivMain(wh))
  }

  ngOnInit(): void {

  }

  ngAfterViewInit(): void {
    this.resizeDivMain(this.windowSize.currentHeight)
  }

  ngOnDestroy(): void {
    this.divMainSub.unsubscribe()
  }

  private resizeDivMain(windowHeight: number) {
    if (this.divMain?.nativeElement != null) {
      this.divMain.nativeElement.style['height'] = `${windowHeight - this.refDivOffsetTop}px`
    }
  }

  divRefOffsetTopChanged(offsetTop: number) {
    this.refDivOffsetTop = offsetTop
    this.resizeDivMain(this.windowSize.currentHeight)
  }

}

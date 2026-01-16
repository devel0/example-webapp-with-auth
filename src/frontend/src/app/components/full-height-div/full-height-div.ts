import { AfterViewInit, Component, ElementRef, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { WindowSizeService } from '../../services/window-size-service';
import { Subscription } from 'rxjs';
import { BasicModule } from "../../modules/basic/basic-module";
import { ElementMetrics } from '../../directives/track-element-metrics';

@Component({
  selector: 'app-full-height-div',
  imports: [BasicModule],
  templateUrl: './full-height-div.html',
  styleUrl: './full-height-div.scss',
})
export class FullHeightDiv implements OnInit, AfterViewInit, OnDestroy {  
  private subs: Subscription[] = []
  
  @ViewChild('divMain') divMain!: ElementRef<HTMLDivElement>

  refDivOffsetTop!: number  

  constructor(
    private readonly windowSize: WindowSizeService
  ) {
    this.subs.push(windowSize.height$.subscribe(wh => this.resizeDivMain(wh)))
  }

  ngOnInit(): void {

  }

  ngAfterViewInit(): void {
    this.resizeDivMain(this.windowSize.currentHeight)
  }

  ngOnDestroy(): void {
    this.subs.forEach(sub => sub.unsubscribe())
  }


  divRefMetricsChanged(e: ElementMetrics) {
    this.refDivOffsetTop = e.top
    this.resizeDivMain(this.windowSize.currentHeight)
  }

  private resizeDivMain(windowHeight: number) {
    if (this.divMain?.nativeElement != null) {
      this.divMain.nativeElement.style['height'] = `${windowHeight - this.refDivOffsetTop}px`
    }
  }

}

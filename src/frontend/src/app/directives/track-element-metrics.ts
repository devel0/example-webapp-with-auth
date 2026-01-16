import { Directive, ElementRef, EventEmitter, Input, NgZone, OnDestroy, OnInit, Output } from '@angular/core';

export interface ElementMetrics {
  width: number;
  height: number;
  /**
   * same as {@link width} plus {@link marginLeft} plus {@link marginRight}
   */
  offsetWidth: number;
  /**
   * same as {@link height} plus {@link marginTop} plus {@link marginBottom}
   */
  offsetHeight: number;
  top: number;
  left: number;
  marginTop: number;
  marginRight: number;
  marginBottom: number;
  marginLeft: number;
}

@Directive({
  selector: '[appTrackElementMetrics]'
})
export class TrackElementMetricsDirective implements OnInit, OnDestroy {
  @Output() metricsChange = new EventEmitter<ElementMetrics>();
  
  private resizeObserver?: ResizeObserver;
  private mutationObserver?: MutationObserver;

  constructor(
    private el: ElementRef,
    private ngZone: NgZone    
  ) {

  }

  ngOnInit(): void {
    // Run outside Angular to avoid unnecessary change detection cycles
    this.ngZone.runOutsideAngular(() => {
      this.resizeObserver = new ResizeObserver(() => this.emitMetrics());
      this.resizeObserver.observe(this.el.nativeElement);

      // MutationObserver to detect DOM changes that might affect position
      this.mutationObserver = new MutationObserver(() => this.emitMetrics());
      this.mutationObserver.observe(document.body, {
        attributes: true,
        childList: true,
        subtree: true
      });

      // Emit initial values
      this.emitMetrics();
    });
  }

  private emitMetrics(): void {
    const rect = this.el.nativeElement.getBoundingClientRect();
    const style = window.getComputedStyle(this.el.nativeElement);

    const marginTop = parseFloat(style.marginTop) || 0
    const marginRight = parseFloat(style.marginRight) || 0
    const marginBottom = parseFloat(style.marginBottom) || 0
    const marginLeft = parseFloat(style.marginLeft) || 0

    const metrics: ElementMetrics = {
      width: rect.width,
      height: rect.height,
      top: rect.top + window.scrollY,
      left: rect.left + window.scrollX,
      marginTop,
      marginRight,
      marginBottom,
      marginLeft,
      offsetWidth: rect.width + marginLeft + marginRight,
      offsetHeight: rect.height + marginTop + marginBottom
    };

    // Emit inside Angular zone so subscribers trigger change detection
    this.ngZone.run(() => this.metricsChange.emit(metrics));
  }

  ngOnDestroy(): void {
    this.resizeObserver?.disconnect();
    this.mutationObserver?.disconnect();
  }
}
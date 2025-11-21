import { Directive, ElementRef, EventEmitter, OnInit, OnDestroy, Output } from '@angular/core';

/** track html element offsetTop */
@Directive({
  selector: '[appTrackOffsetTop]'
})
export class TrackOffsetTop implements OnInit, OnDestroy {
  @Output() offsetTopChange = new EventEmitter<number>()

  private offsetTop: number | null = null

  private divRefResizeObserver!: ResizeObserver
  private divRefMutationObserver!: MutationObserver

  constructor(
    private el: ElementRef<HTMLElement>
  ) {
    this.divRefResizeObserver = new ResizeObserver(() => this.emitOffsetTop())
    this.divRefResizeObserver.observe(document.body, { box: 'border-box' })

    this.divRefMutationObserver = new MutationObserver(() => this.emitOffsetTop())
    this.divRefMutationObserver.observe(document.body, { childList: true, subtree: true })
  }

  ngOnInit() {
    this.emitOffsetTop();
  }

  ngOnDestroy() {
    this.divRefResizeObserver.disconnect()
    this.divRefMutationObserver.disconnect()
  }

  private emitOffsetTop() {
    try {
      const currentOffset = this.el.nativeElement.offsetTop;
      if (this.offsetTop !== currentOffset) {
        this.offsetTop = currentOffset
        this.offsetTopChange.emit(currentOffset)
      }
    } catch (err) {
      console.error(`error reading div offsetTop: ${err}`)
    }
  }

}

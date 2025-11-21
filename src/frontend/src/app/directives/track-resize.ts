import { Directive, ElementRef, EventEmitter, Output } from '@angular/core';

export interface SizeNfo { width: number, height: number }

/** track html element resize */
@Directive({
  selector: '[appTrackResize]'
})
export class TrackResize {
  @Output() sizeChanged = new EventEmitter<SizeNfo>()

  private resizeObserver!: ResizeObserver
  private size: SizeNfo | null = null
  private element!: ElementRef<HTMLElement>

  constructor(    
    element: ElementRef<HTMLElement>
  ) {
    this.element = element
    this.resizeObserver = new ResizeObserver(entries => {
      this.emitSizeChanged()
    })
    this.resizeObserver.observe(element.nativeElement);
  }

  private emitSizeChanged() {
    try {
      const currentSize: SizeNfo = {
        width: this.element.nativeElement.clientWidth,
        height: this.element.nativeElement.clientHeight
      }

      if (this.size == null || currentSize.width !== this.size.width || currentSize.height != this.size.height) {
        this.size = currentSize
        this.sizeChanged.emit(this.size)
      }
    } catch (err) {
      console.error(`error reading element size ${err}`)
    }
  }

}

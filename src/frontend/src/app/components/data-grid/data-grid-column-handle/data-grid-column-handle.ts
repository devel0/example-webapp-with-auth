import { AfterViewInit, Component, ElementRef, EventEmitter, OnDestroy, OnInit, Output, Renderer2, ViewChild } from '@angular/core';

@Component({
  selector: 'app-data-grid-column-handle',
  imports: [],
  templateUrl: './data-grid-column-handle.html',
  styleUrl: './data-grid-column-handle.scss',
})
export class DataGridColumnHandle implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('divRef') divRef!: ElementRef<HTMLDivElement>

  /** column width ( delta ) change */
  @Output() columnWidthChange = new EventEmitter<number>

  startPosX: number | null = null
  posX: number | null = null

  constructor(
    private renderer: Renderer2
  ) {
  }

  ngOnInit() {

  }

  ngAfterViewInit() {
    const div = this.divRef.nativeElement


  }

  ngOnDestroy() {

  }

  onPointerDown(e: PointerEvent) {
    const div = e.currentTarget as HTMLDivElement
    div.setPointerCapture(e.pointerId)

    const right = parseFloat(
      div.computedStyleMap().get('right')?.toString().replace(/px$/, '') ?? '0')

    this.startPosX = 5 + right + e.clientX
  }

  onPointerMove(e: PointerEvent) {
    this.posX = e.clientX
    this.reflectPosX()
  }

  onPointerUp(e: PointerEvent) {
    const div = e.currentTarget as HTMLDivElement
    div.releasePointerCapture(e.pointerId)
    this.startPosX = null

    if (this.divRef.nativeElement != null) {
      const right = parseFloat(
        div.computedStyleMap().get('right')?.toString().replace(/px$/, '') ?? '0')

      this.columnWidthChange.emit(-right - 5)

      this.divRef.nativeElement.style['right'] = `-5px`
    }
  }

  reflectPosX() {
    if (this.divRef.nativeElement != null && this.startPosX != null && this.posX != null) {
      const deltaX = this.startPosX - this.posX

      this.divRef.nativeElement.style['right'] = `${-5 + deltaX}px`
    }

  }

}

import { Directive, ElementRef, Renderer2, OnInit } from '@angular/core';

@Directive({
  selector: '[disableAutocomplete]'
})
export class DisableAutocompleteDirective implements OnInit {
  constructor(private el: ElementRef, private renderer: Renderer2) {}

  ngOnInit() {
    // Set attributes to prevent autocomplete/autofill
    this.renderer.setAttribute(this.el.nativeElement, 'autocomplete', 'off');
    this.renderer.setAttribute(this.el.nativeElement, 'autocorrect', 'off');
    this.renderer.setAttribute(this.el.nativeElement, 'autocapitalize', 'off');
    this.renderer.setAttribute(this.el.nativeElement, 'spellcheck', 'false');
  }
}
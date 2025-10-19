import { DOCUMENT, Inject, Injectable, Renderer2, RendererFactory2 } from '@angular/core';
import { LocalStorageService } from './local-storage-service';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private renderer: Renderer2;

  currentTheme: 'light' | 'dark'

  constructor(
    @Inject(DOCUMENT) private document: Document,
    private rendererFactory: RendererFactory2,
    private localStorageService: LocalStorageService
  ) {
    this.renderer = this.rendererFactory.createRenderer(null, null);

    this.currentTheme = this.localStorageService.data.colorScheme
    this.setTheme(this.currentTheme)
  }

  setTheme(theme: 'light' | 'dark') {
    this.renderer.removeClass(this.document.body, 'light-theme');
    this.renderer.removeClass(this.document.body, 'dark-theme');

    this.renderer.addClass(this.document.body, `${theme}-theme`);

    this.currentTheme = theme
    this.localStorageService.data.colorScheme = this.currentTheme
    this.localStorageService.save()
  }

  toggleTheme() {
    if (this.currentTheme === 'dark')
      this.setTheme('light')
    else
      this.setTheme('dark')
  }

}

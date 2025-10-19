import { Injectable } from '@angular/core';
import { MatDialog, } from '@angular/material/dialog';
import { About } from '../components/about/about';

export interface MenuItem {
  /** name to display */
  name: string,

  /** mat-icon */
  icon?: string,

  /** children of this menu item */
  children?: MenuItem[],

  /** page route */
  path?: string,

  /** custom action */
  action?: () => void
}

@Injectable({
  providedIn: 'root'
})
export class MenuService {

  constructor(
    private readonly dialog: MatDialog
  ) {
  }

  // https://fonts.google.com/icons

  get menuItems() {
    const res: MenuItem[] = [
      {
        icon: 'info_outline',
        name: 'About',
        action: () => {
          this.dialog.open(About)
        }
      }
    ]

    return res
  }


}

import { Component, Input } from '@angular/core';
import { SnackNfo } from '../../services/snack-service';
import { MatIconModule } from "@angular/material/icon";

@Component({
  selector: 'app-stacked-snackbar',
  imports: [MatIconModule],
  templateUrl: './stacked-snackbar.html',
  styleUrl: './stacked-snackbar.scss'
})
export class StackedSnackbar {
  @Input() snacks: SnackNfo[] = []
}

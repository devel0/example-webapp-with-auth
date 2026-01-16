import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatSlideToggle } from '@angular/material/slide-toggle';
import { MatRadioModule } from '@angular/material/radio';
import { MatTabGroup } from '@angular/material/tabs';
import { MatIconModule } from "@angular/material/icon";
import { MatButtonModule } from '@angular/material/button';
import { MatTabsModule } from '@angular/material/tabs';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { TrackElementMetricsDirective } from '../../directives/track-element-metrics';

const modules = [
  CommonModule,
  FormsModule,
  MatButtonModule,
  MatFormFieldModule,
  MatIconModule,
  MatInputModule,
  MatRadioModule,
  MatSlideToggle,
  MatTabGroup,
  MatTabsModule,
  MatSelectModule,
  MatCheckboxModule,
  TrackElementMetricsDirective
]

@NgModule({
  declarations: [],
  imports: [...modules],
  exports: [...modules]
})
export class BasicModule { }

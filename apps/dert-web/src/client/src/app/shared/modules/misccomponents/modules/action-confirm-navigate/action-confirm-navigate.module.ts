import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatSelectModule } from '@angular/material/select';

import { ActionConfirmNavigateComponent } from './components/action-confirm-navigate.component';

@NgModule({
  imports: [
    CommonModule,
    MatButtonModule,
    MatCardModule,
    MatDividerModule,
    MatSelectModule,
    FlexLayoutModule
  ],
  declarations: [
    ActionConfirmNavigateComponent
  ],
  exports: [
    ActionConfirmNavigateComponent
  ]
})
export class ActionConfirmNavigateModule { }

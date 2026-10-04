import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { MatLegacyButtonModule as MatButtonModule } from '@angular/material/legacy-button';
import { MatLegacyCardModule as MatCardModule } from '@angular/material/legacy-card';
import { MatDividerModule } from '@angular/material/divider';
import { MatLegacySelectModule as MatSelectModule } from '@angular/material/legacy-select';

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

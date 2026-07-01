import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { MaterialLayoutsModule } from 'app/material-bundles/material-layouts.module';
import { AppSharedModule } from 'app/shared/app-shared.module';
import { ComplaintsComponent } from './components/complaints.component';

@NgModule({
  imports: [
    AppSharedModule,
    MaterialLayoutsModule,
    CommonModule,
    FlexLayoutModule,
    FormsModule,
    RouterModule,
  ],
  declarations: [ComplaintsComponent],
  exports: [ComplaintsComponent]
})
export class ComplaintsModule { }

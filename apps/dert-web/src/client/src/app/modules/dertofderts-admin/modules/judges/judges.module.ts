import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { MaterialLayoutsModule } from 'app/material-bundles/material-layouts.module';
import { AppSharedModule } from 'app/shared/app-shared.module';
import { JudgesComponent } from './components/judges.component';

@NgModule({
  imports: [
    AppSharedModule,
    MaterialLayoutsModule,
    CommonModule,
    RouterModule,
  ],
  declarations: [JudgesComponent],
  exports: [JudgesComponent],

})
export class JudgesModule { }

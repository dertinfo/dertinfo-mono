import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { MaterialLayoutsModule } from 'app/material-bundles/material-layouts.module';
import { NotificationModule } from 'app/modules/notification/notification.module';
import { AppSharedModule } from 'app/shared/app-shared.module';

import { AuthenticatedRegionComponent } from './authenticated-region.component';

@NgModule({
  imports: [
    AppSharedModule,
    MaterialLayoutsModule,
    CommonModule,
    FormsModule,
    BrowserModule,
    RouterModule,
    TranslateModule,
    NotificationModule
  ],
  declarations: [
    AuthenticatedRegionComponent,
  ],
  providers: [],
  exports: [
    AuthenticatedRegionComponent,
  ]
})
export class AuthenticatedRegionModule {}

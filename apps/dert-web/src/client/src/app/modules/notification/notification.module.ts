import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatTableModule } from '@angular/material/table';
import { RouterModule } from '@angular/router';

import { DetailDialogComponent } from './components/detail-dialog/detail-dialog.component';
import { DrawerComponent } from './components/drawer/drawer.component';
import { SummaryItemActiveComponent } from './components/summary-item-active/summary-item-active.component';
import { SummaryItemDeletedComponent } from './components/summary-item-deleted/summary-item-deleted.component';
import { TopbarButtonComponent } from './components/topbar-button/topbar-button.component';
import { NotificationCheckResolver } from './services/notification-check.resolver';
import { NotificationConductor } from './services/notification.conductor';
import { NotificationRepository } from './services/notification.repository';
import { NotificationTracker } from './services/notification.tracker';

@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    MatButtonModule,
    MatCardModule,
    MatDialogModule,
    MatIconModule,
    MatListModule,
    MatTableModule,
    FlexLayoutModule,

  ],
  declarations: [
    DrawerComponent,
    TopbarButtonComponent,
    DetailDialogComponent,
    SummaryItemActiveComponent,
    SummaryItemDeletedComponent
  ],
  exports: [
    DrawerComponent,
    TopbarButtonComponent,
  ],
  entryComponents: [
    DetailDialogComponent
  ],
  providers: [
    NotificationRepository,
    NotificationConductor,
    NotificationCheckResolver,
    NotificationTracker
  ]
})
export class NotificationModule { }

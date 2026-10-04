import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { MatLegacyButtonModule as MatButtonModule } from '@angular/material/legacy-button';
import { MatLegacyCardModule as MatCardModule } from '@angular/material/legacy-card';
import { MatLegacyDialogModule as MatDialogModule } from '@angular/material/legacy-dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatLegacyListModule as MatListModule } from '@angular/material/legacy-list';
import { MatLegacyTableModule as MatTableModule } from '@angular/material/legacy-table';
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
    providers: [
        NotificationRepository,
        NotificationConductor,
        NotificationCheckResolver,
        NotificationTracker
    ]
})
export class NotificationModule { }

import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { MaterialLayoutsModule } from 'app/material-bundles/material-layouts.module';
import { AppSharedModule } from 'app/shared/app-shared.module';
import { QuillModule } from 'ngx-quill';
import { CreateDialogComponent } from './components/create-dialog/create-dialog.component';
import { AdminDetailComponent } from './components/detail/admin-detail.component';
import { AdminListComponent } from './components/list/admin-list.component';
import { NotificationManagementRoutes } from './notification-management.routing';
import { NotificationAdminConductor } from './services/notification-management.conductor';
import { NotificationAdminMediator } from './services/notification-management.mediator';
import { NotificationAdminRepository } from './services/notification-management.repository';
import { NotificationManagementTracker } from './services/notification-management.tracker';

@NgModule({
    imports: [
        AppSharedModule,
        MaterialLayoutsModule,
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        RouterModule,
        QuillModule,
        NotificationManagementRoutes
    ],
    declarations: [
        AdminListComponent,
        AdminDetailComponent,
        CreateDialogComponent
    ],
    exports: [],
    providers: [
        NotificationAdminRepository,
        NotificationAdminConductor,
        NotificationManagementTracker,
        NotificationAdminMediator
    ]
})
export class NotificationManagementModule { }

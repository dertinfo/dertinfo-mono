import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { MaterialLayoutsModule } from 'app/material-bundles/material-layouts.module';
import { AppSharedModule } from '../../shared/app-shared.module';
import { DashboardComponent } from './dashboard.component';
import { DashboardRoutes } from './dashboard.routing';
import { DashboardConductor } from './services/dashboard.conductor';

import { EventRepository } from 'app/modules/repositories';

import { EventCreateComponent } from './event-create/event-create.component';
import { GdprDialogComponent } from './gdpr-dialog/gdpr-dislogue.component';
import { GroupCreateComponent } from './group-create/group-create.component';
import { DashboardRepository } from './services/dashboard.repository';

@NgModule({
  imports: [
    AppSharedModule,
    MaterialLayoutsModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    NgxDatatableModule,
    RouterModule.forChild(DashboardRoutes)
  ],
  declarations: [
    DashboardComponent,
    EventCreateComponent,
    GroupCreateComponent,
    GdprDialogComponent
  ],
  exports: [],
  providers: [
    DashboardConductor,
    EventRepository,
    DashboardRepository
  ],
  entryComponents: [
    EventCreateComponent,
    GdprDialogComponent,
    GroupCreateComponent
  ]
})
export class DashboardModule {

}

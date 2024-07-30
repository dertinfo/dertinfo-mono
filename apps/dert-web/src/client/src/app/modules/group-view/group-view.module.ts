import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { RouterModule } from '@angular/router';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { MaterialLayoutsModule } from 'app/material-bundles/material-layouts.module';
import {
  EventRepository,
  RegistrationRepository
} from 'app/modules/repositories';
import { AppSharedModule } from '../../shared/app-shared.module';
import { DertOfDertsReportDialogComponent } from './components/dertofderts-reportdialog/dertofderts-reportdialog.component';
import { DertOfDertsComponent } from './components/dertofderts/dertofderts.component';
import { GroupViewOverviewComponent } from './components/overview/overview.component';
import { DancesGridComponent } from './components/registration/dances-grid/dances-grid.component';
import { MarkingSheetZoomComponent } from './components/registration/dialogs/marking-sheet-zoom/marking-sheet-zoom.component';
import { GroupViewRegistrationComponent } from './components/registration/registration.component';
import { RegistrationResolver } from './components/registration/registration.resolver';
import { GroupViewComponent } from './group-view.component';
import { GroupViewRoutes } from './group-view.routing';
import { GroupViewDodResolver } from './services/group-view-dod.resolver';
import { GroupViewRegistrationsResolver } from './services/group-view-registrations.resolver';
import { GroupViewConductor } from './services/group-view.conductor';
import { GroupViewRepository } from './services/group-view.repository';
import { GroupViewResolver } from './services/group-view.resolver';
import { GroupViewTracker } from './services/group-view.tracker';

@NgModule({
    imports: [
        AppSharedModule,
        MaterialLayoutsModule,
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        MatCheckboxModule,
        MatSlideToggleModule,
        NgxDatatableModule,
        RouterModule.forChild(GroupViewRoutes)
    ],
    declarations: [
        DancesGridComponent,
        GroupViewComponent,
        GroupViewOverviewComponent,
        GroupViewRegistrationComponent,
        MarkingSheetZoomComponent,
        DertOfDertsComponent,
        DertOfDertsReportDialogComponent
    ],
    exports: [],
    providers: [
        GroupViewConductor,
        GroupViewTracker,
        GroupViewResolver,
        GroupViewDodResolver,
        GroupViewRegistrationsResolver,
        EventRepository,
        RegistrationRepository,
        RegistrationResolver,
        GroupViewRepository,
    ]
})
export class GroupViewModule {

}

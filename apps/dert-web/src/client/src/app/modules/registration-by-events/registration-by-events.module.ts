import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { MaterialLayoutsModule } from 'app/material-bundles/material-layouts.module';
import { FlowBreadcrumbModule } from 'app/shared/components/flow-breadcrumb/flow-breadcrumb.module';
import { MembersSelectModule } from 'app/shared/components/members-select/members-select.module';
import { TeamsSelectModule } from 'app/shared/components/teams-select/teams-select.module';
import { FileUploadModule } from 'ng2-file-upload';
import { AppSharedModule } from '../../shared/app-shared.module';
import { SystemDialogModalComponent } from './common/system-dialog-modal/system-dialog-modal.component';
import { EmailPreviewDialogComponent } from './components/email-preview-dialog/email-preview-dialog.component';
import { EventRegistrationActivitiesComponent } from './components/event-registration-activities/event-registration-activities.component';
import { EventRegistrationGuestsComponent } from './components/event-registration-guests/event-registration-guests.component';
import { EventRegistrationMembersComponent } from './components/event-registration-members/event-registration-members.component';
import { EventRegistrationOverviewComponent } from './components/event-registration-overview/event-registration-overview.component';
import { EventRegistrationTeamsComponent } from './components/event-registration-teams/event-registration-teams.component';
import { EventRegistrationTicketsComponent } from './components/event-registration-tickets/event-registration-tickets.component';
import { GroupDeletedDialogComponent } from './components/group-deleted-dialog/group-deleted-dialog.component';
import { GroupMembersSelectComponent } from './components/group-members-select/group-members-select.component';
import { GroupTeamsSelectComponent } from './components/group-teams-select/group-teams-select.component';
import { MemberActivitiesSelectComponent } from './components/member-activities-select/member-activities-select.component';
import { TeamActivitiesSelectComponent } from './components/team-activities-select/team-activities-select.component';
import { RegistrationByEventsComponent } from './registration-by-events.component';
import { RegistrationByEventsRoutes } from './registration-by-events.routing';
import { RegistrationByEventsConductor } from './services/registration-by-events.conductor';
import { RegistrationByEventsRepository } from './services/registration-by-events.repository';
import { RegistrationByEventsResolver } from './services/registration-by-events.resolver';
import { RegistrationByEventsTracker } from './services/registration-by-events.tracker';

@NgModule({
    imports: [
        AppSharedModule,
        MaterialLayoutsModule,
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        NgxDatatableModule,
        FileUploadModule,
        RouterModule.forChild(RegistrationByEventsRoutes),
        MembersSelectModule.forRoot(),
        TeamsSelectModule.forRoot(),
        FlowBreadcrumbModule
    ],
    declarations: [
        GroupDeletedDialogComponent,
        GroupMembersSelectComponent,
        GroupTeamsSelectComponent,
        RegistrationByEventsComponent,
        EventRegistrationOverviewComponent,
        EventRegistrationMembersComponent,
        EventRegistrationGuestsComponent,
        EventRegistrationTeamsComponent,
        EventRegistrationActivitiesComponent,
        EventRegistrationTicketsComponent,
        MemberActivitiesSelectComponent,
        SystemDialogModalComponent,
        EmailPreviewDialogComponent,
        TeamActivitiesSelectComponent,
    ],
    providers: [
        RegistrationByEventsConductor,
        RegistrationByEventsTracker,
        RegistrationByEventsResolver,
        RegistrationByEventsRepository
    ]
})
export class RegistrationByEventsModule { }

import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { MaterialLayoutsModule } from 'app/material-bundles/material-layouts.module';
import { MembersSelectModule } from 'app/shared/components/members-select/members-select.module';
import { TeamsSelectModule } from 'app/shared/components/teams-select/teams-select.module';
import { FileUploadModule } from 'ng2-file-upload';
import { AppSharedModule } from '../../shared/app-shared.module';
import { SystemDialogModalComponent } from './common/system-dialog-modal/system-dialog-modal.component';
import { EventDeletedDialogComponent } from './components/event-deleted-dialog/event-deleted-dialog.component';
import { GroupMembersSelectComponent } from './components/group-members-select/group-members-select.component';
import { GroupRegistrationActivitiesComponent } from './components/group-registration-activities/group-registration-activities.component';
import { GroupRegistrationGuestsComponent } from './components/group-registration-guests/group-registration-guests.component';
import { GroupRegistrationMembersComponent } from './components/group-registration-members/group-registration-members.component';
import { GroupRegistrationOverviewComponent } from './components/group-registration-overview/group-registration-overview.component';
import { GroupRegistrationTeamsComponent } from './components/group-registration-teams/group-registration-teams.component';
import { GroupRegistrationTicketsComponent } from './components/group-registration-tickets/group-registration-tickets.component';
import { GroupTeamsSelectComponent } from './components/group-teams-select/group-teams-select.component';
import { MemberActivitiesSelectComponent } from './components/member-activities-select/member-activities-select.component';
import { TeamActivitiesSelectComponent } from './components/team-activities-select/team-activities-select.component';
import { RegistrationByGroupComponent } from './registration-by-group.component';
import { RegistrationByGroupRoutes } from './registration-by-group.routing';
import { RegistrationByGroupConductor } from './services/registration-by-group.conductor';
import { RegistrationByGroupRepository } from './services/registration-by-group.repository';
import { RegistrationByGroupResolver } from './services/registration-by-group.resolver';
import { RegistrationByGroupTracker } from './services/registration-by-group.tracker';

@NgModule({
    imports: [
        AppSharedModule,
        MaterialLayoutsModule,
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        NgxDatatableModule,
        FileUploadModule,
        RouterModule.forChild(RegistrationByGroupRoutes),
        MembersSelectModule.forRoot(),
        TeamsSelectModule.forRoot(),
    ],
    declarations: [
        EventDeletedDialogComponent,
        GroupMembersSelectComponent,
        GroupTeamsSelectComponent,
        RegistrationByGroupComponent,
        GroupRegistrationOverviewComponent,
        GroupRegistrationMembersComponent,
        GroupRegistrationGuestsComponent,
        GroupRegistrationTeamsComponent,
        GroupRegistrationActivitiesComponent,
        GroupRegistrationTicketsComponent,
        MemberActivitiesSelectComponent,
        SystemDialogModalComponent,
        TeamActivitiesSelectComponent,
    ],
    providers: [
        RegistrationByGroupConductor,
        RegistrationByGroupResolver,
        RegistrationByGroupTracker,
        RegistrationByGroupRepository
    ]
})
export class RegistrationByGroupModule { }

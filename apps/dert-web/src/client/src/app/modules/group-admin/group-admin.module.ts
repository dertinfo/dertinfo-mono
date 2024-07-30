import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { MaterialLayoutsModule } from 'app/material-bundles/material-layouts.module';
import { MembersSelectModule } from 'app/shared/components/members-select/members-select.module';
import { TeamsSelectModule } from 'app/shared/components/teams-select/teams-select.module';
import { ActionConfirmNavigateModule } from 'app/shared/modules/misccomponents/modules/action-confirm-navigate/action-confirm-navigate.module';
import { FileUploadModule } from 'ng2-file-upload';
import { AppSharedModule } from '../../shared/app-shared.module';
import { GroupGalleryComponent } from './components/group-gallery/group-gallery.component';
import { GroupInvoicesComponent } from './components/group-invoices/group-invoices.component';
import { GroupMembersCreateComponent } from './components/group-members-create/group-members-create.component';
import { GroupMembersDetailComponent } from './components/group-members-detail/group-members-detail.component';
import { GroupMembersComponent } from './components/group-members/group-members.component';
import { GroupOverviewComponent } from './components/group-overview/group-overview.component';
import { GroupRegistrationsComponent } from './components/group-registrations/group-registrations.component';
import { GroupSettingsComponent } from './components/group-settings/group-settings.component';
import { GroupTeamsCreateComponent } from './components/group-teams-create/group-teams-create.component';
import { GroupTeamsDetailComponent } from './components/group-teams-detail/group-teams-detail.component';
import { GroupTeamsComponent } from './components/group-teams/group-teams.component';
import { InvoiceListingComponent } from './components/invoices-listing/invoices-listing.component';
import { RegistrationsListingForGroupComponent } from './components/registration-listing-for-group/registration-listing-for-group.component';
import { UploadImageComponent } from './components/upload-image/upload-image.component';
import { GroupAdminComponent } from './group-admin.component';
import { GroupAdminRoutes } from './group-admin.routing';
import { GroupAdminDodResolver } from './services/group-admin-dod.resolver';
import { GroupAdminConductor } from './services/group-admin.conductor';
import { GroupAdminRepository } from './services/group-admin.repository';
import { GroupAdminResolver } from './services/group-admin.resolver';
import { GroupAdminTracker } from './services/group-admin.tracker';

@NgModule({
    imports: [
        AppSharedModule,
        MaterialLayoutsModule,
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        ActionConfirmNavigateModule,
        NgxDatatableModule,
        FileUploadModule,
        RouterModule.forChild(GroupAdminRoutes),
        MembersSelectModule.forRoot(),
        TeamsSelectModule.forRoot(),
    ],
    declarations: [
        GroupAdminComponent,
        GroupInvoicesComponent,
        GroupOverviewComponent,
        GroupSettingsComponent,
        GroupMembersComponent,
        GroupMembersCreateComponent,
        GroupMembersDetailComponent,
        GroupRegistrationsComponent,
        RegistrationsListingForGroupComponent,
        GroupTeamsComponent,
        GroupTeamsCreateComponent,
        GroupTeamsDetailComponent,
        GroupGalleryComponent,
        InvoiceListingComponent,
        UploadImageComponent
    ],
    providers: [
        GroupAdminConductor,
        GroupAdminTracker,
        GroupAdminResolver,
        GroupAdminDodResolver,
        GroupAdminRepository
    ]
})
export class GroupAdminModule { }

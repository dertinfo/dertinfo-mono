import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { MaterialLayoutsModule } from 'app/material-bundles/material-layouts.module';
import { ActionConfirmNavigateModule } from 'app/shared/modules/misccomponents/modules/action-confirm-navigate/action-confirm-navigate.module';
import { FileUploadModule } from 'ng2-file-upload';
import { QuillModule } from 'ngx-quill';
import { AppSharedModule } from '../../shared/app-shared.module';
import { AppCompetitionSharedModule } from '../competition-admin/modules/app-competition-shared/app-competition-shared.module';
import { EventComponent } from './components/app-event.component';
import { EventCompetitionsComponent } from './components/competitions/competitions.component';
import { EventActivitiesCreateComponent } from './components/event-activities-create/event-activities-create.component';
import { EventActivitiesDetailComponent } from './components/event-activities-detail/event-activities-detail.component';
import { EventActivitiesComponent } from './components/event-activities/event-activities.component';
import { EventBlankComponent } from './components/event-blank/event-blank.component';
import { EventDownloadsComponent } from './components/event-downloads/event-downloads.component';
import { EventEmailTemplatesComponent } from './components/event-email-templates/event-email-templates.component';
import { EmailTemplatesResolver } from './components/event-email-templates/event-email-templates.resolver';
import { EventGalleryComponent } from './components/event-gallery/event-gallery.component';
import { EventInvoiceListingComponent } from './components/event-invoices/components/event-invoices-listing.component';
import { EventInvoicesComponent } from './components/event-invoices/event-invoices.component';
import { EventOverviewComponent } from './components/event-overview/event-overview.component';
import { EventRegistrationsComponent } from './components/event-registrations/event-registrations.component';
import { EventSettingsComponent } from './components/event-settings/event-settings.component';
import { PaperworkComponent } from './components/paperwork/paperwork.component';
import { RegistrationsListingForEventComponent } from './components/registration-listing-for-event/registration-listing-for-event.component';
import { UploadImageComponent } from './components/upload-image/upload-image.component';
import { EventAdminRoutes } from './event-admin.routing';
import { EventAdminConductor } from './services/event-admin.conductor';
import { EventAdminResolver } from './services/event-admin.resolver';
import { EventAdminTracker } from './services/event-admin.tracker';
@NgModule({
    imports: [
        AppSharedModule,
        MaterialLayoutsModule,
        ActionConfirmNavigateModule,
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        QuillModule,
        NgxDatatableModule,
        FileUploadModule,
        RouterModule.forChild(EventAdminRoutes),
        AppCompetitionSharedModule
    ],
    declarations: [
        EventActivitiesComponent,
        EventActivitiesCreateComponent,
        EventActivitiesDetailComponent,
        EventCompetitionsComponent,
        EventComponent,
        EventEmailTemplatesComponent,
        EventInvoicesComponent,
        EventInvoiceListingComponent,
        EventOverviewComponent,
        EventSettingsComponent,
        EventGalleryComponent,
        EventBlankComponent,
        RegistrationsListingForEventComponent,
        EventRegistrationsComponent,
        UploadImageComponent,
        EventDownloadsComponent,
        PaperworkComponent
    ],
    providers: [
        EventAdminConductor,
        EventAdminTracker,
        EventAdminResolver,
        EmailTemplatesResolver
    ]
})
export class EventAdminModule { }

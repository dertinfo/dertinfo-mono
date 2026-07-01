import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { MaterialLayoutsModule } from 'app/material-bundles/material-layouts.module';
import { EventConfigureTermsComponent } from 'app/regions/terms/components/event-configure-terms/event-configure-terms.component';
import { TermsRegionModule } from 'app/regions/terms/terms-region.module';
import { AppSharedModule } from 'app/shared/app-shared.module';
import { FileUploadModule } from 'ng2-file-upload';
import { StartComponent } from './components/start/start.component';
import { UploadImageComponent } from './components/upload-image/upload-image.component';
import { EventSetupComponent } from './event-setup.component';
import { EventSetupRoutes } from './event-setup.routing';
import { EventSetupConductor } from './services/event-setup.conductor';
import { EventSetupResolver } from './services/event-setup.resolver';
import { EventSetupTracker } from './services/event-setup.tracker';

@NgModule({
    imports: [
        AppSharedModule,
        MaterialLayoutsModule,
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        NgxDatatableModule,
        FileUploadModule,
        RouterModule.forChild(EventSetupRoutes),
        TermsRegionModule
    ],
    declarations: [
        EventSetupComponent,
        StartComponent,
        UploadImageComponent
    ],
    providers: [
        EventSetupConductor,
        EventSetupTracker,
        EventSetupResolver
    ]
})
export class EventSetupModule { }

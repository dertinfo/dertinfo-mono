import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { MaterialLayoutsModule } from 'app/material-bundles/material-layouts.module';
import { GroupConfigureTermsComponent } from 'app/regions/terms/components/group-configure-terms/group-configure-terms.component';
import { TermsRegionModule } from 'app/regions/terms/terms-region.module';
import { AppSharedModule } from 'app/shared/app-shared.module';
import { FileUploadModule } from 'ng2-file-upload';
import { StartComponent } from './components/start/start.component';
import { UploadImageComponent } from './components/upload-image/upload-image.component';
import { GroupSetupComponent } from './group-setup.component';
import { GroupSetupRoutes } from './group-setup.routing';
import { GroupSetupConductor } from './services/group-setup.conductor';
import { GroupSetupRepository } from './services/group-setup.repository';
import { GroupSetupResolver } from './services/group-setup.resolver';
import { GroupSetupTracker } from './services/group-setup.tracker';

@NgModule({
  imports: [
    AppSharedModule,
    MaterialLayoutsModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    NgxDatatableModule,
    FileUploadModule,
    RouterModule.forChild(GroupSetupRoutes),
    TermsRegionModule
  ],
  declarations: [
    GroupSetupComponent,
    StartComponent,
    UploadImageComponent
  ],
  providers: [
    GroupSetupConductor,
    GroupSetupTracker,
    GroupSetupResolver,
    GroupSetupRepository
  ],
  entryComponents: [
    UploadImageComponent,
    GroupConfigureTermsComponent
  ]
})
export class GroupSetupModule { }

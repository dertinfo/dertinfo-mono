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
import { GroupRegisterComponent } from './group-register.component';
import { GroupRegisterRoutes } from './group-register.routing';
import { GroupRegisterConductor } from './services/group-register.conductor';
import { GroupRegisterRepository } from './services/group-register.repository';
import { GroupRegisterResolver } from './services/group-register.resolver';
import { GroupRegisterTracker } from './services/group-register.tracker';

@NgModule({
  imports: [
    AppSharedModule,
    MaterialLayoutsModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    NgxDatatableModule,
    FileUploadModule,
    RouterModule.forChild(GroupRegisterRoutes),
    TermsRegionModule
  ],
  declarations: [
    GroupRegisterComponent,
    StartComponent
  ],
  providers: [
    GroupRegisterConductor,
    GroupRegisterTracker,
    GroupRegisterResolver,
    GroupRegisterRepository
  ],
  entryComponents: [
    GroupConfigureTermsComponent
  ]
})
export class GroupRegisterModule { }

import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { MaterialLayoutsModule } from 'app/material-bundles/material-layouts.module';
import { FileUploadModule } from 'ng2-file-upload';
import { AppSharedModule } from '../../shared/app-shared.module';
import { UserAccountOverviewComponent } from './components/user-account-overview/user-account-overview.component';
import { UserAccountSettingsComponent } from './components/user-account-settings/user-account-settings.component';
import { UserAccountConductor } from './services/user-account.conductor';
import { UserAccountComponent } from './user-account.component';
import { UserAccountRoutes } from './user-account.routing';

@NgModule({
    imports: [
        AppSharedModule,
        MaterialLayoutsModule,
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        NgxDatatableModule,
        FileUploadModule,
        RouterModule.forChild(UserAccountRoutes)
    ],
    declarations: [
        UserAccountComponent,
        UserAccountOverviewComponent,
        UserAccountSettingsComponent
    ],
    exports: [],
    providers: [
        UserAccountConductor
    ]
})
export class AppUserAccountModule {

}

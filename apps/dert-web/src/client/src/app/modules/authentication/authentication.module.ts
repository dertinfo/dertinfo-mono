import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { MaterialLayoutsModule } from 'app/material-bundles/material-layouts.module';
import { AppSharedModule } from 'app/shared/app-shared.module';
import { AuthenticationRoutes } from './authentication.routing';
import { SigninComponent } from './components/sign-in/sign-in.component';
import { SignoutComponent } from './components/sign-out/sign-out.component';

@NgModule({
    imports: [
        AppSharedModule,
        MaterialLayoutsModule,
        RouterModule.forChild(AuthenticationRoutes),
    ],
    declarations: [
        SigninComponent,
        SignoutComponent
    ],
    providers: [],
    exports: []
})
export class AuthenticationModule { }

import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AuthenticationRoutes } from './authentication.routing';
import { SigninComponent } from './components/sign-in/sign-in.component';
import { SignoutComponent } from './components/sign-out/sign-out.component';

@NgModule({
    imports: [
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

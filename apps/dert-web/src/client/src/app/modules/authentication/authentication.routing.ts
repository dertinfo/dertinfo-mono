import { Routes } from '@angular/router';
import { SigninComponent } from '../authentication/components/sign-in/sign-in.component';
import { SignoutComponent } from './components/sign-out/sign-out.component';

export const AuthenticationRoutes: Routes = [{
      path: 'signin',
      component: SigninComponent,
      data: { title: 'Signin' }
    }, {
      path: 'signout',
      component: SignoutComponent,
      data: { title: 'Signout' }
    }];

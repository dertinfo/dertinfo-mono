import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';


import { CallbackPage } from './callback.page';
import { SignInPage } from './sign-in.page';
import { SignOutPage } from './sign-out.page';
import { SilentPage } from './silent.page';

const routes: Routes = [
  {
    path: 'signin',
    component: SignInPage
  },
  {
    path: 'signout',
    component: SignOutPage
  },
  {
    path: 'callback',
    component: CallbackPage
  },
  {
    path: 'silent',
    component: SilentPage
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AuthRoutingModule {}

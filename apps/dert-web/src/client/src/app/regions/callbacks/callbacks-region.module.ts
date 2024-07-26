import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CallbacksRegionComponent } from './callbacks-region.component';
import { AuthCallbackComponent } from './components/auth-callback/auth-callback.component';
import { AuthSilentComponent } from './components/auth-silent/auth-silent.component';

@NgModule({
  imports: [
    RouterModule
  ],
  declarations: [
    AuthCallbackComponent,
    AuthSilentComponent,
    CallbacksRegionComponent
  ],
  providers: [],
  exports: [
    CallbacksRegionComponent
  ]
})
export class CallbacksRegionModule {}

import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CallbacksRegionComponent } from './callbacks-region.component';
import { AuthCallbackComponent } from './components/auth-callback/auth-callback.component';

@NgModule({
  imports: [
    RouterModule
  ],
  declarations: [
    AuthCallbackComponent,
    CallbacksRegionComponent
  ],
  providers: [],
  exports: [
    CallbacksRegionComponent
  ]
})
export class CallbacksRegionModule {}

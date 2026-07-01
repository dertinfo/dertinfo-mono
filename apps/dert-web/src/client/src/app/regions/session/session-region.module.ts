import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { SessionRegionComponent } from './session-region.component';

@NgModule({
  imports: [
    RouterModule
  ],
  declarations: [
    SessionRegionComponent,
  ],
  providers: [],
  exports: [
    SessionRegionComponent
  ]
})
export class SessionRegionModule {}

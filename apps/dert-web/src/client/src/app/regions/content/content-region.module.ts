import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ContentRegionComponent } from './content-region.component';

@NgModule({
  imports: [
    RouterModule
  ],
  declarations: [
    ContentRegionComponent
  ],
  providers: [],
  exports: [
    ContentRegionComponent
  ]
})
export class ContentRegionModule {}

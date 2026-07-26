import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ClientSettingsResolver } from 'app/core/resolvers/clientsettings.resolver';
import { DertOfDertsRegionComponent } from './dertofderts-region.component';

@NgModule({
  imports: [
    RouterModule
  ],
  declarations: [
    DertOfDertsRegionComponent
  ],
  providers: [
    ClientSettingsResolver
  ],
  exports: [
    DertOfDertsRegionComponent
  ]
})
export class DertOfDertsRegionModule {}

import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ClientSettingsResolver } from 'app/core/resolvers/clientsettings.resolver';
import { WarmupResolver } from 'app/core/resolvers/warmup.resolver';
import { DertOfDertsRegionComponent } from './dertofderts-region.component';

@NgModule({
  imports: [
    RouterModule
  ],
  declarations: [
    DertOfDertsRegionComponent
  ],
  providers: [
    WarmupResolver,
    ClientSettingsResolver
  ],
  exports: [
    DertOfDertsRegionComponent
  ]
})
export class DertOfDertsRegionModule {}

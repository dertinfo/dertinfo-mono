import { NgModule } from '@angular/core';

import { EventService } from './event.service';
import { GroupService } from './group.service';
import { VenueService } from './venue.service';
import { DanceService } from './dance.service';
import { PinCodeService } from './pin-code.service';

import { IonicStorageModule } from '@ionic/storage-angular';
import { ConfigurationService } from './configuration.service';

@NgModule({
  imports: [
    IonicStorageModule.forRoot()
  ],
  declarations: [],
  exports: [],
  providers: [
    EventService,
    GroupService,
    VenueService,
    DanceService,
    PinCodeService,
  ]
})
export class AppServicesModule {}

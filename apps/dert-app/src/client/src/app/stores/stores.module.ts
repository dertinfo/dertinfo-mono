import { NgModule } from '@angular/core';

import { EventStore } from './event.store';
import { GroupStore } from './group.store';
import { VenueStore } from './venue.store';
import { DanceStore } from './dance.store';

@NgModule({
  imports: [],
  declarations: [],
  exports: [],
  providers: [
    EventStore,
    GroupStore,
    VenueStore,
    DanceStore
  ]
})
export class AppStoresModule {}

import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { Subscription } from 'rxjs';

import { VenuesSelectMediator } from 'app/shared/components/venues-select/services/venues-select.mediator';
import { VenueDto } from 'app/models/dto';
import { CompetitionAdminConductor } from 'app/modules/competition-admin/services/competition-admin.conductor';
import { CompetitionAdminTracker } from 'app/modules/competition-admin/services/competition-admin.tracker';

@Component({
  selector: 'app-venue-create',
  templateUrl: './venue-create.template.html'
})
export class VenueCreateComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];
  private _allVenues: VenueDto[] = [];

  form: FormGroup;

  constructor(
    private composeDialog: MatDialog,
    private _selectCreateMediator: VenuesSelectMediator,
    private _conductor: CompetitionAdminConductor,
    private _tracker: CompetitionAdminTracker,
  ) {
  }

  ngOnInit() {

    this._subscriptions.push(this._selectCreateMediator.newItemChange$.subscribe(newVenueSubmission => {
      const subs = this._conductor.addVenue(newVenueSubmission).subscribe(
        (savedMember) => {
          subs.unsubscribe();
          this.composeDialog.closeAll();
        },
        (error) => {
          throw new Error('VenueCreateComponent - Adding Venue Failed');
        }
      );
    }));

    this._subscriptions.push(this._selectCreateMediator.itemsChange$.subscribe(changedSelections => {
      const subs = this._conductor.toggleVenueAllocations(changedSelections).subscribe(
        (changes) => {
          subs.unsubscribe();
          this.composeDialog.closeAll();
        },
        (error) => {
          throw new Error('VenueCreateComponent - Updating Venues Failed');
        }
      );
    }));

    this._subscriptions.push(this._tracker.allVenues$.subscribe((venues) => {

      if (venues) {
        this._selectCreateMediator.applyFullSet(venues);
      }
    }));

    this._subscriptions.push(this._tracker.venues$.subscribe((venues) => {

      if (venues) {

        // note the spread here is filthy. Just cannot seem to work out why base collection getting changed.
        this._selectCreateMediator.applySelectedSet([...venues]);
      }
    }));

    this._subscriptions.push(this._selectCreateMediator.closeModal$.subscribe(() => {
      this.composeDialog.closeAll();
    }));

    this._conductor.initAllVenues();
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }
}

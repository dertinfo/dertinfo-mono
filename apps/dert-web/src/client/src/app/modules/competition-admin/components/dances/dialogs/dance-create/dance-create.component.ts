import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { Subscription } from 'rxjs';

import { DanceAdditionSubmissionDto } from 'app/models/dto';
import { CompetitionAdminConductor } from 'app/modules/competition-admin/services/competition-admin.conductor';
import { CompetitionAdminTracker } from 'app/modules/competition-admin/services/competition-admin.tracker';

@Component({
    selector: 'app-dance-create',
    templateUrl: './dance-create.component.html'
})
export class DanceCreateComponent implements OnInit, OnDestroy {

    private _subscriptions: Subscription[] = [];

    form: UntypedFormGroup;

    public get teamEntrants() {
        return this._tracker.entrants$;
    }

    public get venues() {
        return this._tracker.venues$;
    }

    constructor(
        private composeDialog: MatDialog,
        private _conductor: CompetitionAdminConductor,
        private _tracker: CompetitionAdminTracker,

    ) {
    }

    ngOnInit() {

        if (!this._tracker.hasLoadedVenues()) {
            this._conductor.initVenues();
        }

        if (!this._tracker.hasLoadedEntrants()) {
            this._conductor.initEntrants();
        }

        this.form = new UntypedFormGroup({
            competitionEntryId: new UntypedFormControl('', [Validators.required]),
            venueId: new UntypedFormControl('', [Validators.required])
        });
    }

    ngOnDestroy() {

        this._subscriptions.map((sub) => { sub.unsubscribe(); });
        this._subscriptions = []; // For good measure
    }

    onCreateSubmit() {

        if (this.form.valid) {

            const danceAdditionSubmission: DanceAdditionSubmissionDto = {
                competitionEntryId: this.form.value.competitionEntryId,
                venueId: this.form.value.venueId,
            };

            const subs = this._conductor.addDance(danceAdditionSubmission).subscribe(
                (saved) => {
                    subs.unsubscribe();
                    this.composeDialog.closeAll();
                },
                (error) => {
                    console.error('ERROR: Adding Dance Failed');
                }
            );
        }
    }

    onCancel() {
        this.composeDialog.closeAll();
    }
}

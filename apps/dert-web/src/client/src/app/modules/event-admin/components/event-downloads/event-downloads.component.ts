import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { RegistrationFlowState } from 'app/models/app/Enumerations/RegistrationFlowStates';
import { Subscription } from 'rxjs';
import { EventAdminConductor } from '../../services/event-admin.conductor';
import { EventAdminTracker } from '../../services/event-admin.tracker';

@Component({
    selector: 'app-event-downloads',
    templateUrl: './event-downloads.component.html',
    styleUrls: ['./event-downloads.component.css']
})
export class EventDownloadsComponent implements OnInit, OnDestroy {

    private _subscriptions: Subscription[] = [];

    public confirmedTeams;

    constructor(
        private router: ActivatedRoute,
        private _conductor: EventAdminConductor,
        private _tracker: EventAdminTracker,

    ) { }

    ngOnInit() {

        this._subscriptions.push(this._tracker.downloads$.subscribe(confirmedTeams => {

            this.confirmedTeams = confirmedTeams;
        }));

        this._subscriptions.push(this.router.parent.params.subscribe(params => {
            this._conductor.initDownloads(params['id']);
        }));
    }

    ngOnDestroy() {
        this._subscriptions.map((sub) => { sub.unsubscribe(); });
        this._subscriptions = []; // For good measure
    }

}

import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute} from '@angular/router';
import { Subscription } from 'rxjs';
import { SystemDialogModalComponent } from './common/system-dialog-modal/system-dialog-modal.component';
import { EventDeletedDialogComponent } from './components/event-deleted-dialog/event-deleted-dialog.component';
import { RegistrationByGroupConductor } from './services/registration-by-group.conductor';
import { RegistrationByGroupTracker } from './services/registration-by-group.tracker';

declare function require(url: string);
const registrationFlowStates = require('../../../assets/staticdata/registration-flow-states.en.json');

@Component({
  selector: 'app-registration-by-group',
  templateUrl: './registration-by-group.component.html',
  styleUrls: ['./registration-by-group.component.scss']
})
export class RegistrationByGroupComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];
  private activeView: string = 'start';

  public informationHeader = '';
  public informationBody = '';

  public flowStates = [];
  public flowStateSelection = 0;
  public flowStateMessage: string = null;

  public attendingMembersCount = 0;
  public attendingGuestsCount = 0;
  public attendingTeamsCount = 0;

  @ViewChild('systemDialog', { static: true }) systemDialog: SystemDialogModalComponent;

  public get capabilities() {
    return this._tracker.capabilities;
  }

  constructor(

    private _activatedRoute: ActivatedRoute,
    private _conductor: RegistrationByGroupConductor,
    private _tracker: RegistrationByGroupTracker,
  ) { }

  ngOnInit() {

    // Listen for changes to Route Paramaters
    this._subscriptions.push(this._activatedRoute.params.subscribe(params => {
      // Insert the data from the resolver
      this._conductor.setOverview(this._activatedRoute.snapshot.data.groupRegistrationOverview);
      this.activeView = this._activatedRoute.snapshot.params['view'];
    }));

    this._subscriptions.push(this._tracker.registrationOverview$.subscribe(registrationOverview => {

      this.attendingTeamsCount = registrationOverview ? registrationOverview.teamAttendancesCount : 0;
      this.attendingGuestsCount = registrationOverview ? registrationOverview.guestAttendancesCount : 0;
      this.attendingMembersCount = registrationOverview ? registrationOverview.memberAttendancesCount : 0;
      this.flowStateSelection = registrationOverview ? registrationOverview.flowState : 0;

      if (registrationOverview && registrationOverview.isEventDeleted) {

        this.systemDialog.openSystemDialog(EventDeletedDialogComponent);

        this.flowStateMessage = 'This registration has been cancelled as the event has been deleted by the owner';
        this.flowStates = [{
          id: registrationOverview.flowState,
          name: 'Event Removed',
          info: 'This registration has been cancelled as the event has been deleted by the owner'
        }];

      } else {
        this.flowStates = registrationFlowStates;
      }
    }));
  }

  ngOnDestroy() {
    this._conductor.clear();
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  public onRegistrationSubmit() {
    this._conductor.submitRegistration();
  }
}

import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { EventOverviewDto } from 'app/models/dto';
import { FlowBreadcrumbItem } from 'app/shared/components/flow-breadcrumb/models/flow-breadcrumb-item.model';
import { Observable ,  Subscription } from 'rxjs';
import { RegistrationFlowState } from '../../models/app/Enumerations/RegistrationFlowStates';
import { SystemDialogModalComponent } from './common/system-dialog-modal/system-dialog-modal.component';
import { EmailPreviewDialogComponent } from './components/email-preview-dialog/email-preview-dialog.component';
import { GroupDeletedDialogComponent } from './components/group-deleted-dialog/group-deleted-dialog.component';
import { RegistrationByEventsConductor } from './services/registration-by-events.conductor';
import { RegistrationByEventsTracker } from './services/registration-by-events.tracker';

declare function require(url: string);
const registrationFlowStates = require('../../../assets/staticdata/registration-flow-states.en.json');

@Component({
  selector: 'app-registration-by-events',
  templateUrl: './registration-by-events.component.html',
  styleUrls: ['./registration-by-events.component.css']
})
export class RegistrationByEventsComponent implements OnInit, OnDestroy {

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
    return this._conductor.capabilities;
  }

  constructor(

    private _activatedRoute: ActivatedRoute,
    private _conductor: RegistrationByEventsConductor,
    private _tracker: RegistrationByEventsTracker,
    public composeDialog: MatDialog,
  ) { }

  ngOnInit() {

    // Listen for changes to Route Paramaters
    this._subscriptions.push(this._activatedRoute.params.subscribe(params => {
      // Insert the data from the resolver
      this._conductor.setOverview(this._activatedRoute.snapshot.data.eventRegistrationOverview);
      this.activeView = this._activatedRoute.snapshot.params['view'];
    }));

    // Listen for changes to the overview
    this._subscriptions.push(this._tracker.registrationOverview$.subscribe(registrationOverview => {
      this.attendingTeamsCount = registrationOverview ? registrationOverview.teamAttendancesCount : 0;
      this.attendingGuestsCount = registrationOverview ? registrationOverview.guestAttendancesCount : 0;
      this.attendingMembersCount = registrationOverview ? registrationOverview.memberAttendancesCount : 0;
      this.flowStateSelection = registrationOverview ? registrationOverview.flowState : 0;

      this.flowStates = registrationFlowStates.map(fs => {
        const flowState: FlowBreadcrumbItem = {
          id: fs.id,
          name: fs.name,
          selected: fs.id === this.flowStateSelection,
          info: fs.info
        };

        return flowState;
      });

      // if (registrationOverview && registrationOverview.isEventDeleted) {
      //
      //   this.systemDialog.openSystemDialog(GroupDeletedDialogComponent);

      //   this.flowStateMessage = 'This registration has been cancelled as the group has been deleted by the owner';
      //   this.flowStates = [{
      //     id: registrationOverview.flowState,
      //     name: "Group Removed",
      //     info: 'This registration has been cancelled as the group has been deleted by the owner'
      //   }];

      // } else {
      //   this.flowStates = registrationFlowStates;
      // }
    }));
  }

  ngOnDestroy() {
    this._conductor.clear();
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  public onRegistrationConfirm() {
    this._conductor.confirmRegistration();
  }

  public onPreviewConfirmationEmail() {
    this._conductor.getConfirmationEmailPreview().subscribe((previewHtml: string) => {
      const dialogRef = this.composeDialog.open(EmailPreviewDialogComponent, {
        data: { previewHtml },
      });
      dialogRef.afterClosed().subscribe(result => {

      });
    });
  }
}

import { AfterContentInit, Component, HostBinding, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import * as domHelper from '../../helpers/dom.helper';

import { DashboardConductor } from './services/dashboard.conductor';

import { ActivatedRoute } from '@angular/router';
import { GroupAccessContext } from 'app/models/app/Enumerations/GroupAccessContext';
import { Subscription } from 'rxjs';
import { EventDto, GroupDto, GroupOverviewDto } from '../../models/dto';
import { AuthService } from '../../core/authentication/auth.service';
import { EventCreateComponent } from './event-create/event-create.component';
import { GdprDialogComponent } from './gdpr-dialog/gdpr-dislogue.component';
import { GroupCreateComponent } from './group-create/group-create.component';

@Component({
  selector: 'dashboard',
  templateUrl: './dashboard.template.html',
  styleUrls: ['./dashboard.template.css']
})
export class DashboardComponent implements OnInit, OnDestroy, AfterContentInit {

  public get GroupAccessContext() { return GroupAccessContext; }

  public get totalGroupsCount(): number {
    const groupsCount = this.groupsData ? this.groupsData.length : 0;
    const memberGroupsCount = this.memberGroupsData ? this.memberGroupsData.length : 0;
    return groupsCount + memberGroupsCount;
  }

  private _subscriptions: Subscription[] = [];

  groupsData: GroupDto[] = [];
  memberGroupsData: GroupDto[] = [];
  eventsData: EventDto[] = [];
  dataRolloutCounter = 0;

  flyoutMenuItems = [{
    matIcon: 'group_work',
    name: 'create-group'
  }
    // , {
    //   matIcon: 'event',
    //   name: 'create-event'
    // }
  ];

  constructor(
    public composeDialog: MatDialog,
    public authService: AuthService,
    private _conductor: DashboardConductor,
    private _activatedRoute: ActivatedRoute,
  ) { }

  ngOnInit() {

    this.subscribe();

    // note - these are now done using a resolver.
    // this.dashboardConductor.refreshGroups();
    // this.dashboardConductor.refreshEvents();

    // apply the settings through the conductor to the tracker from the resolver
    this._conductor.applyGroups(this._activatedRoute.snapshot.data.dashboarddata[0]);
    this._conductor.applyEvents(this._activatedRoute.snapshot.data.dashboarddata[1]);

    if (this.authService.userData().email === 'info@dert2014.co.uk' || this.authService.userData().email.startsWith('davidsmonkeys')) {
      this.flyoutMenuItems.push({
        matIcon: 'event',
        name: 'create-event'
      });
    }
  }

  ngAfterContentInit() {
    setTimeout(() => {

      const userData = this.authService.userData();

      if (!userData.gdprConsentGained) {
        this.openGdprDialog();
      }
    }, 1000);

  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  openCreateGroupDialog() {
    const groupDialogRef = this.composeDialog.open(GroupCreateComponent);
    const dialogueSubscription = groupDialogRef.afterClosed().subscribe(result => {
      dialogueSubscription.unsubscribe();
    });
  }

  openCreateEventDialog() {
    const eventDialogRef = this.composeDialog.open(EventCreateComponent);
    const dialogueSubscription = eventDialogRef.afterClosed().subscribe(result => {
      dialogueSubscription.unsubscribe();
    });
  }

  openGdprDialog() {
    const eventDialogRef = this.composeDialog.open(GdprDialogComponent);
    const dialogueSubscription = eventDialogRef.afterClosed().subscribe(result => {
      dialogueSubscription.unsubscribe();
    });
  }

  onFabFlyoutItemClicked(buttonName: string) {
    switch (buttonName) {
      case 'create-group':
        this.openCreateGroupDialog();
        break;
      case 'create-event':
        this.openCreateEventDialog();
        break;
    }
  }

  public getGroupDetailRouteLink(group: GroupDto) {
    if (group.userAccessContext === GroupAccessContext.adminaccess) {
      return ['/group/' + encodeURIComponent(group.groupName) + '/' + group.id + '/overview'];
    }
    return ['/group-view/' + encodeURIComponent(group.groupName) + '/' + group.id + ''];
  }

  public getGroupDetailRouteLinkForMember(group: GroupDto) {
    return ['/group-view/' + encodeURIComponent(group.groupName) + '/' + group.id + ''];
  }

  public getEventDetailRouteLink(event: EventDto) {
    return ['/event/' + encodeURIComponent(event.name) + '/' + event.id + '/overview'];
  }

  public getEventConfigureRouteLink(event: EventDto) {
    return ['/event-configure/' + encodeURIComponent(event.name) + '/' + event.id + '/start'];
  }

  public getGroupConfigureRouteLink(group: GroupDto) {
    return ['/group-configure/' + encodeURIComponent(group.groupName) + '/' + group.id + '/start'];
  }

  private subscribe() {
    const groupsSubscription = this._conductor.groups$.subscribe(data => {
      this.groupsData = data.filter(g => g.userAccessContext === GroupAccessContext.adminaccess);
      this.memberGroupsData = data.filter(g => g.userAccessContext === GroupAccessContext.memberaccess);
    });

    const eventsSubscription = this._conductor.events$.subscribe(data => {
      this.eventsData = data;
    });

    this._subscriptions.push(groupsSubscription);
    this._subscriptions.push(eventsSubscription);
  }
}

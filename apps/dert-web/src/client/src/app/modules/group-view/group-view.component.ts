import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { GroupDto, GroupRegistrationDto } from 'app/models/dto';
import { Observable, Subscription } from 'rxjs';
import { GroupViewConductor } from './services/group-view.conductor';

import { GroupAccessContext } from 'app/models/app/Enumerations/GroupAccessContext';
import { GroupViewTracker } from './services/group-view.tracker';

@Component({
  selector: 'app-group-view',
  templateUrl: './group-view.component.html',
  styleUrls: ['./group-view.component.css']
})
export class GroupViewComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];

  public group: GroupDto;
  public userIsAdmin: boolean = false;
  public showDertOfDertsMenu: boolean = false;

  public get groupRegistrations$(): Observable<GroupRegistrationDto[]> {
    return this._tracker.groupRegistrations$;
  }

  constructor(

    private _activatedRoute: ActivatedRoute,
    private _router: Router,
    private _conductor: GroupViewConductor,
    private _tracker: GroupViewTracker
  ) { }

  ngOnInit() {

    // Listen to changes on the tracker "group"
    this._subscriptions.push(this._tracker.group$.subscribe((group: GroupDto) => {
      if (group != null) {
        this.group = group;
        this.userIsAdmin = this.group.userAccessContext === GroupAccessContext.adminaccess;
      }
    }));

    // Listen to changes on the route "params"
    this._subscriptions.push(this._activatedRoute.params.subscribe(params => {

      // Insert the data from the resolver
      this._conductor.applyGroup(this._activatedRoute.snapshot.data.group);
      this._conductor.applyRegistrations(this._activatedRoute.snapshot.data.registrations);

      this.showDertOfDertsMenu = this._activatedRoute.snapshot.data.showDertOfDerts;

      // note - we will need this later to highlight the navigation in this page.
      // this.activeView = this._activatedRoute.snapshot.params['view'];
    }));

    // Listen to changes on the tracker "groupRegistrations$"
    this._subscriptions.push(this._tracker.groupRegistrations$.subscribe((groupRegistrations: GroupRegistrationDto[]) => {
      // this.groupRegistrations = groupRegistrations;
    }));
  }

  ngOnDestroy() {

    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
    this._conductor.clearState();
  }

  public onRegistrationClick(registration: GroupRegistrationDto) {
    console.log('Registration Clicked', registration);
  }
}

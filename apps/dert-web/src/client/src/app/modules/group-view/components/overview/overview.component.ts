import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { GroupDto } from 'app/models/dto';
import { Subscription } from 'rxjs';
import { GroupViewConductor } from '../../services/group-view.conductor';

import { GroupAccessContext } from 'app/models/app/Enumerations/GroupAccessContext';
import { GroupViewTracker } from '../../services/group-view.tracker';

@Component({
  selector: 'app-groupview-overview',
  templateUrl: './overview.component.html',
  styleUrls: ['./overview.component.css']
})
export class GroupViewOverviewComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];

  public group: GroupDto;
  public userIsAdmin: boolean = false;

  constructor(

    private _activatedRoute: ActivatedRoute,
    private _router: Router,
    private _conductor: GroupViewConductor,
    private _tracker: GroupViewTracker
  ) { }

  ngOnInit() {

    this._subscriptions.push(this._tracker.group$.subscribe((group: GroupDto) => {
      if (group != null) {
        this.group = group;
        this.userIsAdmin = this.group.userAccessContext === GroupAccessContext.adminaccess;
      }
    }));
  }

  ngOnDestroy() {

    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  public switchToAdmin() {

    const groupId: number = this.group.id;
    const groupName: string = this.group.groupName;
    this._router.navigate([`group/${groupName}/${groupId}`]);
  }

}

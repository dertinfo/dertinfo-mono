
import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute } from '@angular/router';
import { GroupMemberDto } from 'app/models/dto';
import { Subscription } from 'rxjs';
import { Observable } from 'rxjs/Observable';
import { map } from 'rxjs/operators';
import { MemberType } from '../../../../models/app/Enumerations/MemberType';
import { GroupAdminConductor } from '../../services/group-admin.conductor';
import { GroupAdminTracker } from '../../services/group-admin.tracker';
import { GroupMembersCreateComponent } from '../group-members-create/group-members-create.component';

@Component({
  selector: 'app-group-members',
  templateUrl: './group-members.component.html',
  styleUrls: ['./group-members.component.scss']
})
export class GroupMembersComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];

  public membersDataSource = new MatTableDataSource<GroupMemberDto>();
  public guestsDataSource = new MatTableDataSource<GroupMemberDto>();
  public tableColumnsAll: string[] = ['gravitar', 'name', 'edit'];
  public tableColumnsSort: MatSort;

  public groupMembers$: Observable<GroupMemberDto[]>;
  public groupGuests$: Observable<GroupMemberDto[]>;

  constructor(
    public composeDialog: MatDialog,
    private router: ActivatedRoute,
    private groupConductor: GroupAdminConductor,
    private _groupTracker: GroupAdminTracker
  ) { }

  ngOnInit() {

    this.groupMembers$ = this._groupTracker.groupMembers$.pipe(
      map(members => members ? members.filter(member => member.memberType === MemberType.active) : []));

    this.groupGuests$ = this._groupTracker.groupMembers$.pipe(
      map(members => members ? members.filter(member => member.memberType === MemberType.guest) : []));

    this.listenForDataTableChanges();

    const routeParamsSubscription = this.router.parent.params.subscribe(params => {
      this.groupConductor.initMembers(params['id']);
    });

    this._subscriptions.push(routeParamsSubscription);
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  openCreateMemberDialog() {
    const dialogRef = this.composeDialog.open(GroupMembersCreateComponent);
    const dialogueSubscription = dialogRef.afterClosed().subscribe(result => {
      dialogueSubscription.unsubscribe();
    });
  }

  onGroupMemberRemoveClick(groupMember: GroupMemberDto) {
    this.groupConductor.removeMember(groupMember);
  }

  public getDetailRouteLink(groupMember: GroupMemberDto) {
    return ['./' + encodeURIComponent(groupMember.name) + '/' + groupMember.groupMemberId];
  }

  private listenForDataTableChanges() {

    this._subscriptions.push(this.groupMembers$.subscribe((data) => {
      this.membersDataSource = new MatTableDataSource<GroupMemberDto>(data);
    }));

    this._subscriptions.push(this.groupGuests$.subscribe((data) => {
      this.guestsDataSource = new MatTableDataSource<GroupMemberDto>(data);
    }));
  }

}

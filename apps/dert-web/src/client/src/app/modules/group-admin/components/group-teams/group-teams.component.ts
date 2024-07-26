import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { GroupTeamDto } from 'app/models/dto';
import { Observable ,  Subscription } from 'rxjs';
import { GroupAdminConductor } from '../../services/group-admin.conductor';
import { GroupAdminTracker } from '../../services/group-admin.tracker';
import { GroupTeamsCreateComponent } from '../group-teams-create/group-teams-create.component';

@Component({
  selector: 'app-group-teams',
  templateUrl: './group-teams.component.html',
  styleUrls: ['./group-teams.component.css']
})
export class GroupTeamsComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];

  public groupTeams$: Observable<GroupTeamDto[]>;

  constructor(
    public composeDialog: MatDialog,
    private router: ActivatedRoute,
    private groupConductor: GroupAdminConductor,
    private _groupTracker: GroupAdminTracker
  ) { }

  ngOnInit() {

    this.groupTeams$ = this._groupTracker.groupTeams$; // Subscribe Direct uses | async

    const routeParamsSubscription = this.router.parent.params.subscribe(params => {
      this.groupConductor.initTeams(params['id']);
    });

    this._subscriptions.push(routeParamsSubscription);
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  openCreateTeamDialog() {
    const dialogRef = this.composeDialog.open(GroupTeamsCreateComponent);
    const dialogueSubscription = dialogRef.afterClosed().subscribe(result => {
      dialogueSubscription.unsubscribe();
    });
  }

  onGroupTeamRemoveClick(groupTeam: GroupTeamDto) {
    this.groupConductor.removeTeam(groupTeam);
  }

  private getDetailRouteLink(team: GroupTeamDto) {
    return ['./' + encodeURIComponent(team.teamName) + '/' + team.teamId];
  }

}

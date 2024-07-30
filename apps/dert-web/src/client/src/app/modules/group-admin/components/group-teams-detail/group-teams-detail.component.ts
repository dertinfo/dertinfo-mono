import { Location } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { GroupImage } from 'app/models/app';
import { GroupTeamDetailDto, GroupTeamDto, GroupTeamUpdateDto, TeamImageSubmissionDto } from 'app/models/dto';
import { Observable ,  Subscription } from 'rxjs';
import { GroupAdminConductor } from '../../services/group-admin.conductor';
import { GroupAdminTracker } from '../../services/group-admin.tracker';

@Component({
  selector: 'app-group-teams-detail',
  templateUrl: './group-teams-detail.component.html',
  styleUrls: ['./group-teams-detail.component.css']
})
export class GroupTeamsDetailComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];
  private _teamNamePattern = '^(?!.*\/).*$';
  private groupTeam: GroupTeamDetailDto;
  private teamUpdateForm: UntypedFormGroup;

  private rows = [];

  public _isReady = false;

  public teamImages$: Observable<GroupImage[]>;
  public groupTeams$: Observable<GroupTeamDto[]>;

  constructor(
    private router: ActivatedRoute,
    private groupConductor: GroupAdminConductor,
    private _location: Location,
    private _groupTracker: GroupAdminTracker
  ) { }

  ngOnInit() {

    this.teamImages$ = this._groupTracker.teamImages$; // Subscribe Direct uses | async
    this.groupTeams$ = this._groupTracker.groupTeams$;

    const focussedTeamSubscription = this._groupTracker.focussedGroupTeam$.subscribe((groupTeamDetail) => {
      if (groupTeamDetail) {
        this.groupTeam = groupTeamDetail;
        this.rows = this.groupTeam.teamAttendances;

        this.teamUpdateForm = new UntypedFormGroup({
          teamName: new UntypedFormControl(this.groupTeam.teamName, [
            Validators.required,
            Validators.pattern(this._teamNamePattern)
          ]),
          teamBio: new UntypedFormControl(this.groupTeam.teamBio, [])
        });

        this._isReady = true;
      }
    });

    const parentRouteParamsSubscription = this.router.parent.params.subscribe(params => {
      this.groupConductor.initTeams(params['id']);
      this.groupConductor.initImages(params['id']);
    });

    const routeParamsSubscription = this.router.params.subscribe(params => {
      this.groupConductor.initFocussedGroupTeam(params['teamId']);
    });

    this._subscriptions.push(parentRouteParamsSubscription, routeParamsSubscription);
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  onUpdateTeamSubmit() {

    if (this.teamUpdateForm.valid) {

      // Update the visible entities
      this.groupTeam.teamName = this.teamUpdateForm.value.teamName;
      this.groupTeam.teamBio = this.teamUpdateForm.value.teamBio;

      const groupTeamUpdate: GroupTeamUpdateDto = {
        teamId: this.groupTeam.teamId,
        teamName: this.teamUpdateForm.value.teamName,
        teamBio: this.teamUpdateForm.value.teamBio
      };

      this.groupConductor.updateTeam(groupTeamUpdate);
    }
  }

  onSetTeamPicture(groupImage: GroupImage) {

    const teamImageSubmission: TeamImageSubmissionDto = {
      imageId: groupImage.imageId,
      teamId: this.groupTeam.teamId
    };
    this.groupConductor.attachImageToTeam(teamImageSubmission);
  }

  onGroupTeamRemoveClick() {
    this.groupConductor.removeTeam(this.groupTeam);
    this._location.back();
  }
}

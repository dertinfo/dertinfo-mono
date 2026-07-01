import { Injectable } from '@angular/core';
import { ScoreCard } from 'app/models/app/ScoreCard';

import { DanceDetailDto, GroupDto, GroupRegistrationDto, TeamAttendanceDto } from 'app/models/dto';
import { DodGroupResultsDto } from 'app/models/dto/DodGroupResultsDto';
import { DodResultComplaintDto } from 'app/models/dto/DodResultComplaintDto';
import { DodResultComplaintSubmissionDto } from 'app/models/dto/DodResultComplaintSubmissionDto';
import { ResultsAuthRepository } from '../../repositories/repositories/resultsauth.repository';
import { GroupViewRepository } from './group-view.repository';
import { GroupViewTracker } from './group-view.tracker';

@Injectable()
export class GroupViewConductor {

  constructor(

    private _tracker: GroupViewTracker,
    private _groupRepo: GroupViewRepository,
    private _resultsAuthRepo: ResultsAuthRepository
  ) {

    this._tracker.reset();
  }

  public clearState() {

    this._tracker.reset();
  }

  public initGroup(groupId: number, force: boolean = false) {

    if (!this._tracker.hasLoadedGroup() || force) {
      const subs = this._groupRepo.get(this._tracker.groupId).subscribe((group) => {
        subs.unsubscribe();
        this._tracker.group = group;
      });
    }
  }

  public applyGroup(groupDto: GroupDto) {

    this._tracker.reset();
    this._tracker.group = groupDto;
  }

  public applyRegistrations(registrations: GroupRegistrationDto[]) {

    this._tracker.registrations = registrations;
  }

  public applyTeamAttendances(registrationId: number, teamAttendances: TeamAttendanceDto[]) {

    this._tracker.addTeamAttendances(registrationId, teamAttendances, true);
  }

  public initFocussedRegistration(registrationId: number) {

    if (!this._tracker.hasRegistrationDetail(registrationId)) {

      // note - at this time we do not need to load detail seperately.

      // const subs = this.groupRepo.registrationDetail(registrationId).subscribe((registrationDetail) => {
      //     subs.unsubscribe();
      //     this._tracker.addRegistrationDetail(registrationDetail, true);
      // });
    } else {

      this._tracker.focusRegistrationDetail(registrationId);
    }
  }

  initResults(teamId: number, activityId: number): any {

    if (!this._tracker.hasLoadedTeamActivityResults(teamId, activityId)) {
      const subs = this._resultsAuthRepo.getResultsByCompetitionAndTeam(activityId, teamId).subscribe((results: Array<DanceDetailDto>) => {
        subs.unsubscribe();
        this._tracker.addTeamActivityResults(teamId, activityId, results, true);
      });
    } else {
      this._tracker.focusTeamActivityResults(teamId, activityId);
    }
  }

  initDodResults(groupId: number, force: boolean = false) {
    if (!this._tracker.hasLoadedDodGroupResults() || force) {
      const subs = this._groupRepo.getDodResults(groupId).subscribe((results: DodGroupResultsDto) => {
        subs.unsubscribe();
        this._tracker.dodResults = results;
      });
    }
  }

  public reportScoreCard(groupId: number, submission: DodResultComplaintSubmissionDto) {

    const obs$ = this._groupRepo.raiseResultComplaint(groupId, submission);

    const subs = obs$.subscribe((succeeded: boolean) => {
      subs.unsubscribe();
      this._tracker.removeDodResult(submission.resultId);
    });

    return obs$;
  }

}

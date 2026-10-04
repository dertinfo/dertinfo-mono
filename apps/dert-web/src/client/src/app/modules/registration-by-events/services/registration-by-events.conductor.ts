
// Angualr and RxJS
import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from 'app/core/authentication/auth.service';
import { RegistrationFlowState } from 'app/models/app/Enumerations/RegistrationFlowStates';
import {
    ActivityDto,
    ActivityMemberAttendanceDto,
    ActivityTeamAttendanceDto,
    EventRegistrationOverviewDto,
    GroupMemberDto,
    GroupMemberSubmissionDto,
    GroupTeamDto,
    GroupTeamSubmissionDto,
    MemberAttendanceDto,
    RegistrationConfirmSubmissionDto,
    RegistrationMemberAttendanceAttachActivitiesSubmissionDto,
    RegistrationMemberAttendanceDeleteSubmissionDto,
    RegistrationMemberAttendanceDetachActivitiesSubmissionDto,
    RegistrationMemberAttendanceSubmissionDto,
    RegistrationSubmitSubmissionDto,
    RegistrationTeamAttendanceAttachActivitiesSubmissionDto,
    RegistrationTeamAttendanceDeleteSubmissionDto,
    RegistrationTeamAttendanceDetachActivitiesSubmissionDto,
    RegistrationTeamAttendanceSubmissionDto,
    TeamAttendanceDto
} from 'app/models/dto';
import { RegistrationRepository } from 'app/modules/repositories';
import { EventRepository } from 'app/modules/repositories';
import { forkJoin ,  Observable } from 'rxjs';
import {map, mergeMap} from 'rxjs/operators';
import { RegistrationByEventsRepository } from './registration-by-events.repository';
import { RegistrationByEventsTracker } from './registration-by-events.tracker';

@Injectable()
export class RegistrationByEventsConductor {

    private _snackBarDuration = 1200;

    get capabilities() {
        return this._tracker.capabilities;
    }

    constructor(
        public authService: AuthService,
        private _tracker: RegistrationByEventsTracker,
        private emailRepo: RegistrationByEventsRepository,
        private eventRepo: EventRepository,
        private registationRepo: RegistrationRepository,
        private snackBar: MatSnackBar,

    ) {

        this._tracker.reset();
    }

    public setOverview(eventRegistrationOverviewDto: EventRegistrationOverviewDto) {

        if (this._tracker.overview && this._tracker.overview.id !== eventRegistrationOverviewDto.id) {
            this._tracker.reset();
        }

        this._tracker.overview = {
            ...eventRegistrationOverviewDto,
            isDeleting: false,
            isDeleted: false,
            flowStateText: 'todo',
            flowStateColour: '#CCC'
        };
    }

    public initContactInfo() {

        if (!this._tracker.hasLoadedContactInfo()) {
            const subs = this.registationRepo.getGroupContactInfo(this._tracker.registrationId).subscribe((contactInfo) => {
                subs.unsubscribe();
                this._tracker.contactInfo = contactInfo;
            });
        }
    }

    /**
     * [Init]
     * Loads the data for all members on the group relating to the registration.
     * @param registrationId - the registration in question
     */
    public initAttendingIndividuals() {

        if (!this._tracker.hasLoadedMemberAttendances()) {
            const subs = this.registationRepo.getAttendingIndividuals(this._tracker.registrationId).subscribe((memberAttendanceDtos) => {
                subs.unsubscribe();
                this._tracker.memberAttendances = memberAttendanceDtos;
            });
        }
    }

    /**
     * [Init]
     * Loads the data for the attending teams.
     * @param registrationId - the registration in question
     */
    public initAttendingTeams() {

        if (!this._tracker.hasLoadedTeamAttendances()) {
            const subs = this.registationRepo.getAttendingTeams(this._tracker.registrationId).subscribe((memberAttendanceDtos) => {
                subs.unsubscribe();
                this._tracker.teamAttendances = memberAttendanceDtos;
            });
        }
    }

    /**
     * [Init]
     * Loads the data for the individual activities
     */
    public initIndividualActivities() {

        if (!this._tracker.hasLoadedMemberActivities()) {
            const subs = this.registationRepo.getIndividualActivities(this._tracker.registrationId).subscribe((activityDtos) => {
                subs.unsubscribe();
                this._tracker.memberActivities = activityDtos;
            });
        }
    }

    /**
     * [Init]
     * Loads the data for the team activities.
     */
    public initTeamActivities() {

        if (!this._tracker.hasLoadedTeamActivities()) {
            const subs = this.registationRepo.getTeamActivities(this._tracker.registrationId).subscribe((activityDtos) => {
                subs.unsubscribe();
                this._tracker.teamActivities = activityDtos;
            });
        }
    }

    public getConfirmationEmailPreview(): Observable<string> {

        const getData$ = this.registationRepo.getConfirmationEmailData(this._tracker.registrationId);

        return getData$.pipe(mergeMap(data => {
            return this.emailRepo.previewRegistrationConfirmation(data);
        }));
    }

    /**
     * [Public]
     * Exposed capability to clear state from the conductor
     */
    public clear() {

        this._tracker.reset();
    }

    public confirmRegistration() {

        // Create the submission to create the member and attendance
        const registationConfirmSubmissionDto: RegistrationConfirmSubmissionDto = {};

        // Submit to the api.
        const confirmSubmission$ = this.registationRepo.confirmRegistration(
            this._tracker.registrationId,
            registationConfirmSubmissionDto
        );

        // Subscribe
        const subs = confirmSubmission$.subscribe(() => {
            subs.unsubscribe();

            const overview = this._tracker.overview;
            overview.flowState = RegistrationFlowState.confirmed;
            this._tracker.overview = overview;

            this.snackBar.open('Registration Confirmed', 'close', { duration: this._snackBarDuration });
        });
    }

    /**
     * [Public]
     * Exposed method for creating a new attendance with a new member
     * @param groupMemberSubmission - standard group member create submission
     */
    public addMember(groupMemberSubmission: GroupMemberSubmissionDto) {

        // Create the submission to create the member and attendance
        const addMemberAttendanceSubmissionDto: RegistrationMemberAttendanceSubmissionDto = {
            memberAttendanceId: 0, // no attendance at this time
            groupMemberId: 0, // member does not exisit
            groupMemberSubmission: groupMemberSubmission // member to create
        };

        // Submit to the api.
        const createMemberAndAttenadance$ = this.registationRepo.addMemberAttendance(
            this._tracker.registrationId,
            addMemberAttendanceSubmissionDto
        );

        const subs = createMemberAndAttenadance$.subscribe((memberAttendanceDto) => {
            subs.unsubscribe();
            this._tracker.addNewMemberAndAttendance(memberAttendanceDto);
        });

        return createMemberAndAttenadance$;
    }

    /**
     * [Public]
     * Exposed method for creating a new attendance with a new team
     * @param teamSubmission - standard group member create submission
     */
    public addTeam(groupTeamSubmission: GroupTeamSubmissionDto) {

        // Create the submission to create the member and attendance
        const addTeamAttendanceSubmissionDto: RegistrationTeamAttendanceSubmissionDto = {
            teamAttendanceId: 0, // no attendance at this time
            teamId: 0, // member does not exisit
            groupTeamSubmission: groupTeamSubmission // member to create
        };

        // Submit to the api.
        const createTeamAndAttenadance$ = this.registationRepo.addTeamAttendance(
            this._tracker.registrationId,
            addTeamAttendanceSubmissionDto
        );

        const subs = createTeamAndAttenadance$.subscribe((teamAttendanceDto) => {
            subs.unsubscribe();
            this._tracker.addNewTeamAndAttendance(teamAttendanceDto);
        });

        return createTeamAndAttenadance$;
    }

    /**
     * [Public]
     * Based on a number of edited attendances identifies is they are creations or deletions
     * @param editedGroupMembers - members to which the attendance state has changed
     */
    public toggleMemberAttendances(editedGroupMembers: GroupMemberDto[]): Observable<MemberAttendanceDto[]> {

        const allDone$: Observable<any>[] = [];

        const changeReport = this._tracker.identifyMemberAttendanceChanges(editedGroupMembers);

        const obs1 = this.removeMemberAttendances(changeReport.deletes);
        const obs2 = this.addMemberAttendances(changeReport.adds.map((gm) => {
            const memberAttendnaceSubmission: RegistrationMemberAttendanceSubmissionDto = {
                groupMemberId: gm.groupMemberId,
                memberAttendanceId: 0 // none
            };

            return memberAttendnaceSubmission;
        }));

        if (obs1 !== null) { allDone$.push(obs1); }
        if (obs2 !== null) { allDone$.push(obs2); }

        const obsAll = forkJoin(allDone$);
        obsAll.subscribe(result => {
            this.snackBar.open('Attending Members Updated', 'close', { duration: this._snackBarDuration });
        });

        return obsAll;
    }

    /**
     * [Public]
     * Based on a number of edited attendances identifies is they are creations or deletions
     * @param editedGroupTeams - members to which the attendance state has changed
     */
    public toggleTeamAttendances(editedGroupTeams: GroupTeamDto[]): Observable<TeamAttendanceDto[]> {

        const allDone$: Observable<any>[] = [];

        const changeReport = this._tracker.identifyTeamAttendanceChanges(editedGroupTeams);

        const obs1 = this.removeTeamAttendances(changeReport.deletes);
        const obs2 = this.addTeamAttendances(changeReport.adds.map((gt) => {
            const teamAttendnaceSubmission: RegistrationTeamAttendanceSubmissionDto = {
                teamId: gt.teamId,
                teamAttendanceId: 0 // none
            };

            return teamAttendnaceSubmission;
        }));

        if (obs1 !== null) { allDone$.push(obs1); }
        if (obs2 !== null) { allDone$.push(obs2); }

        const obsAll = forkJoin(allDone$);
        obsAll.subscribe(result => {
            this.snackBar.open('Attending Teams Updated', 'close', { duration: this._snackBarDuration });
        });

        return obsAll;
    }

    /**
     * [PUBLIC]
     * Creates direct reference to the memberAttendance we are insterested in. By assigning them to the focussedMember
     * @param memberAttendanceId - the id of the memberAttendance to focus
     */
    public focusMemberAttendance(memberAttendanceId: number) {
        this._tracker.focusMemberAttendance(memberAttendanceId);
    }

    /**
     * [PUBLIC]
     * Creates direct reference to the teamAttendance we are insterested in. By assigning them to the focussedTeam
     * @param memberAttendanceId - the id of the teamAttendance to focus
     */
    public focusTeamAttendance(teamAttendanceId: number) {
        this._tracker.focusTeamAttendance(teamAttendanceId);
    }

    /**
     * [Public]
     * Accepts a number of changed ActivityDtos and identifies whether they are additions or deletions for a member.
     * Routes additions and deletions to other methods
     * @param edited
     */
    public toggleFocussedMemberAttendanceActivities(edited: ActivityDto[]): Observable<ActivityMemberAttendanceDto[]> {

        const allDone$: Observable<any>[] = [];

        const changeReport = this._tracker.identifyMemberAttendanceActivityChanges(edited);

        const obs1 = this.removeMemberAttendanceActivities(changeReport.deletes);
        const obs2 = this.addMemberAttendanceActivities(changeReport.adds);

        if (obs1 !== null) { allDone$.push(obs1); }
        if (obs2 !== null) { allDone$.push(obs2); }

        const obsAll = forkJoin(allDone$);
        obsAll.subscribe(result => {
            this._tracker.focusMemberAttendance(null);
            this.snackBar.open('Activities Updated', 'close', { duration: this._snackBarDuration });
        });

        return obsAll;
    }

    /**
     * [Public]
     * Accepts a number of changed ActivityDtos and identifies whether they are additions or deletions for a team.
     * Routes additions and deletions to other methods
     * @param edited
     */
    public toggleFocussedTeamAttendanceActivities(edited: ActivityDto[]): Observable<ActivityTeamAttendanceDto[]> {

        const allDone$: Observable<any>[] = [];

        const changeReport = this._tracker.identifyTeamAttendanceActivityChanges(edited);

        const obs1 = this.removeTeamAttendanceActivities(changeReport.deletes);
        const obs2 = this.addTeamAttendanceActivities(changeReport.adds);

        if (obs1 !== null) { allDone$.push(obs1); }
        if (obs2 !== null) { allDone$.push(obs2); }

        const obsAll = forkJoin(allDone$);
        obsAll.subscribe(result => {
            this._tracker.focusTeamAttendance(null);
            this.snackBar.open('Activities Updated', 'close', { duration: this._snackBarDuration });
        });

        return obsAll;
    }

    /**
     * [Public]
     * Remove a members attendance by the memberId
     * @param memberId - the Id of the member to remove
     */
    public removeAttendanceByMember(memberId: number) {

        const memberAttendance = this._tracker.findMemberAttendanceByMemberId(memberId);
        const subs = this.registationRepo.removeMemberAttendance(this._tracker.registrationId, memberAttendance.id).subscribe(ma => {
            subs.unsubscribe();
            this._tracker.removeMemberAttendance(memberAttendance);

            this.getUpdatedPrice();
        });
    }

    /**
    * [Public]
    * Remove a team attendance by the teamId
    * @param teamId - the Id of the member to remove
    */
    public removeAttendanceByTeam(teamId: number) {
        const teamAttendance = this._tracker.findTeamAttendanceByTeamId(teamId);
        const subs = this.registationRepo.removeTeamAttendance(this._tracker.registrationId, teamAttendance.id).subscribe(ma => {
            subs.unsubscribe();
            this._tracker.removeTeamAttendance(teamAttendance);

            this.getUpdatedPrice();
        });
    }

    /**
     * [External State]
     * Submits to the api those activities that are to be removed from the focussed memberAttendance
     * @param activityIds - the ids of the activities to remove
     */
    private removeMemberAttendanceActivities(activityMemberAttendanceIds: number[]) {

        if (activityMemberAttendanceIds.length === 0) { return null; }

        // Create the submission to create the member and attendance
        const removeMemberAttendanceSubmissionDto: RegistrationMemberAttendanceDetachActivitiesSubmissionDto = {
            activityMemberAttendanceIds: activityMemberAttendanceIds
        };

        const removeActivities$ = this.registationRepo.batchRemoveMemberAttendanceActivities(
            this._tracker.registrationId,
            this._tracker.focussedMemberAttendanceId,
            removeMemberAttendanceSubmissionDto
        );

        const subs = removeActivities$.subscribe(activityMemberAttendances => {
            subs.unsubscribe();
            activityMemberAttendances.forEach(detached => {
                this._tracker.removeActivityFromMember(detached);

                this.getUpdatedPrice();
            });
        });

        return removeActivities$;
    }

    /**
     * [External State]
     * Submits to the api those activities that are to be removed from the focussed teamAttendance
     * @param activityIds - the ids of the activities to remove
     */
    private removeTeamAttendanceActivities(activityTeamAttendanceIds: number[]) {

        if (activityTeamAttendanceIds.length === 0) { return null; }

        // Create the submission to create the member and attendance
        const removeTeamAttendanceSubmissionDto: RegistrationTeamAttendanceDetachActivitiesSubmissionDto = {
            activityTeamAttendanceIds: activityTeamAttendanceIds
        };

        const removeActivities$ = this.registationRepo.batchRemoveTeamAttendanceActivities(
            this._tracker.registrationId,
            this._tracker.focussedTeamAttendanceId,
            removeTeamAttendanceSubmissionDto
        );

        const subs = removeActivities$.subscribe(activityTeamAttendances => {
            subs.unsubscribe();
            activityTeamAttendances.forEach(detached => {
                this._tracker.removeActivityFromTeam(detached);

                this.getUpdatedPrice();
            });
        });

        return removeActivities$;
    }

    /**
     * [External State]
     * Submits to the api those activities that are to be added to the focussed memberAttendance
     * @param activityIds - the ids of the activities to attach
     */
    private addMemberAttendanceActivities(activityIds: number[]) {

        if (activityIds.length === 0) { return null; }

        // Create the submission to create the member and attendance
        const addMemberAttendanceSubmissionDto: RegistrationMemberAttendanceAttachActivitiesSubmissionDto = {
            activityIds: activityIds
        };

        const addActivities$ = this.registationRepo.batchAddMemberAttendanceActivities(
            this._tracker.registrationId,
            this._tracker.focussedMemberAttendanceId,
            addMemberAttendanceSubmissionDto
        );

        const subs = addActivities$.subscribe(activityMemberAttendances => {
            subs.unsubscribe();
            activityMemberAttendances.forEach(attached => {
                this._tracker.addActivityToMember(attached);

                this.getUpdatedPrice();
            });
        });

        return addActivities$;
    }

    /**
     * [External State]
     * Submits to the api those activities that are to be added to the focussed teamAttendance
     * @param activityIds - the ids of the activities to attach
     */
    private addTeamAttendanceActivities(activityIds: number[]) {

        if (activityIds.length === 0) { return null; }

        // Create the submission to create the team and attendance
        const addTeamAttendanceSubmissionDto: RegistrationTeamAttendanceAttachActivitiesSubmissionDto = {
            activityIds: activityIds
        };

        const addActivities$ = this.registationRepo.batchAddTeamAttendanceActivities(
            this._tracker.registrationId,
            this._tracker.focussedTeamAttendanceId,
            addTeamAttendanceSubmissionDto
        );

        const subs = addActivities$.subscribe(activityTeamAttendances => {
            subs.unsubscribe();
            activityTeamAttendances.forEach(attached => {
                this._tracker.addActivityToTeam(attached);

                this.getUpdatedPrice();
            });
        });

        return addActivities$;
    }

    /**
     * Submits a batch of member attendances to be created at the api.
     * @param submissions - an array of submissions to be sent together.
     */
    private addMemberAttendances(submissions: RegistrationMemberAttendanceSubmissionDto[]): Observable<MemberAttendanceDto[]> {

        const createObs = this.registationRepo.batchAddMemberAttendances(this._tracker.registrationId, submissions);

        const createSub = createObs.subscribe((responses: MemberAttendanceDto[]) => {
            createSub.unsubscribe();
            this._tracker.addNewMemberAttendances(responses);

            this.getUpdatedPrice();
        });

        return createObs;
    }

    /**
     * Submits a batch of member attendances to be created at the api.
     * @param submissions - an array of submissions to be sent together.
     */
    private addTeamAttendances(submissions: RegistrationTeamAttendanceSubmissionDto[]): Observable<TeamAttendanceDto[]> {

        const createObs = this.registationRepo.batchAddTeamAttendances(this._tracker.registrationId, submissions);

        const createSub = createObs.subscribe((responses: TeamAttendanceDto[]) => {
            createSub.unsubscribe();
            this._tracker.addNewTeamAttendances(responses);

            this.getUpdatedPrice();
        });

        return createObs;
    }

    /**
     * [External State]
     * Submits a delete multiple member attendances request to the repository.
     * @param memberAttendances - the member attendances to remove
     */
    private removeMemberAttendances(memberAttendances: MemberAttendanceDto[]): Observable<MemberAttendanceDto[]> {

        const deleteBatchSubmission: RegistrationMemberAttendanceDeleteSubmissionDto = {
            memberAttendanceIds: memberAttendances.map((ma) => ma.id)
        };

        const deleteObs = this.registationRepo.removeMemberAttendances(this._tracker.registrationId, deleteBatchSubmission);

        const sub = deleteObs.subscribe((deletedMemberAttendances: MemberAttendanceDto[]) => {
            sub.unsubscribe();
            deletedMemberAttendances.forEach(deletedMemberAttendance => {
                this._tracker.removeMemberAttendance(deletedMemberAttendance);
            });

            this.getUpdatedPrice();
        });

        return deleteObs;
    }

    /**
    * [External State]
    * Submits a delete multiple team attendances request to the repository.
    * @param teamAttendances - the team attendances to remove
    */
    private removeTeamAttendances(teamAttendances: TeamAttendanceDto[]): Observable<TeamAttendanceDto[]> {

        const deleteBatchSubmission: RegistrationTeamAttendanceDeleteSubmissionDto = {
            teamAttendanceIds: teamAttendances.map((ma) => ma.id)
        };

        const deleteObs = this.registationRepo.removeTeamAttendances(this._tracker.registrationId, deleteBatchSubmission);

        const sub = deleteObs.subscribe((deletedTeamAttendances: TeamAttendanceDto[]) => {
            sub.unsubscribe();
            deletedTeamAttendances.forEach(deletedTeamAttendance => {
                this._tracker.removeTeamAttendance(deletedTeamAttendance);
            });

            this.getUpdatedPrice();
        });

        return deleteObs;
    }

    private getUpdatedPrice() {
        const subs = this.registationRepo.getUpdatedPriceForRegistration(this._tracker.registrationId).subscribe((newPrice: number) => {
            subs.unsubscribe();

            this._tracker.overview.currentTotal = newPrice;
            this.setOverview(this._tracker.overview);
        });
    }
}

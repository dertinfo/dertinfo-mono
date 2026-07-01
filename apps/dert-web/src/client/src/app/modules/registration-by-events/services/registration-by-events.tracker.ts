
import { Injectable } from '@angular/core';
import { EventRegistrationOverview } from 'app/models/app';
import { MemberType } from 'app/models/app/Enumerations/MemberType';
import { RegistrationFlowState } from 'app/models/app/Enumerations/RegistrationFlowStates';
import {
    ActivityDto,
    ActivityMemberAttendanceDto,
    ActivityTeamAttendanceDto,
    ContactInfoDto,
    GroupMemberDto,
    GroupTeamDto,
    MemberAttendanceDto,
    TeamAttendanceDto
} from 'app/models/dto';
import { BehaviorSubject } from 'rxjs';
import {map} from 'rxjs/operators';

@Injectable()
export class RegistrationByEventsTracker {

    get registrationOverview$() { return this._registrationOverview.asObservable(); }
    get allGroupMembers$() { return this._allGroupMembers.asObservable(); }
    get allGroupTeams$() { return this._allGroupTeams.asObservable(); }
    get memberAttendances$() { return this._memberAttendances.asObservable(); }
    get teamAttendances$() { return this._teamAttendances.asObservable(); }
    get activitiesForIndividuals$() { return this._activitiesForIndividuals.asObservable(); }
    get activitiesForTeams$() { return this._activitiesForTeams.asObservable(); }
    get focussedMemberAttendance$() { return this._focussedMemberAttendance.asObservable(); }
    get focussedTeamAttendance$() { return this._focussedTeamAttendance.asObservable(); }
    get contactInfo$() { return this._contactInfo.asObservable(); }
    get attendingGroupMembers$() {
        return this._allGroupMembers.pipe(
            map(agm => agm
                .filter(
                    gm => { return this._memoryStore.memberAttendances.find(ma => ma.groupMemberId === gm.groupMemberId); }
                )
            ));
    }
    get attendingGroupTeams$() {
        return this._allGroupTeams.pipe(
            map(agt => agt
                .filter(
                    gt => { return this._memoryStore.teamAttendances.find(ta => ta.teamId === gt.teamId); }
                )
            ));
    }

    // #region Properties Getters

    public get registrationId(): number {
        return this._memoryStore.registrationOverview.id;
    }

    public get focussedMemberAttendanceId(): number {
        return this._memoryStore.focussedMemberAttendance.id;
    }

    public get focussedTeamAttendanceId(): number {
        return this._memoryStore.focussedTeamAttendance.id;
    }

    public get overview(): EventRegistrationOverview {
        return this._memoryStore.registrationOverview;
    }

    public get capabilities() {
        return this._capabilityStore;
    }

    // #endregion

    // #region Properties Setters

    public set overview(overview: EventRegistrationOverview) {
        this._memoryStore.registrationOverview = overview;
        this._registrationOverview.next(this._memoryStore.registrationOverview);

        this.setCapabilities(overview.flowState);
    }

    public set memberAttendances(memberAttendances: MemberAttendanceDto[]) {
        this._memoryStore.memberAttendances = memberAttendances;
        this._memberAttendances.next(this._memoryStore.memberAttendances);
    }

    public set teamAttendances(teamAttendances: TeamAttendanceDto[]) {
        this._memoryStore.teamAttendances = teamAttendances;
        this._teamAttendances.next(this._memoryStore.teamAttendances);
    }

    public set memberActivities(memberActivities: ActivityDto[]) {
        this._memoryStore.activitiesForIndividuals = memberActivities;
        this._activitiesForIndividuals.next(this._memoryStore.activitiesForIndividuals);
    }

    public set teamActivities(teamActivities: ActivityDto[]) {
        this._memoryStore.activitiesForTeams = teamActivities;
        this._activitiesForTeams.next(this._memoryStore.activitiesForTeams);
    }

    public set contactInfo(contactInfo: ContactInfoDto) {
        this._memoryStore.contactInfo = contactInfo;
        this._contactInfo.next(this._memoryStore.contactInfo);
    }

    // #region Observables and Subjects

    private _registrationOverview = new BehaviorSubject<EventRegistrationOverview>(null);
    private _allGroupMembers = new BehaviorSubject<GroupMemberDto[]>([]);
    private _allGroupTeams = new BehaviorSubject<GroupTeamDto[]>([]);
    private _memberAttendances = new BehaviorSubject<MemberAttendanceDto[]>([]);
    private _teamAttendances = new BehaviorSubject<TeamAttendanceDto[]>([]);
    private _activitiesForIndividuals = new BehaviorSubject<ActivityDto[]>([]);
    private _activitiesForTeams = new BehaviorSubject<ActivityDto[]>([]);
    private _focussedMemberAttendance = new BehaviorSubject<MemberAttendanceDto>(null);
    private _focussedTeamAttendance = new BehaviorSubject<TeamAttendanceDto>(null);
    private _contactInfo = new BehaviorSubject<ContactInfoDto>(null);

    // #endregion

    private _memoryStore: {
        registrationOverview: EventRegistrationOverview,
        memberAttendances: MemberAttendanceDto[],
        teamAttendances: TeamAttendanceDto[],
        activitiesForIndividuals: ActivityDto[],
        activitiesForTeams: ActivityDto[],
        focussedMemberAttendance: MemberAttendanceDto,
        focussedTeamAttendance: TeamAttendanceDto,
        contactInfo: ContactInfoDto
    };

    private _capabilityStore = {
        canAddEditMembers: false,
        canAddEditTeams: false,
        canDeleteMembers: false,
        canDeleteTeams: false,
        canConfirm: false
    };

    // #endregion

    // #region Properties hasLoaded()

    public hasLoadedOverview(): boolean {
        return this._memoryStore.registrationOverview !== null;
    }

    public hasLoadedMemberActivities(): boolean {
        return this._memoryStore.activitiesForIndividuals !== null;
    }

    public hasLoadedTeamActivities(): boolean {
        return this._memoryStore.activitiesForTeams !== null;
    }

    public hasLoadedMemberAttendances(): boolean {
        return this._memoryStore.memberAttendances !== null;
    }

    public hasLoadedTeamAttendances(): boolean {
        return this._memoryStore.teamAttendances !== null;
    }

    public hasLoadedContactInfo(): boolean {
        return this._memoryStore.contactInfo !== null;
    }

    // #endregion

    // #region MemberAttendance

    public findMemberAttendanceByMemberId(memberId: number): any {
        const found = this._memoryStore.memberAttendances.find(ma => ma.groupMemberId === memberId);
        return found;
    }

    public addNewMemberAndAttendance(memberAttendanceDto: MemberAttendanceDto) {

        const groupMember: GroupMemberDto = {
            groupId: this._memoryStore.registrationOverview.groupId,
            groupMemberId: memberAttendanceDto.groupMemberId,
            name: memberAttendanceDto.groupMemberName,
            memberType: memberAttendanceDto.groupMemberType,
            memberAttendances: [memberAttendanceDto]
        };

        this._memoryStore.memberAttendances.push(memberAttendanceDto);

        this.updateOverviewCounts();

        this._registrationOverview.next(this._memoryStore.registrationOverview);
        this._memberAttendances.next(this._memoryStore.memberAttendances);
    }

    public addNewMemberAttendances(memberAttendanceDtos: MemberAttendanceDto[]) {
        this._memoryStore.memberAttendances = this._memoryStore.memberAttendances.concat(memberAttendanceDtos);

        this.updateOverviewCounts();

        this._registrationOverview.next(this._memoryStore.registrationOverview);
        this._memberAttendances.next(this._memoryStore.memberAttendances);
    }

    public removeMemberAttendance(memberAttendanceDto: MemberAttendanceDto) {
        if (!memberAttendanceDto) {
            throw new Error('GroupRegistrationTracker - removeMemberAttendance - memberAttendance is null');
        }

        const found = this._memoryStore.memberAttendances.find(ta => ta.id === memberAttendanceDto.id);
        const index = this._memoryStore.memberAttendances.indexOf(found);
        this._memoryStore.memberAttendances.splice(index, 1);

        this.updateOverviewCounts();

        this._registrationOverview.next(this._memoryStore.registrationOverview);
        this._memberAttendances.next(this._memoryStore.memberAttendances);
    }

    public removeMemberAttendanceByMemberId(memberId: number) {
        const found = this._memoryStore.memberAttendances.find(ma => ma.groupMemberId === memberId);
        const index = this._memoryStore.memberAttendances.indexOf(found);
        this._memoryStore.memberAttendances.splice(index, 1);

        this.updateOverviewCounts();

        this._registrationOverview.next(this._memoryStore.registrationOverview);
        this._memberAttendances.next(this._memoryStore.memberAttendances);
    }

    public focusMemberAttendance(memberAttendanceId: number) {

        if (memberAttendanceId) {
            this._memoryStore.focussedMemberAttendance = this._memoryStore.memberAttendances.find(ma => ma.id === memberAttendanceId);
        } else {
            this._memoryStore.focussedMemberAttendance = null;
        }

        this._focussedMemberAttendance.next(this._memoryStore.focussedMemberAttendance);
    }

    public identifyMemberAttendanceChanges(changes: GroupMemberDto[]) {
        const adds: GroupMemberDto[] = [];
        const deletes: MemberAttendanceDto[] = [];

        changes.forEach(gm => {
            const memberAttendance = this._memoryStore.memberAttendances.find(ma => ma.groupMemberId === gm.groupMemberId);

            if (memberAttendance) {
                deletes.push(memberAttendance);
            } else {
                adds.push(gm);
            }
        });

        return { adds, deletes };
    }

    // #endregion

    // #region TeamAttendance

    public findTeamAttendanceByTeamId(teamId: number): any {
        const found = this._memoryStore.teamAttendances.find(ta => ta.teamId === teamId);
        return found;
    }

    public addNewTeamAndAttendance(teamAttendanceDto: TeamAttendanceDto) {

        const groupTeam: GroupTeamDto = {
            groupId: this._memoryStore.registrationOverview.groupId,
            teamId: teamAttendanceDto.teamId,
            teamName: teamAttendanceDto.groupTeamName,
            teamAttendances: [teamAttendanceDto]
        };

        this._memoryStore.teamAttendances.push(teamAttendanceDto);

        this.updateOverviewCounts();

        this._registrationOverview.next(this._memoryStore.registrationOverview);
        this._teamAttendances.next(this._memoryStore.teamAttendances);

    }

    public addNewTeamAttendances(teamAttendanceDtos: TeamAttendanceDto[]) {
        this._memoryStore.teamAttendances = this._memoryStore.teamAttendances.concat(teamAttendanceDtos);

        this.updateOverviewCounts();

        this._registrationOverview.next(this._memoryStore.registrationOverview);
        this._teamAttendances.next(this._memoryStore.teamAttendances);
    }

    public removeTeamAttendance(teamAttendanceDto: TeamAttendanceDto) {
        if (!teamAttendanceDto) {
            throw new Error('GroupRegistrationTracker - removeMemberAttendance - memberAttendance is null');
        }

        const found = this._memoryStore.teamAttendances.find(ta => ta.id === teamAttendanceDto.id);
        const index = this._memoryStore.teamAttendances.indexOf(found);
        this._memoryStore.teamAttendances.splice(index, 1);

        this.updateOverviewCounts();

        this._registrationOverview.next(this._memoryStore.registrationOverview);
        this._teamAttendances.next(this._memoryStore.teamAttendances);
    }

    public removeTeamAttendanceByTeamId(teamId: number) {
        const found = this._memoryStore.teamAttendances.find(ta => ta.teamId === teamId);
        const index = this._memoryStore.teamAttendances.indexOf(found);
        this._memoryStore.teamAttendances.splice(index, 1);

        this.updateOverviewCounts();

        this._registrationOverview.next(this._memoryStore.registrationOverview);
        this._teamAttendances.next(this._memoryStore.teamAttendances);
    }

    public focusTeamAttendance(teamAttendanceId: number) {

        if (teamAttendanceId) {
            this._memoryStore.focussedTeamAttendance = this._memoryStore.teamAttendances.find(ta => ta.id === teamAttendanceId);
        } else {
            this._memoryStore.focussedTeamAttendance = null;
        }

        this._focussedTeamAttendance.next(this._memoryStore.focussedTeamAttendance);
    }

    public identifyTeamAttendanceChanges(changes: GroupTeamDto[]) {
        const adds: GroupTeamDto[] = [];
        const deletes: TeamAttendanceDto[] = [];

        changes.forEach(gt => {
            const teamAttendance = this._memoryStore.teamAttendances.find(ma => ma.teamId === gt.teamId);

            if (teamAttendance) {
                deletes.push(teamAttendance);
            } else {
                adds.push(gt);
            }
        });

        return { adds, deletes };
    }

    // #endregion

    // #region MemberActivities

    public removeActivityFromMember(activityMemberAttendance: ActivityMemberAttendanceDto) {
        const found = this._memoryStore.focussedMemberAttendance.attendanceActivities.find(aa => aa.id === activityMemberAttendance.id);
        const index = this._memoryStore.focussedMemberAttendance.attendanceActivities.indexOf(found);
        this._memoryStore.focussedMemberAttendance.attendanceActivities.splice(index, 1);

        this._focussedMemberAttendance.next(this._memoryStore.focussedMemberAttendance);
    }

    public addActivityToMember(attendanceActivity: ActivityMemberAttendanceDto) {
        this._memoryStore.focussedMemberAttendance.attendanceActivities.push(attendanceActivity);

        this._focussedMemberAttendance.next(this._memoryStore.focussedMemberAttendance);
    }

    public identifyMemberAttendanceActivityChanges(changes: ActivityDto[]) {
        const adds: number[] = [];
        const deletes: number[] = [];

        const focussedMemberAttendanceActivities = this._memoryStore.focussedMemberAttendance.attendanceActivities;
        changes.forEach(a => {
            const activityMemberAttendance = focussedMemberAttendanceActivities.find(aa => aa.activityId === a.id);

            if (activityMemberAttendance) {
                deletes.push(activityMemberAttendance.id);
            } else {
                adds.push(a.id);
            }
        });

        return { adds, deletes };
    }

    // #endregion

    // #region TeamActivities

    public removeActivityFromTeam(activityTeamAttendance: ActivityTeamAttendanceDto) {
        const found = this._memoryStore.focussedTeamAttendance.attendanceActivities.find(aa => aa.id === activityTeamAttendance.id);
        const index = this._memoryStore.focussedTeamAttendance.attendanceActivities.indexOf(found);
        this._memoryStore.focussedTeamAttendance.attendanceActivities.splice(index, 1);

        this._focussedTeamAttendance.next(this._memoryStore.focussedTeamAttendance);
    }

    public addActivityToTeam(attendanceActivity: ActivityTeamAttendanceDto) {
        this._memoryStore.focussedTeamAttendance.attendanceActivities.push(attendanceActivity);

        this._focussedTeamAttendance.next(this._memoryStore.focussedTeamAttendance);
    }

    public identifyTeamAttendanceActivityChanges(changes: ActivityDto[]) {
        const adds: number[] = [];
        const deletes: number[] = [];

        const focussedTeamAttendanceActivities = this._memoryStore.focussedTeamAttendance.attendanceActivities;
        changes.forEach(a => {
            const activityTeamAttendance = focussedTeamAttendanceActivities.find(aa => aa.activityId === a.id);

            if (activityTeamAttendance) {
                deletes.push(activityTeamAttendance.id);
            } else {
                adds.push(a.id);
            }
        });

        return { adds, deletes };
    }

    // #endregion

    // #region Public Methods Other

    public reset(): void {
        this._memoryStore = {
            registrationOverview: null,
            memberAttendances: null,
            teamAttendances: null,
            activitiesForIndividuals: null,
            activitiesForTeams: null,
            focussedMemberAttendance: null,
            focussedTeamAttendance: null,
            contactInfo: null
        };

        this._registrationOverview.next(null);
        this._allGroupMembers.next([]);
        this._allGroupTeams.next([]);
        this._memberAttendances.next([]);
        this._teamAttendances.next([]);
        this._activitiesForIndividuals.next(null);
        this._activitiesForTeams.next([]);
        this._focussedMemberAttendance.next(null);
        this._focussedTeamAttendance.next(null);
    }

    // #endregion

    // #region Capabilities

    private setCapabilities(flowState: number) {
        switch (flowState) {
            case RegistrationFlowState.new:
                this._capabilityStore = {
                    canAddEditMembers: true,
                    canAddEditTeams: true,
                    canDeleteMembers: true,
                    canDeleteTeams: true,
                    canConfirm: false
                };
                break;
            case RegistrationFlowState.submitted:
            case RegistrationFlowState.confirmed:
            case RegistrationFlowState.pulled:
                this._capabilityStore = {
                    canAddEditMembers: true,
                    canAddEditTeams: true,
                    canDeleteMembers: true,
                    canDeleteTeams: true,
                    canConfirm: true
                };
                break;
            default:
                this._capabilityStore = {
                    canAddEditMembers: false,
                    canAddEditTeams: false,
                    canDeleteMembers: false,
                    canDeleteTeams: false,
                    canConfirm: false
                };
        }
    }

    // #endregion

    // #region Private Helper Methods

    private updateOverviewCounts() {

        // Members
        if (this._memoryStore.memberAttendances) {
            this._memoryStore.registrationOverview.memberAttendancesCount = this._memoryStore.memberAttendances
                .filter(ma => ma.groupMemberType === MemberType.active).length;
            this._memoryStore.registrationOverview.guestAttendancesCount = this._memoryStore.memberAttendances
                .filter(ma => ma.groupMemberType === MemberType.guest).length;
        }

        // teams
        if (this._memoryStore.teamAttendances) {
            this._memoryStore.registrationOverview.teamAttendancesCount = this._memoryStore.teamAttendances.length;
        }
    }

    // #endregion
}

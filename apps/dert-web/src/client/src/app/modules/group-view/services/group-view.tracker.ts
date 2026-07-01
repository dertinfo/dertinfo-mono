import { Injectable } from '@angular/core';
import {
    DanceDetailDto, GroupDto, GroupRegistrationDto, GroupTeamDto, TeamAttendanceDto
} from 'app/models/dto';
import { BehaviorSubject } from 'rxjs';

import { DodGroupResultsDto } from 'app/models/dto/DodGroupResultsDto';
import { TeamActivityResultStorage } from '../models/TeamActivityResultStorage';

@Injectable()
export class GroupViewTracker {

    private _group = new BehaviorSubject<GroupDto>(null);
    private _focussedTeamAttendances = new BehaviorSubject<TeamAttendanceDto[]>([]);
    private _groupRegistrations = new BehaviorSubject<GroupRegistrationDto[]>([]);
    private _focussedRegistration = new BehaviorSubject<GroupRegistrationDto>(null);
    private _focussedResults = new BehaviorSubject<Array<DanceDetailDto>>([]);
    private _dodResults = new BehaviorSubject<DodGroupResultsDto>(null);

    get group$() { return this._group.asObservable(); }
    get focussedTeamAttendances$() { return this._focussedTeamAttendances.asObservable(); }
    get groupRegistrations$() { return this._groupRegistrations.asObservable(); }
    get focussedRegistration$() { return this._focussedRegistration.asObservable(); }
    get focussedResults$() { return this._focussedResults.asObservable(); }
    get dodResults$() { return this._dodResults.asObservable(); }

    private _memoryStore: {
        group: GroupDto,
        teamAttendances: { [registrationId: number]: TeamAttendanceDto[] },
        registrations: GroupRegistrationDto[],
        results: Array<TeamActivityResultStorage>,
        dodResults: DodGroupResultsDto
    };

    constructor() {
        this.reset();
    }

    public get groupId(): number {
        return this._memoryStore.group.id;
    }

    public get focussedRegistrationId(): number {

        if (this._focussedRegistration.value) {
            return this._focussedRegistration.value.id;
        } else {
            return 0;
        }
    }

    public get group(): GroupDto {
        return this._memoryStore.group;
    }
    public set group(group: GroupDto) {

        this._memoryStore.group = group;
        this._group.next(this._memoryStore.group);
    }

    public set registrations(registrations: GroupRegistrationDto[]) {

        this._memoryStore.registrations = registrations;
        this._groupRegistrations.next(registrations);
    }

    public get dodResults(): DodGroupResultsDto {
        return this._memoryStore.dodResults;
    }
    public set dodResults(dodResults: DodGroupResultsDto) {
        this._memoryStore.dodResults = dodResults;
        this._dodResults.next(this._memoryStore.dodResults);
    }

    public getTeamAttendanceForRegistration(registrationId: number) {
        return this._memoryStore.teamAttendances[registrationId];
    }

    public hasLoadedGroup(): boolean {
        return this._memoryStore.group !== null;
    }

    public hasLoadedTeamAttendances(registrationId: number): boolean {
        return this._memoryStore.teamAttendances &&
            null !== this._memoryStore.teamAttendances[registrationId] &&
            undefined !== this._memoryStore.teamAttendances[registrationId];
    }

    public hasLoadedTeamActivityResults(teamId: number, activityId: number): boolean {
        return this._memoryStore.results.some(tar => tar.teamId === teamId && tar.activityId === activityId);
    }

    public hasLoadedRegistrations(): boolean {
        return this._memoryStore.registrations !== null;
    }

    public hasRegistrationDetail(registrationId: number): boolean {

        // note - intermediate find while base registration is sufficient
        const registration = this._memoryStore.registrations.find(r => r.id === registrationId);

        return null !== registration && undefined !== registration;
    }

    public hasLoadedDodGroupResults(): boolean {
        return this._memoryStore.dodResults !== null;
    }

    public addTeamAttendances(registrationId: number, teamAttendances: TeamAttendanceDto[], setFocussed: boolean = false) {

        this._memoryStore.teamAttendances[registrationId] = teamAttendances;

        if (setFocussed) {
            this._focussedTeamAttendances.next(this._memoryStore.teamAttendances[registrationId]);
        }
    }

    public addTeamActivityResults(teamId: number, activityId: number, danceResults: DanceDetailDto[], setFocussed: boolean): any {

        const teamActivityResults = {
            teamId: teamId,
            activityId: activityId,
            danceResults: danceResults
        };

        this._memoryStore.results.push(teamActivityResults);

        if (setFocussed) {
            this.focusTeamActivityResults(teamId, activityId);
        }
    }

    public focusTeamActivityResults(teamId: number, activityId: number): any {

        const results = this._memoryStore.results.find(tars => tars.teamId === teamId && tars.activityId === activityId);
        this._focussedResults.next(results.danceResults);
    }

    public focusRegistrationDetail(registrationId: number) {

        const registration = this._memoryStore.registrations.find(r => r.id === registrationId);
        this._focussedRegistration.next(registration);
    }

    public removeDodResult(dodResultId) {
        const index = this._memoryStore.dodResults.scoreCards.findIndex(sc => sc.dodResultId === dodResultId);
        this._memoryStore.dodResults.scoreCards.splice(index, 1);
        this._dodResults.next(this._memoryStore.dodResults);
    }

    public reset(): void {
        this._memoryStore = {
            group: null,
            teamAttendances: {},
            registrations: [],
            results: [],
            dodResults: null,
        };

        this._group.next(null);
        this._focussedTeamAttendances.next([]);
        this._groupRegistrations.next([]);
        this._focussedResults.next([]);
    }
}

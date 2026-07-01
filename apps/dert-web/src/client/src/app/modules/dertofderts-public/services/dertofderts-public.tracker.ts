import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

import { DodTeamCollatedResultPairDto } from 'app/models/dto/DodCollatedResultPairDto';
import { DodSubmissionDto } from 'app/models/dto/DodSubmissionDto';
import { DodTalkDto } from 'app/models/dto/DodTalkDto';

@Injectable()
export class Tracker {

    private _dodSubmissionsSubject = new BehaviorSubject<DodSubmissionDto[]>([]);
    private _dodTalksSubject = new BehaviorSubject<DodTalkDto[]>([]);
    private _activeSubmissionSubject = new BehaviorSubject<DodSubmissionDto>(null);
    private _dodResultsSubject = new BehaviorSubject<DodTeamCollatedResultPairDto>(null);

    private _memoryStore: {
        dodSubmissions: DodSubmissionDto[],
        dodTalks: DodTalkDto[],
        dodResults: DodTeamCollatedResultPairDto,
    };

    public get currentSubmissionId() { return this._activeSubmissionSubject.value.id; }

    public get dodSubmissions$() { return this._dodSubmissionsSubject.asObservable(); }
    public get dodTalks$() { return this._dodTalksSubject.asObservable(); }
    public get activeSubmission$() { return this._activeSubmissionSubject.asObservable(); }
    public get dodResults$() { return this._dodResultsSubject.asObservable(); }

    public set dodSubmissions(submissions: DodSubmissionDto[]) {
        this._memoryStore.dodSubmissions = submissions;
        this._dodSubmissionsSubject.next(this._memoryStore.dodSubmissions);
    }

    public set dodTalks(talks: DodTalkDto[]) {
        this._memoryStore.dodTalks = talks;
        this._dodTalksSubject.next(this._memoryStore.dodTalks);
    }

    public set dodResults(results: DodTeamCollatedResultPairDto) {
        this._memoryStore.dodResults = results;
        this._dodResultsSubject.next(this._memoryStore.dodResults);
    }

    public hasLoadedSubmissions(): boolean { return this._memoryStore.dodSubmissions != null; }
    public hasLoadedTalks(): boolean { return this._memoryStore.dodTalks != null; }
    public hasLoadedResults(): boolean { return this._memoryStore.dodResults != null; }

    public populateActiveSubmission(submissionId: number) {
        const activateSubmission = this._dodSubmissionsSubject.value.filter(sub => sub.id === submissionId);
        this._activeSubmissionSubject.next(activateSubmission[0]);
    }

    public incrementResultCount(submissionId: number) {

        // Update the object in the memory store
        const index = this._memoryStore.dodSubmissions.findIndex(s => s.id === this.currentSubmissionId);
        const found = this._memoryStore.dodSubmissions[index];

        found.numberOfResults = found.numberOfResults + 1;

        // Publish the memory store listing.
        this._dodSubmissionsSubject.next(this._memoryStore.dodSubmissions);
    }

    public reset() {
        this._memoryStore = {
            dodSubmissions: null,
            dodResults: null,
            dodTalks: null
        };

        this._dodSubmissionsSubject.next([]);
        this._dodTalksSubject.next(null);
        this._dodResultsSubject.next(null);

    }
}

import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

import { GroupDto } from 'app/models/dto';
import { DodJudgeInfoDto } from 'app/models/dto/DodJudgeInfoDto';
import { DodResultComplaintDto } from 'app/models/dto/DodResultComplaintDto';
import { DodSubmissionDto } from 'app/models/dto/DodSubmissionDto';
import { DodTalkDto } from 'app/models/dto/DodTalkDto';

@Injectable()
export class Tracker {

    private _dodComplaints = new BehaviorSubject<DodResultComplaintDto[]>([]);
    private _dodSubmissions = new BehaviorSubject<DodSubmissionDto[]>([]);
    private _dodTalks = new BehaviorSubject<DodTalkDto[]>([]);
    private _dodJudgeInfos = new BehaviorSubject<DodJudgeInfoDto[]>([]);
    private _focussedSubmission = new BehaviorSubject<DodSubmissionDto>(null);
    private _focussedTalk = new BehaviorSubject<DodTalkDto>(null);
    private _groups = new BehaviorSubject<GroupDto[]>([]);

    private _memoryStore: {
        dodSubmissions: DodSubmissionDto[],
        dodTalks: DodTalkDto[],
        groups: GroupDto[],
        dodJudgeInfos: DodJudgeInfoDto[],
        dodComplaints: DodResultComplaintDto[],
    };

    get dodSubmissions$() { return this._dodSubmissions.asObservable(); }
    get dodTalks$() { return this._dodTalks.asObservable(); }
    get dodComplaints$() { return this._dodComplaints.asObservable(); }
    get groups$() { return this._groups.asObservable(); }
    get dodJudgeInfos$() { return this._dodJudgeInfos.asObservable(); }
    get focussedSubmission$() { return this._focussedSubmission.asObservable(); }
    get focussedTalk$() { return this._focussedTalk.asObservable(); }

    public set dodSubmissions(submissions: DodSubmissionDto[]) {
        this._memoryStore.dodSubmissions = submissions;
        this._dodSubmissions.next(this._memoryStore.dodSubmissions);
    }

    public set dodTalks(talks: DodTalkDto[]) {
        this._memoryStore.dodTalks = talks;
        this._dodTalks.next(this._memoryStore.dodTalks);
    }

    public set dodJudgeInfos(judgeInfos: DodJudgeInfoDto[]) {
        this._memoryStore.dodJudgeInfos = judgeInfos;
        this._dodJudgeInfos.next(this._memoryStore.dodJudgeInfos);
    }

    public set dodComplaints(complaints: DodResultComplaintDto[]) {
        this._memoryStore.dodComplaints = complaints;
        this._dodComplaints.next(this._memoryStore.dodComplaints);
    }

    public set groups(groups: GroupDto[]) {
        this._memoryStore.groups = groups;
        this._groups.next(this._memoryStore.groups);
    }

    public addSubmission(dodSubmission: DodSubmissionDto) {
        this._memoryStore.dodSubmissions.push(dodSubmission);
        this._dodSubmissions.next(this._memoryStore.dodSubmissions);
    }

    public addTalk(dodTalk: DodTalkDto) {
        this._memoryStore.dodTalks.push(dodTalk);
        this._dodTalks.next(this._memoryStore.dodTalks);
    }

    public deleteSubmission(dodSubmissionId: number) {
        this._memoryStore.dodSubmissions = this._memoryStore.dodSubmissions.filter(s => s.id !== dodSubmissionId);
        this._dodSubmissions.next(this._memoryStore.dodSubmissions);
    }

    public deleteTalk(dodTalkId: number) {
        this._memoryStore.dodTalks = this._memoryStore.dodTalks.filter(s => s.id !== dodTalkId);
        this._dodTalks.next(this._memoryStore.dodTalks);
    }

    public updateSubmission(updatedSubmission: DodSubmissionDto) {
        const index = this._memoryStore.dodSubmissions.findIndex(t => t.id === updatedSubmission.id);
        this._memoryStore.dodSubmissions[index] = updatedSubmission;
        this._dodSubmissions.next(this._memoryStore.dodSubmissions);
        this._focussedSubmission.next(updatedSubmission);
    }

    public updateTalk(updatedTalk: DodTalkDto) {
        const index = this._memoryStore.dodTalks.findIndex(t => t.id === updatedTalk.id);
        this._memoryStore.dodTalks[index] = updatedTalk;
        this._dodTalks.next(this._memoryStore.dodTalks);
        this._focussedTalk.next(updatedTalk);
    }

    public hasLoadedSubmissions(): boolean { return this._memoryStore.dodSubmissions !== null; }
    public hasLoadedTalks(): boolean { return this._memoryStore.dodTalks !== null; }
    public hasLoadedGroups(): boolean { return this._memoryStore.groups !== null; }
    public hasLoadedJudgeInfos(): boolean { return this._memoryStore.dodJudgeInfos !== null; }
    public hasLoadedComplaints(): boolean { return this._memoryStore.dodComplaints !== null; }

    public focusSubmission(submissionId: number) {
        const index = this._memoryStore.dodSubmissions.findIndex(x => x.id === submissionId);
        const found = this._memoryStore.dodSubmissions[index];

        this._focussedSubmission.next(found);
    }

    public focusTalk(talkId: number) {
        const index = this._memoryStore.dodTalks.findIndex(x => x.id === talkId);
        const found = this._memoryStore.dodTalks[index];

        this._focussedTalk.next(found);
    }

    markComplaintResolved(complaint: DodResultComplaintDto) {
        const index = this._memoryStore.dodComplaints.findIndex(x => x.id === complaint.id);
        this._memoryStore.dodComplaints.splice(index, 1);
        this._dodComplaints.next(this._memoryStore.dodComplaints);
    }

    toggleJudgeBlocked(userId: number) {
        const index = this._memoryStore.dodJudgeInfos.findIndex(j => j.dodUserId === userId);
        const found = this._memoryStore.dodJudgeInfos[index];
        found.resultsBlocked = !found.resultsBlocked;
        this._dodJudgeInfos.next(this._memoryStore.dodJudgeInfos);
    }

    public reset() {
        this._memoryStore = {
            dodSubmissions: null,
            dodTalks: null,
            groups: null,
            dodJudgeInfos: null,
            dodComplaints: null
        };

        this._dodSubmissions.next([]);
        this._dodTalks.next([]);
        this._dodJudgeInfos.next([]);
        this._dodComplaints.next([]);
        this._groups.next([]);
        this._focussedTalk.next(null);
    }
}

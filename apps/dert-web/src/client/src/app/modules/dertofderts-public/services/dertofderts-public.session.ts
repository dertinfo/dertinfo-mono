import { Injectable } from '@angular/core';
import { now } from 'moment';
import { LocalStorageData } from '../models/localStorageData.model';
import { SessionStorageData } from '../models/sessionStorageData.model';
import { SubmitFormData } from '../models/submitFormData.model';

@Injectable()
export class SessionService {

    private _session: SessionStorageData;
    private _local: LocalStorageData;

    constructor() { }

    public hasSessionState() {
        if (this._session == null) { this.loadSession(); }

        return this._session !== null;
    }

    public hasLocalState() {
        if (this._local == null) { this.loadLocal(); }

        return this._local != null && this._local.userGuid != null;
    }

    public loadOrTemplateLocal() {

        if (this._local == null) { this.loadLocal(); }

        if (this._local == null) {

            this._local = {
                userGuid: null,
                expireAt: new Date(new Date().getTime() + (3 * 24 * 60 * 60 * 1000))
                // Set the expiry in 3 days. Session will no longer be recoverable after this time.
            };
        }
    }

    /**
     * Used to store the user provided info before the response from the api.
     * This is so that the data is persisted and retrivable incase there is an error in the submission.
     * @param submitFormData - the data provided on the submit dialog.
     */
    public loadOrTemplateSession(submitFormData: SubmitFormData, userGuid: string = null) {

        // Load the current session if there is one and is not in memory
        if (this._session == null) { this.loadSession(); }

        if (this._session == null) {
             // If there is still no session create a blank one with the new data
            this._session = {
                name: submitFormData.name,
                email: submitFormData.email,
                interestedInJudging: submitFormData.interestedInJudging,
                officialJudge: false,
                agreeToTermsAndConditions: submitFormData.agreeToTermsAndConditions,
                dancesJudged: [],
                userGuid: userGuid
            };
        }
    }

    public getLocalInfo() {
        if (this._local == null) { this.loadLocal(); }
        if (this._local != null) { return this._local; }
        return null;
    }

    public getSessionInfo() {
        if (this._session == null) { this.loadSession(); }
        if (this._session != null) { return this._session; }
        return null;
    }

    public addUserGuid(userGuid: string) {
        if (this._local == null) { this.loadLocal(); }
        if (this._session == null) { this.loadSession(); }
        if (this._local == null) { throw new Error('You cannot add a user guid unless there is a user local storage.'); }
        if (this._local == null) { throw new Error('You cannot add a user guid unless there is a user session storage.'); }

        this._local.userGuid = userGuid;
        this._local.expireAt = new Date(new Date().getTime() + (3 * 24 * 60 * 60 * 1000));
        this._session.userGuid = userGuid;
        // Roll the expiry forward by 3 days.

        this.saveLocal(this._local);
    }

    public addDanceJudged(danceId: number) {
        if (this._session == null) { this.loadSession(); }
        if (this._session == null) { throw new Error('You cannot add dances judged unless there is a user session.'); }

        if (!this._session.dancesJudged.some(x => x === danceId)) {
            this._session.dancesJudged.push(danceId);
        }

        this.saveSession(this._session);
    }

    public applyJudgeOfficialStatus(isOfficial: boolean) {
        if (this._session == null) { this.loadSession(); }
        if (this._session == null) { throw new Error('You cannot add dances judged unless there is a user session.'); }

        this._session.officialJudge = isOfficial;

        this.saveSession(this._session);
    }

    public getLocalUserGuid(): string {
        if (this._local == null) { this.loadLocal(); }
        if (this._local != null) { return this._local.userGuid; }
        return null;
    }

    public getSessionUserGuid(): string {
        if (this._session == null) { this.loadSession(); }
        if (this._session != null) { return this._session.userGuid; }
        return null;
    }

    public getDancesJudged(): Array<number> {
        if (this._session == null) { this.loadSession(); }
        if (this._session != null) { return this._session.dancesJudged; }
        return [];
    }

    public clearSession(): void {
        sessionStorage.removeItem('dodsession');
        this._session = null;
    }

    public clearLocal(): void {
        localStorage.removeItem('dodsession');
        this._local = null;
    }

    private saveLocal(localStorageData: LocalStorageData) {
        this._local = localStorageData;

        localStorage.setItem('dodsession', JSON.stringify(localStorageData));
    }

    private loadLocal() {
        if (this._local == null) {
            this._local = JSON.parse(localStorage.getItem('dodsession'));

            if (this._local && this._local.expireAt > new Date()) {
                this.clearLocal();
            }
        }
    }

    private saveSession(sessionStorageData: SessionStorageData) {
        this._session = sessionStorageData;

        sessionStorage.setItem('dodsession', JSON.stringify(sessionStorageData));
    }

    private loadSession() {
        if (this._session == null) {
            this._session = JSON.parse(sessionStorage.getItem('dodsession'));
        }
    }

}

import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

import { DodSubmissionSubmissionDto } from 'app/models/dto/DodSubmissionSubmissionDto';
import { DodTalkSubmissionDto } from 'app/models/dto/DodTalkSubmissionDto';

@Injectable()
export class Mediator {

    private _closeModalSubject: Subject<null> = new Subject<null>();
    private _newSubmissionSubject: Subject<DodSubmissionSubmissionDto> = new Subject<DodSubmissionSubmissionDto>();
    private _newTalkSubject: Subject<DodTalkSubmissionDto> = new Subject<DodTalkSubmissionDto>();

    public closeModal$ = this._closeModalSubject.asObservable();
    public newSubmission$ = this._newSubmissionSubject.asObservable();
    public newTalk$ = this._newTalkSubject.asObservable();

    constructor() { }

    public announceNewSubmission(myNew: DodSubmissionSubmissionDto) {
        this._newSubmissionSubject.next(myNew);
    }

    public announceNewTalk(myNew: DodTalkSubmissionDto) {
        this._newTalkSubject.next(myNew);
    }

    public announceCloseModal() {
        this._closeModalSubject.next();
    }
}

import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { NotificationMessageSubmissionDto } from '../models/NotificationMessageSubmissionDto';

@Injectable()
export class NotificationAdminMediator {

    private _closeModalSubject: Subject<null> = new Subject<null>();
    private _newNotificationSubject: Subject<NotificationMessageSubmissionDto> = new Subject<NotificationMessageSubmissionDto>();

    public closeModal$ = this._closeModalSubject.asObservable();
    public newNotificationMessage$ = this._newNotificationSubject.asObservable();

    constructor() { }

    public announceNewNotificationMessage(myNew: NotificationMessageSubmissionDto) {
        this._newNotificationSubject.next(myNew);
    }

    public announceCloseModal() {
        this._closeModalSubject.next();
    }
}

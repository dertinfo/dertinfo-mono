import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { NotificationMessageDto } from '../models/NotificationMessageDto';

@Injectable()
export class NotificationManagementTracker {

    private _allNotificationMesssagesSubject = new BehaviorSubject<Array<NotificationMessageDto>>([]);

    private _memoryStore: {
        allNotificationMessages: Array<NotificationMessageDto>
    };

    public get allNotificationMessages$() { return this._allNotificationMesssagesSubject.asObservable(); }

    public set allNotificationMessages(notificationMessages: Array<NotificationMessageDto>) {
        this._memoryStore.allNotificationMessages = notificationMessages;
        this._allNotificationMesssagesSubject.next(this._memoryStore.allNotificationMessages);
    }

    public hasLoadedAllNotificationMessages(): boolean { return this._memoryStore.allNotificationMessages !== null; }

    public addNotificationMessage(notificationMessage: NotificationMessageDto) {
        this._memoryStore.allNotificationMessages.unshift(notificationMessage);
        this._allNotificationMesssagesSubject.next(this._memoryStore.allNotificationMessages);
    }

    public deleteNotificationMessage(deleted: NotificationMessageDto) {
        const found = this._memoryStore.allNotificationMessages.find(n => n.id === deleted.id);
        const index = this._memoryStore.allNotificationMessages.indexOf(found);
        this._memoryStore.allNotificationMessages.splice(index, 1);

        this._allNotificationMesssagesSubject.next(this._memoryStore.allNotificationMessages);
    }

    public reset() {
        this._memoryStore = {
            allNotificationMessages: null
        };

        this._allNotificationMesssagesSubject.next([]);
    }
}

import { Injectable } from '@angular/core';
import { NotificationMessageDto } from '../models/NotificationMessageDto';
import { NotificationMessageSubmissionDto } from '../models/NotificationMessageSubmissionDto';

import { NotificationAdminRepository } from './notification-management.repository';
import { NotificationManagementTracker } from './notification-management.tracker';

@Injectable()
export class NotificationAdminConductor {

    constructor(
        private repository: NotificationAdminRepository,
        private tracker: NotificationManagementTracker
    ) {
        this.tracker.reset();
    }

    public getAllMessages() {
        const obs$ = this.repository.getAllNotificationMessages();

        obs$.subscribe(notificationMessages => {
            this.tracker.allNotificationMessages = notificationMessages;
        });

        return obs$;
    }

    public addNotificationMessage(notificationMessage: NotificationMessageSubmissionDto) {
        const subs = this.repository.addNotificationMessage(notificationMessage).subscribe((added: NotificationMessageDto) => {
            subs.unsubscribe();
            this.tracker.addNotificationMessage(added);
        });
    }

    public deleteNotificationMessage(notificationMessage: NotificationMessageDto) {
        const subs = this.repository.deleteNotificationMessage(notificationMessage).subscribe((deleted: NotificationMessageDto) => {
            subs.unsubscribe();
            this.tracker.deleteNotificationMessage(deleted);
        });
    }
}

import { Injectable } from '@angular/core';
import { NotificationSummaryDto } from '../models/NotificationSummaryDto';
import { NotificationThumbnailInfoDto } from '../models/NotificationThumbnailInfoDto';
import { NotificationRepository } from './notification.repository';
import { NotificationTracker } from './notification.tracker';

@Injectable()
export class NotificationConductor {

    public lastThumbnailInfoRecieved: NotificationThumbnailInfoDto;

    constructor(
        private repository: NotificationRepository,
        private tracker: NotificationTracker
    ) {
        this.tracker.reset();
    }

    public checkForMessages() {
        const obs$ = this.repository.getCheck();

        obs$.subscribe(nti => {
            this.tracker.thumbnailInfo = nti;
        });

        return obs$;
    }

    public getMessageSummaries() {
        const obs$ = this.repository.getSummaries();

        obs$.subscribe(summaries => {
            this.tracker.summaries = summaries;
            this.updateThumbnailAfterAction(summaries);
        });

        return obs$;
    }

    public getMessageDetail(id: number) {
        const obs$ = this.repository.getDetail(id);

        obs$.subscribe(detail => {
            this.tracker.focussedNotification = detail;
        });

        return obs$;
    }

    public dismissMessage(id: number) {
        const obs$ = this.repository.dismissMessage(id);

        obs$.subscribe(x => {
            this.tracker.setNotificationDismissed(id);
        });

        return obs$;
    }

    public tagNotificationOpened(notificationLogId: number) {

        this.tracker.clearAnyBlocker();

        if (this.tracker.hasLoadedSummaries()) {
            this.tracker.setNotificationOpened(notificationLogId);
            this.updateThumbnailAfterAction(this.tracker.summaries);
        }
    }

    public tagNotificationAcknowledged(notificationLogId: number) {

        const obs$ = this.repository.acknowledgeNotification(notificationLogId);

        obs$.subscribe(x => {
            this.tracker.setNotificationAcknowledged(notificationLogId);
        });

        return obs$;
    }

    /**
     * When the user opens the summary panel thier messages will now been seen.
     * Depending on what the summaries contain we need to update the thumbnail with the
     * max severity and if there are messages that they need to do something with.
     * @param suammries - the summaries for the drawer
     */
    public updateThumbnailAfterAction(summaries: Array<NotificationSummaryDto>) {
        if (this.tracker.hasLoadedSummaries()) {
            const stillToBeOpened = summaries.filter(s => !s.canDismiss && s.canOpen && !s.hasBeenOpened);
            const stillToBeOpenedMaxSeverity = Math.max.apply(Math, stillToBeOpened.map((s) => { return s.severity; }));
            this.tracker.updateClientThumbnailState(stillToBeOpened.length > 0, stillToBeOpenedMaxSeverity);
        } else {
            this.tracker.clearAnyBlocker();
        }
    }

}

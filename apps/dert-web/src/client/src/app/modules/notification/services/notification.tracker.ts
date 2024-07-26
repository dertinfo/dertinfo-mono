import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { NotificationDetailDto } from '../models/NotificationDetailDto';
import { NotificationSummaryDto } from '../models/NotificationSummaryDto';
import { NotificationThumbnailInfoDto } from '../models/NotificationThumbnailInfoDto';

@Injectable()
export class NotificationTracker {

    private _thumnailInfoSubject = new BehaviorSubject<NotificationThumbnailInfoDto>(null);
    private _summariesSubject = new BehaviorSubject<NotificationSummaryDto[]>([]);
    private _focussedNotificationSubject = new BehaviorSubject<NotificationDetailDto>(null);

    private _memoryStore: {
        thumnailInfo: NotificationThumbnailInfoDto,
        summaries: NotificationSummaryDto[],
        focussedNotification: NotificationDetailDto
    };

    public get thumbnailInfo$() { return this._thumnailInfoSubject.asObservable(); }
    public get summaries$() { return this._summariesSubject.asObservable(); }
    public get focussedNotification$() { return this._focussedNotificationSubject.asObservable(); }

    public set thumbnailInfo(thumnailInfo: NotificationThumbnailInfoDto) {
        this._memoryStore.thumnailInfo = thumnailInfo;
        this._thumnailInfoSubject.next(this._memoryStore.thumnailInfo);
    }

    public get summaries() { return this._memoryStore.summaries; } // We get these for processing in the conductor.
    public set summaries(summaries: NotificationSummaryDto[]) {
        this._memoryStore.summaries = summaries;
        this._summariesSubject.next(this._memoryStore.summaries);
    }

    public set focussedNotification(focussedNotification: NotificationDetailDto) {
        this._memoryStore.focussedNotification = focussedNotification;
        this._focussedNotificationSubject.next(this._memoryStore.focussedNotification);
    }

    public hasLoadedThumbnailInfo(): boolean { return this._memoryStore.thumnailInfo !== null; }
    public hasLoadedSummaries(): boolean { return this._memoryStore.summaries !== null; }
    public hasFocussedNotification(): boolean { return this._memoryStore.focussedNotification !== null; }

    public clearAnyBlocker() {
        // If we have a block then this opening will be the blocker so we need to clear the block
        if (this._memoryStore.thumnailInfo.hasBlocking) {
            this._memoryStore.thumnailInfo.hasBlocking = false;
            this._memoryStore.thumnailInfo.blockingNotificationLogId = null;
        }
    }

    public setNotificationOpened(notificationLogId: any) {

        if (this.hasLoadedSummaries()) {
            const index = this._memoryStore.summaries.findIndex(s => s.notificationAudienceLogId === notificationLogId);
            if (index > -1) {
                // if we have a blocking notification it is not yet loaded in the summary hence may be null.
                const found = this._memoryStore.summaries[index];
                found.hasBeenOpened = true;
                found.canDismiss = found.mustAcknowledge ? found.hasBeenAcknowledged : true;
            }

            this._summariesSubject.next(this._memoryStore.summaries);
        }
    }

    public setNotificationAcknowledged(notificationLogId: number) {

        if (this.hasLoadedSummaries()) {

            const index = this._memoryStore.summaries.findIndex(s => s.notificationAudienceLogId === notificationLogId);
            if (index > -1) {
                // if we have a blocking notification it is not yet loaded in the summary hence may be null.
                const found = this._memoryStore.summaries[index];
                found.hasBeenAcknowledged = true;
                found.canDismiss = found.mustAcknowledge ? found.hasBeenAcknowledged : true;
            }

            this._summariesSubject.next(this._memoryStore.summaries);
        }
    }

    public setNotificationDismissed(id: number) {
        if (this.hasLoadedSummaries()) {

            const found = this._memoryStore.summaries.find(s => s.notificationAudienceLogId === id);
            const index = this._memoryStore.summaries.indexOf(found);
            this._memoryStore.summaries.splice(index, 1);

            this._summariesSubject.next(this._memoryStore.summaries);
        }
    }

    /**
     * Update the thumbnail state when the panel is opened and the summaries are gained.
     * @param hasUnreadNotifications - boolean to indicate if any of the summaries need further action.
     * @param maxUnreadSeverity - the maximum severity of the remaining messages that require action.
     */
    public updateClientThumbnailState(hasUnreadNotifications: boolean, maxUnreadSeverity: number) {
        this._memoryStore.thumnailInfo.hasUnreadMessages = hasUnreadNotifications;
        this._memoryStore.thumnailInfo.maximumMessageSeverity = maxUnreadSeverity;

        this._thumnailInfoSubject.next(this._memoryStore.thumnailInfo);
    }

    public reset() {
        this._memoryStore = {
            thumnailInfo: null,
            summaries: null,
            focussedNotification: null
        };

        this._thumnailInfoSubject.next(null);
        this._summariesSubject.next([]);
        this._focussedNotificationSubject.next(null);
    }
}

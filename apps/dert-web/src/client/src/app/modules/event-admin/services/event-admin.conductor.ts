import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

import { NavigationService } from 'app/core/services/navigation.service';
import { EventImage } from 'app/models/app';
import { EventRegistration } from 'app/models/app/EventRegistration';
import {
    ActivityDto,
    ActivitySubmissionDto,
    ActivityUpdateDto,
    EmailTemplateDto,
    EmailTemplateUpdateSubmissionDto,
    EventActivityDto,
    EventDatesUpdateDto,
    EventDto,
    EventImageDto,
    EventOverviewDto,
    EventOverviewUpdateDto,
    EventRegistrationDto,
    InvoicePaidUpdateSubmissionDto
} from 'app/models/dto';
import { EventCancellationOptionsDto } from 'app/models/dto/EventCancellationOptionsDto';
import { EventListCache, EventRepository, InvoiceRepository } from 'app/modules/repositories';

import { EventAdminTracker } from './event-admin.tracker';

@Injectable()
export class EventAdminConductor {

    private _snackBarDuration: number = 1200;

    public activityAudienceTypes = [{ id: 1, name: 'Individual' }, { id: 2, name: 'Team' }];

    constructor(

        private _tracker: EventAdminTracker,
        private eventRepo: EventRepository,
        private invoiceRepo: InvoiceRepository,
        private navigationService: NavigationService,
        private snackBar: MatSnackBar,
        private _eventListCache: EventListCache,
    ) {

        this._tracker.reset();
    }

    public clear() {

        this._tracker.reset();
    }

    public setOverview(eventOverviewDto: EventOverviewDto) {

        if (this._tracker.overview && this._tracker.overview.id !== eventOverviewDto.id) {
            this._tracker.reset();
        }

        // todo - determine the closed status of the event,
        this._tracker.overview = {
            ...eventOverviewDto,
            isDeleting: false,
            isDeleted: false,
            isClosing: false,
            isClosed: false,
            isCancelling: false,
            isCancelled: false
        };

        this.initRegistrations(this._tracker.eventId);
    }

    public initImages(eventId, forceUpdate = false) {

        if (!this._tracker.hasLoadedImages() || forceUpdate) {
            const subs = this.eventRepo.images(eventId).subscribe((eventImages) => {
                subs.unsubscribe();
                const overview = this._tracker.overview;

                // Check to see if there is only 1 image. If there is then the default may have been replaced.
                if (eventImages && eventImages.length === 1) {
                    overview.eventPictureUrl = eventImages[0].imageResourceUri;
                    this._tracker.overview = overview;
                }

                this._tracker.images = eventImages;
            });
        }
    }

    public initInvoices(eventId) {

        if (!this._tracker.hasLoadedInvoices()) {
            const subs = this.invoiceRepo.invoicesByEvent(eventId).subscribe((invoices) => {
                subs.unsubscribe();
                this._tracker.invoices = invoices;
            });
        }
    }

    public initFocussedInvoice(invoiceId) {

        if (!this._tracker.hasInvoiceHistoryDetail(invoiceId)) {
            const subs = this.invoiceRepo.invoiceHistory(invoiceId).subscribe((invoiceHistory) => {
                subs.unsubscribe();
                this._tracker.addInvoiceHistory(invoiceHistory, true);
            });
        } else {
            this._tracker.focusInvoiceHistory(invoiceId);
        }
    }

    public initRegistrations(eventId) {

        if (!this._tracker.hasLoadedRegistrations()) {
            const subs = this.eventRepo.registrations(eventId).subscribe((registrations) => {
                subs.unsubscribe();
                this._tracker.registrations = registrations.map((rDto) => this.mapRegistrationDtoToEventRegistration(rDto));
            });
        }
    }

    public initActivities(eventId): any {

        if (!this._tracker.hasLoadedIndividualActivities()) {
            const subs = this.eventRepo.individualActivities(eventId).subscribe((individualActivities) => {
                subs.unsubscribe();
                this._tracker.individualActivities = individualActivities;
            });
        }

        if (!this._tracker.hasLoadedTeamActivities()) {
            const subs = this.eventRepo.teamActivities(eventId).subscribe((teamActivities) => {
                subs.unsubscribe();
                this._tracker.teamActivities = teamActivities;
            });
        }
    }

    public initFocussedActivity(activityId: number, forceUpdate: boolean = false) {

        if (!this._tracker.hasActivityDetail(activityId) || forceUpdate) {
            const subs = this.eventRepo.activityDetail(this._tracker.eventId, activityId).subscribe((activityDetail) => {
                subs.unsubscribe();
                this._tracker.addActivityDetail(activityDetail, true);
            });
        }
    }

    // note - Now populates using a resolver
    // public initEmailTemplates(eventId): any {
    //

    //     if (!this._tracker.hasEmailTemplates()) {
    //         const subs = this.eventRepo.emailTemplates(eventId).subscribe((emailTemplates) => {
    //             subs.unsubscribe();
    //             this._tracker.emailTemplates = emailTemplates;
    //         });
    //     }
    // }

    public initFocussedEmailTemplate(emailTemplateId: number, forceUpdate: boolean = false) {

        if (!this._tracker.hasEmailTemplateDetail(emailTemplateId) || forceUpdate) {
            const subs = this.eventRepo.emailTemplateDetail(this._tracker.eventId, emailTemplateId).subscribe((emailTemplateDetail) => {
                subs.unsubscribe();
                this._tracker.addEmailTemplateDetail(emailTemplateDetail, true);
            });
        }
    }

    public initCompetitions(eventId) {

        if (!this._tracker.hasCompetitions()) {
            const subs = this.eventRepo.competitions(eventId).subscribe((competitions) => {
                subs.unsubscribe();
                this._tracker.competitions = competitions;
            });
        }
    }

    public initDownloads(eventId) {

        if (!this._tracker.hasDownloads()) {
            const subs = this.eventRepo.downloads(eventId).subscribe((confirmedTeams) => {
                subs.unsubscribe();
                this._tracker.downloads = confirmedTeams;
            });
        }
    }

    // "Apply" used when data is loaded from a resolver
    public applyEmailTemplates(emailTemplates: Array<EmailTemplateDto>) {
        // note - these are not full. They do not contin the score sheets.

        this._tracker.emailTemplates = emailTemplates;
    }

    public setPrimaryImage(eventImage: EventImage) {

        if (!eventImage.isPrimary) {

            const eventImageDto: EventImageDto = {
                eventId: eventImage.eventId,
                eventImageId: eventImage.eventImageId,
                imageId: eventImage.imageId,
                imageResourceUri: null
            };

            const subs = this.eventRepo.setPrimaryImage(eventImageDto).subscribe(() => {
                subs.unsubscribe();
                this._tracker.setEventPrimaryImage(eventImage.imageResourceUri);
            });
        }
    }

    public toggleInvoicePaid(invoiceId: number): void {

        const newValue: boolean = this._tracker.toggleInvoicePaid(invoiceId);

        const submission: InvoicePaidUpdateSubmissionDto = {
            invoiceId: invoiceId,
            hasPaid: newValue
        };

        const subs = this.eventRepo.updateInvoicePaid(this._tracker.eventId, submission).subscribe(() => {
            subs.unsubscribe();
        });
    }

    public refreshImages() {

        this.initImages(this._tracker.eventId, true);
    }

    public deleteImage(eventImage: EventImageDto) {

        const subs = this.eventRepo.deleteImage(eventImage).subscribe((deletedImage: EventImageDto) => {
            subs.unsubscribe();
            this._tracker.deleteImage(deletedImage.eventImageId);
        });
    }

    public removeEvent() {

        this._tracker.startDelete();

        const subs = this.eventRepo.deleteEvent(this._tracker.eventId).subscribe((removedEvent: EventDto) => {
            subs.unsubscribe();
            this._tracker.completeDelete();
            this.snackBar.open('Event Deleted', 'close', { duration: this._snackBarDuration });

            this._eventListCache.clearCache(); // todo - this would be tidier is we just remove the single

            // Insurance - to ensure the clear down has completed after the delete.
            setTimeout(() => {
                this.navigationService.removeEventForUser(removedEvent);
                this._tracker.reset();
            }, 2000); // note - this matches with the redirect after delete.
        });
    }

    public cancelEvent(options: EventCancellationOptionsDto) {
        this._tracker.startCancellation();

        const subs = this.eventRepo.cancelEvent(this._tracker.eventId, options).subscribe((event: EventDto) => {
            subs.unsubscribe();
            this._tracker.completeCancellation();
            this.snackBar.open('Event Cancelled', 'close', { duration: this._snackBarDuration });

            this._eventListCache.clearCache(); // todo - this would be tidier is we just remove the single

            // Insurance - to ensure the clear down has completed after the delete.
            setTimeout(() => {
                this.navigationService.removeEventForUser(event);
                this._tracker.reset();
            }, 2000); // note - this matches with the redirect after delete.
        });
    }

    public closeEvent() {

        this._tracker.startClose();

        const subs = this.eventRepo.closeEvent(this._tracker.eventId).subscribe((closedEvent: EventDto) => {
            subs.unsubscribe();
            this._tracker.completeClose();
            this.snackBar.open('Event Closed', 'close', { duration: this._snackBarDuration });
        });
    }

    public removeActivity(activity: ActivityDto) {

        const subs = this.eventRepo.removeActivity(this._tracker.eventId, activity).subscribe((removedActivity: ActivityDto) => {
            subs.unsubscribe();
            this._tracker.deleteActivity(removedActivity);
        });
    }

    public updateEventOverview(eventOverviewUpdate: EventOverviewUpdateDto) {

        const subs = this.eventRepo.updateOverview(this._tracker.eventId, eventOverviewUpdate).subscribe(() => {
            subs.unsubscribe();
            this._tracker.updateOverview(eventOverviewUpdate);
            this.snackBar.open('Event Settings Updated', 'close', { duration: this._snackBarDuration });
            this.navigationService.updateEventNameForUser(this._tracker.eventId, eventOverviewUpdate.eventName);
        });
    }

    public updateEventDates(eventDatesUpdate: EventDatesUpdateDto) {

        const subs = this.eventRepo.updateDates(this._tracker.eventId, eventDatesUpdate).subscribe(() => {
            subs.unsubscribe();
            this._tracker.updateEventDates(eventDatesUpdate);
            this.snackBar.open('Event Dates Updated', 'close', { duration: this._snackBarDuration });
        });
    }

    public updateEmailTemplate(emailTemplateUpdate: EmailTemplateUpdateSubmissionDto) {

        const subs = this.eventRepo.updateEmailTemplate(this._tracker.eventId, emailTemplateUpdate).subscribe(() => {
            subs.unsubscribe();
            this._tracker.updateEmailTemplate(emailTemplateUpdate);
            this.snackBar.open('Email Template Updated', 'close', { duration: this._snackBarDuration });
        });
    }

    public addActivity(activitySubmission: ActivitySubmissionDto) {

        const activityAddObs = this.eventRepo.addActivity(this._tracker.eventId, activitySubmission);

        const subs = activityAddObs.subscribe((newActivity: EventActivityDto) => {
            subs.unsubscribe();
            this._tracker.addActivity(newActivity);
        });

        return activityAddObs;
    }

    public updateActivity(activityUpdate: ActivityUpdateDto) {

        const subs = this.eventRepo.updateActivity(this._tracker.eventId, activityUpdate).subscribe((updatedActivity: EventActivityDto) => {
            subs.unsubscribe();
            this._tracker.updateActivity(updatedActivity);
            this.snackBar.open('Activity Updated', 'close', { duration: this._snackBarDuration });
        });
    }

    public registrationActiveFlowStates(): number[] {
        return [0, 1, 2, 3];
    }

    public registrationInActiveFlowStates(): number[] {
        return [4, 5, 6, 7];
    }

    private mapRegistrationDtoToEventRegistration(registrationDto: EventRegistrationDto): EventRegistration {

        const flowState = this.extractFlowState(registrationDto.flowState);

        const eventRegistration: EventRegistration = {
            ...registrationDto,
            flowStateText: flowState[0],
            flowStateColour: flowState[1]
        };

        return eventRegistration;
    }

    private extractFlowState(registrationFlowStateId: number) {
        switch (registrationFlowStateId) {
            case 0: return ['pending', 'accent'];
            case 1: return ['submitted', 'accent'];
            case 2: return ['confirmed', 'accent'];
            case 3: return ['locked', 'warn'];
            case 4: return ['closed', 'default'];
            case 5: return ['cancelled', 'warn'];
            case 6: return ['deleted', 'warn'];
            case 7: return ['pulled', 'accent'];
            default: return ['unknown', 'warn'];
        }
    }
}

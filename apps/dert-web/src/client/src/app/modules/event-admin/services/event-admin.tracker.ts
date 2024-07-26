
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import {map} from 'rxjs/operators';

import { EventImage, EventOverview } from 'app/models/app';
import { RegistrationFlowState } from 'app/models/app/Enumerations/RegistrationFlowStates';
import {
    ActivityDetailDto,
    ActivityDto,
    ActivityUpdateDto,
    CompetitionSummaryDto,
    EmailTemplateDetailDto,
    EmailTemplateDto,
    EmailTemplateUpdateSubmissionDto,
    EventActivityDto,
    EventCompetitionDto,
    EventDatesUpdateDto,
    EventImageDto,
    EventInvoiceDto,
    EventOverviewUpdateDto,
    EventRegistrationDto,
    GroupTeamDto,
    InvoiceDto
} from 'app/models/dto';

@Injectable()
export class EventAdminTracker {

    private _individualAudienceTypeId = 1;
    private _teamAudienceTypeId = 2;

    private _activeRegistrationStates: number[] = [
        RegistrationFlowState.new,
        RegistrationFlowState.submitted,
        RegistrationFlowState.confirmed,
        RegistrationFlowState.pulled
    ];

    private _eventOverview = new BehaviorSubject<EventOverview>(null);
    private _eventImages = new BehaviorSubject<EventImage[]>([]);
    private _eventInvoices = new BehaviorSubject<EventInvoiceDto[]>([]);
    private _eventRegistrations = new BehaviorSubject<EventRegistrationDto[]>([]);
    private _eventIndividualActivities = new BehaviorSubject<EventActivityDto[]>([]);
    private _eventTeamActivities = new BehaviorSubject<EventActivityDto[]>([]);
    private _focussedActivity = new BehaviorSubject<ActivityDto>(null);
    private _emailTemplates = new BehaviorSubject<EmailTemplateDto[]>([]);
    private _focussedEmailTemplate = new BehaviorSubject<EmailTemplateDetailDto>(null);
    private _focussedInvoiceHistory = new BehaviorSubject<InvoiceDto[]>(null);
    private _competitions = new BehaviorSubject<EventCompetitionDto[]>(null);
    private _downloads = new BehaviorSubject<GroupTeamDto[]>(null);

    get eventOverview$() { return this._eventOverview.asObservable(); }
    get eventImages$() { return this._eventImages.asObservable(); }
    get eventInvoices$() { return this._eventInvoices.asObservable(); }
    get eventRegistrations$() { return this._eventRegistrations.asObservable(); }
    get activeRegistrations$() {
        return this.eventRegistrations$.pipe(map(
            eventRegistrations => eventRegistrations.filter(registration => {
                return this._activeRegistrationStates.indexOf(registration.flowState) > -1;
            })
        ));
    }
    get eventIndividualActivities$() { return this._eventIndividualActivities.asObservable(); }
    get eventTeamActivities$() { return this._eventTeamActivities.asObservable(); }
    get focussedActivity$() { return this._focussedActivity.asObservable(); }
    get emailTemplates$() { return this._emailTemplates.asObservable(); }
    get focussedEmailTemplate$() { return this._focussedEmailTemplate.asObservable(); }
    get focussedInvoiceHistory$() { return this._focussedInvoiceHistory.asObservable(); }
    get competitions$() { return this._competitions.asObservable(); }
    get downloads$() { return this._downloads.asObservable(); }

    private _memoryStore: {
        overview: EventOverview,
        images: EventImageDto[],
        invoices: EventInvoiceDto[],
        registrations: EventRegistrationDto[],
        individualActivities: EventActivityDto[],
        teamActivities: EventActivityDto[],
        activityDetail: { [id: number]: ActivityDetailDto },
        emailTemplates: EmailTemplateDto[],
        emailTemplateDetail: { [id: number]: EmailTemplateDetailDto },
        invoiceHistory: { [registrationId: number]: InvoiceDto[] },
        competitions: EventCompetitionDto[],
        downloads: GroupTeamDto[]
    };

    public get eventId(): number {
        return this._memoryStore.overview.id;
    }

    public get overview(): EventOverview {
        return this._memoryStore.overview;
    }

    public set overview(overview: EventOverview) {
        this._memoryStore.overview = overview;
        this._eventOverview.next(this._memoryStore.overview);
    }

    public set images(images: EventImageDto[]) {
        this._memoryStore.images = images;
        this._eventImages.next(this.MarkSelectedImage(images, this.overview.eventPictureUrl || ''));
    }

    public set invoices(invoices: EventInvoiceDto[]) {
        this._memoryStore.invoices = invoices;
        this._eventInvoices.next(this._memoryStore.invoices);
    }

    public set registrations(registrations: EventRegistrationDto[]) {
        this._memoryStore.registrations = registrations;
        this._eventRegistrations.next(registrations);
    }

    public set individualActivities(individualActivities: EventActivityDto[]) {
        this._memoryStore.individualActivities = individualActivities;
        this._eventIndividualActivities.next(individualActivities);
    }

    public set teamActivities(teamActivities: EventActivityDto[]) {
        this._memoryStore.teamActivities = teamActivities;
        this._eventTeamActivities.next(teamActivities);
    }

    public set emailTemplates(emailTemplates: EmailTemplateDto[]) {
        this._memoryStore.emailTemplates = emailTemplates;
        this._emailTemplates.next(emailTemplates);
    }

    public set competitions(competitions: EventCompetitionDto[]) {
        this._memoryStore.competitions = competitions;
        this._competitions.next(competitions);
    }

    public set downloads(confirmedGroupTeams: GroupTeamDto[]) {
        this._memoryStore.downloads = confirmedGroupTeams;
        this._downloads.next(confirmedGroupTeams);
    }

    constructor() {
        this.reset();
    }

    public hasLoadedOverview(): boolean {
        return this._memoryStore.overview !== null;
    }

    public hasLoadedImages(): boolean {
        return this._memoryStore.images !== null;
    }

    public hasLoadedInvoices(): boolean {
        return this._memoryStore.invoices !== null;
    }

    public hasLoadedRegistrations(): boolean {
        return this._memoryStore.registrations !== null;
    }

    public hasLoadedIndividualActivities(): boolean {
        return this._memoryStore.individualActivities !== null;
    }

    public hasLoadedTeamActivities(): boolean {
        return this._memoryStore.teamActivities !== null;
    }

    public hasEmailTemplates(): boolean {
        return this._memoryStore.emailTemplates !== null;
    }

    public hasActivityDetail(activityId: number): boolean {
        return this._memoryStore.activityDetail !== null && this._memoryStore.activityDetail[activityId] !== undefined;
    }

    public hasEmailTemplateDetail(emailTemplateId: number): boolean {
        return this._memoryStore.emailTemplateDetail !== null && this._memoryStore.emailTemplateDetail[emailTemplateId] !== undefined;
    }

    public hasInvoiceHistoryDetail(invoiceId: number): boolean {
        const invoice = this._memoryStore.invoices.find(i => i.invoiceId === invoiceId);
        return this._memoryStore.emailTemplateDetail !== null && this._memoryStore.invoiceHistory[invoice.registrationId] !== undefined;
    }

    public hasCompetitions(): boolean {
        return this._memoryStore.competitions !== null;
    }

    public hasDownloads(): boolean {
        return this._memoryStore.downloads !== null;
    }

    public reset(): void {
        this._memoryStore = {
            overview: null,
            images: null,
            invoices: null,
            registrations: null,
            individualActivities: null,
            teamActivities: null,
            activityDetail: {},
            emailTemplates: null,
            emailTemplateDetail: {},
            invoiceHistory: {},
            competitions: null,
            downloads: null
        };

        this._eventOverview.next(null);
        this._eventImages.next([]);
        this._eventInvoices.next([]);
        this._eventRegistrations.next([]);
        this._eventIndividualActivities.next([]);
        this._eventTeamActivities.next([]);
        this._focussedActivity.next(null);
        this._emailTemplates.next([]);
        this._focussedEmailTemplate.next(null);
        this._focussedInvoiceHistory.next(null);
        this._competitions.next(null);
        this._downloads.next(null);
    }

    public updateOverview(update: EventOverviewUpdateDto) { // note - should be EventOverviewUpdateSubmissionDto

        this._memoryStore.overview.name = update.eventName;
        this._memoryStore.overview.eventSynopsis = update.eventSynopsis;
        this._memoryStore.overview.contactTelephone = update.contactTelephone;
        this._memoryStore.overview.contactEmail = update.contactEmail;
        this._memoryStore.overview.contactName = update.contactName;
        this._memoryStore.overview.locationTown = update.locationTown;
        this._memoryStore.overview.locationPostcode = update.locationPostcode;
        this._memoryStore.overview.visibility = update.visibility;
        this._memoryStore.overview.sentEmailsBcc = update.sentEmailsBcc;

        this._eventOverview.next(this._memoryStore.overview);
    }

    public updateEventDates(update: EventDatesUpdateDto) {

        this._memoryStore.overview.eventStartDate = update.eventStartDate;
        this._memoryStore.overview.eventEndDate = update.eventEndDate;
        this._memoryStore.overview.registrationOpenDate = update.registrationOpenDate;
        this._memoryStore.overview.registrationCloseDate = update.registrationCloseDate;

        this._eventOverview.next(this._memoryStore.overview);
    }

    public addActivity(activity: EventActivityDto) {

        switch (activity.audienceTypeId) {
            case this._individualAudienceTypeId:
                this._memoryStore.individualActivities.push(activity);
                this._eventIndividualActivities.next(this._memoryStore.individualActivities);
                break;
            case this._teamAudienceTypeId:
                this._memoryStore.teamActivities.push(activity);
                this._eventTeamActivities.next(this._memoryStore.teamActivities);
                break;
            default:
                throw new Error('Cannot add activity with no audience');
        }
    }

    public addActivityDetail(activityDetail: ActivityDetailDto, setFocussed: boolean): void {
        this._memoryStore.activityDetail[activityDetail.id] = activityDetail;

        if (setFocussed) {
            this._focussedActivity.next(this._memoryStore.activityDetail[activityDetail.id]);
        }
    }

    public addInvoiceHistory(invoiceHistory: InvoiceDto[], setFocussed: boolean): void {

        if (invoiceHistory.length > 0) {
            this._memoryStore.invoiceHistory[invoiceHistory[0].registrationId] = invoiceHistory;

            if (setFocussed) {
                this._focussedInvoiceHistory.next(this._memoryStore.invoiceHistory[invoiceHistory[0].registrationId]);
            }
        }
    }

    public updateActivity(update: ActivityUpdateDto) {

        // Get and update the listing activity
        const originallyIndividual = this._memoryStore.individualActivities.find(ta => ta.id === update.id);
        const originallyTeam = this._memoryStore.teamActivities.find(ta => ta.id === update.id);
        const activity = originallyIndividual || originallyTeam;

        const hasChangedType = activity.audienceTypeId !== update.audienceTypeId;

        activity.title = update.title;
        activity.description = update.description;
        activity.price = update.price;
        activity.audienceTypeId = update.audienceTypeId;
        activity.isDefault = update.isDefault;
        activity.priceTBC = update.priceTBC;

        if (hasChangedType && originallyIndividual) {
            const index = this._memoryStore.individualActivities.indexOf(originallyIndividual);
            this._memoryStore.individualActivities.splice(index, 1);
            this._memoryStore.teamActivities.push(activity);
        }

        if (hasChangedType && originallyTeam) {
            const index = this._memoryStore.teamActivities.indexOf(originallyTeam);
            this._memoryStore.teamActivities.splice(index, 1);
            this._memoryStore.individualActivities.push(activity);
        }

        if (hasChangedType) {
            this._eventIndividualActivities.next(this._memoryStore.individualActivities);
            this._eventTeamActivities.next(this._memoryStore.teamActivities);
        } else {
            if (originallyIndividual) { this._eventIndividualActivities.next(this._memoryStore.individualActivities); }
            if (originallyTeam) { this._eventTeamActivities.next(this._memoryStore.teamActivities); }
        }

        // Update the focussed activity
        if (this._focussedActivity.value && this._focussedActivity.value.id === update.id) {
            const focussedActivity = this._focussedActivity.value;
            focussedActivity.title = update.title;
            focussedActivity.description = update.description;
            focussedActivity.price = update.price;
            focussedActivity.audienceTypeId = update.audienceTypeId;
            focussedActivity.isDefault = update.isDefault;

            this._focussedActivity.next(focussedActivity);
        }
    }

    public deleteActivity(activity: ActivityDto) {
        const isIndividual = this._memoryStore.individualActivities.find(ta => ta.id === activity.id);
        const isTeam = this._memoryStore.teamActivities.find(ta => ta.id === activity.id);

        if (isIndividual) {
            const index = this._memoryStore.individualActivities.indexOf(isIndividual);
            this._memoryStore.individualActivities.splice(index, 1);
            this._eventIndividualActivities.next(this._memoryStore.individualActivities);
        }

        if (isTeam) {
            const index = this._memoryStore.teamActivities.indexOf(isTeam);
            this._memoryStore.teamActivities.splice(index, 1);
            this._eventTeamActivities.next(this._memoryStore.teamActivities);
        }
    }

    public updateEmailTemplate(emailTemplateUpdate: EmailTemplateUpdateSubmissionDto) {

        // Update the list
        const listItem = this._memoryStore.emailTemplates.find(et => et.id === emailTemplateUpdate.id);
        listItem.templateName = emailTemplateUpdate.templateName;
        listItem.subject = emailTemplateUpdate.subject;

        // Update the detail
        const detailItem = this._memoryStore.emailTemplateDetail[emailTemplateUpdate.id];
        detailItem.templateName = emailTemplateUpdate.templateName;
        detailItem.subject = emailTemplateUpdate.subject;
        detailItem.body = emailTemplateUpdate.body;

        this._emailTemplates.next(this._memoryStore.emailTemplates);
        this._focussedEmailTemplate.next(this._memoryStore.emailTemplateDetail[emailTemplateUpdate.id]);
    }

    public addEmailTemplateDetail(emailTemplateDetail: EmailTemplateDetailDto, setFocussed: boolean): void {
        this._memoryStore.emailTemplateDetail[emailTemplateDetail.id] = emailTemplateDetail;

        if (setFocussed) {
            this._focussedEmailTemplate.next(this._memoryStore.emailTemplateDetail[emailTemplateDetail.id]);
        }
    }

    public deleteImage(eventImageId: number) {

        const removalIndex = this._memoryStore.images.findIndex(i => i.eventImageId === eventImageId);
        const removedImage = this._memoryStore.images.splice(removalIndex, 1);
        this._eventImages.next(this.MarkSelectedImage(this._memoryStore.images, this.overview.eventPictureUrl || ''));
    }

    public focusActivityDetail(activityId: number) {
        this._focussedActivity.next(this._memoryStore.activityDetail[activityId]);
    }

    public focusInvoiceHistory(invoiceId: number) {

        const invoice = this._memoryStore.invoices.find(i => i.invoiceId === invoiceId);
        this._focussedInvoiceHistory.next(this._memoryStore.invoiceHistory[invoice.registrationId]);
    }

    public setEventPrimaryImage(imageResourceUri: string) {
        this._memoryStore.overview.eventPictureUrl = imageResourceUri;
        this._eventOverview.next(this._memoryStore.overview);
        this._eventImages.next(this.MarkSelectedImage(this._memoryStore.images, imageResourceUri || ''));
    }

    public startDelete() {
        this._memoryStore.overview.isDeleting = true;
        this._eventOverview.next(this._memoryStore.overview);
    }

    public completeDelete() {
        this._memoryStore.overview.isDeleted = true;
        this._eventOverview.next(this._memoryStore.overview);
    }

    public startCancellation() {
        this._memoryStore.overview.isCancelling = true;
        this._eventOverview.next(this._memoryStore.overview);
    }

    public completeCancellation() {
        this._memoryStore.overview.isCancelled = true;
        this._eventOverview.next(this._memoryStore.overview);
    }

    public startClose() {
        this._memoryStore.overview.isClosing = true;
        this._eventOverview.next(this._memoryStore.overview);
    }

    public completeClose() {
        this._memoryStore.overview.isClosed = true;
        this._eventOverview.next(this._memoryStore.overview);
    }

    public toggleInvoicePaid(invoiceId: number, forceRefresh: boolean = false): boolean {
        const invoice = this._memoryStore.invoices.find(i => i.invoiceId === invoiceId);
        invoice.hasPaid = !invoice.hasPaid;

        const invoiceHistory = this._memoryStore.invoiceHistory[invoice.registrationId];

        if (invoiceHistory !== null && invoiceHistory !== undefined) {
            const historyInvoiceIndex = invoiceHistory.findIndex(i => i.invoiceId === invoiceId);
            invoiceHistory[historyInvoiceIndex].hasPaid = !invoiceHistory[historyInvoiceIndex].hasPaid;

            const currentFocussedHistory = this._focussedInvoiceHistory.value;
            if (
                currentFocussedHistory &&
                currentFocussedHistory.length > 0
                && currentFocussedHistory[0].registrationId === invoice.registrationId
            ) {
                this._focussedInvoiceHistory.next(this._memoryStore.invoiceHistory[invoice.registrationId]);
            }
        }

        this._eventOverview.next(this._memoryStore.overview);

        return invoice.hasPaid;
    }

    private MarkSelectedImage(images: EventImageDto[], currentUri: string): EventImage[] {
        return images.map(i => {
            return {
                ...i,
                isPrimary: i.imageResourceUri === currentUri
            };
        });
    }
}

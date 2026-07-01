
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError, map, share } from 'rxjs/operators';

import { RepositoryBase } from 'app/core/base/repository.base';
import { AppInsightsService } from 'app/core/logging/appinsights.service';
import { EventRegistrationOverview, GroupRegistrationOverview } from 'app/models/app';
import {
    ActivityDto,
    ActivityMemberAttendanceDto,
    ActivityTeamAttendanceDto,
    ContactInfoDto,
    EmailRegistrationConfirmationDataDto,
    EventRegistrationOverviewDto,
    GroupRegistrationOverviewDto,
    MemberAttendanceDto,
    RegistrationConfirmSubmissionDto,
    RegistrationDto,
    RegistrationMemberAttendanceAttachActivitiesSubmissionDto,
    RegistrationMemberAttendanceDeleteSubmissionDto,
    RegistrationMemberAttendanceDetachActivitiesSubmissionDto,
    RegistrationMemberAttendanceSubmissionDto,
    RegistrationSubmitSubmissionDto,
    RegistrationTeamAttendanceAttachActivitiesSubmissionDto,
    RegistrationTeamAttendanceDeleteSubmissionDto,
    RegistrationTeamAttendanceDetachActivitiesSubmissionDto,
    RegistrationTeamAttendanceSubmissionDto,
    TeamAttendanceDto
} from 'app/models/dto';
import { ConfigurationService } from 'app/core/services/configuration.service';

@Injectable()
export class RegistrationRepository extends RepositoryBase {

    constructor(http: HttpClient, configurationService: ConfigurationService, appInsightsService: AppInsightsService) {
        super(http, configurationService, appInsightsService);
    }

    /**
     * Returns the event information containing the contact details for the event management interface.
     * Protection at the Api to "Authenticated Users"
     */
    public get(registrationId: number): Observable<RegistrationDto> {
        const url = this.ApiUri + '/registration/' + registrationId.toString();

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    /**
     * Gets the basic inforamtion for the group related to the specified registration
     * @param registrationId the Id of the registration
     */
    public getGroupRegistrationOverview(registrationId: number): Observable<GroupRegistrationOverview> {
        const url = this.ApiUri + '/registration/' + registrationId.toString() + '/group-overview';

        return this.http.get(url).pipe(
            share(),
            map((data) => this.groupRegistrationOverviewDtoMap(data)),
            catchError((err) => this.processApiError(err)));
    }

    /**
     * Gets the basic information for the event related to the specifed registration
     * @param registrationId the Id of the registration
     */
    public getEventRegistrationOverview(registrationId: number): Observable<EventRegistrationOverview> {
        const url = this.ApiUri + '/registration/' + registrationId.toString() + '/event-overview';

        return this.http.get(url).pipe(
            share(),
            map((data) => this.eventRegistrationOverviewDtoMap(data)),
            catchError((err) => this.processApiError(err)));
    }

    /**
     * Gets a list of the members that are attached to the specified registration
     * @param registrationId the Id of the registration
     */
    public getAttendingIndividuals(registrationId: number): Observable<MemberAttendanceDto[]> {
        const url = this.ApiUri + '/registration/' + registrationId.toString() + '/attending-individuals';

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    /**
     * Gets a list of the teams that are attached to the specified registration
     * @param registrationId the Id of the registration
     */
    public getAttendingTeams(registrationId: number): Observable<TeamAttendanceDto[]> {
        const url = this.ApiUri + '/registration/' + registrationId.toString() + '/attending-teams';

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    public getConfirmationEmailData(registrationId: number): Observable<EmailRegistrationConfirmationDataDto> {
        const url = this.ApiUri + '/registration/' + registrationId.toString() + '/confirmation-email-data';

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    public getGroupContactInfo(registrationId: number): Observable<ContactInfoDto> {
        const url = this.ApiUri + '/registration/' + registrationId.toString() + '/contactinfo/group';

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    public getEventContactInfo(registrationId: number): Observable<ContactInfoDto> {
        const url = this.ApiUri + '/registration/' + registrationId.toString() + '/contactinfo/event';

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    public getTeamActivities(registrationId: number): Observable<ActivityDto[]> {
        const url = this.ApiUri + '/registration/' + registrationId.toString() + '/team-activities';  // URL to web API

        return this.http.get(url).pipe(
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    public getIndividualActivities(registrationId: number): Observable<ActivityDto[]> {
        const url = this.ApiUri + '/registration/' + registrationId.toString() + '/individual-activities';  // URL to web API

        return this.http.get(url).pipe(
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    /**
     * Adds new memberAttendance to the registration.
     * This call creates (or finds) the member and attaches them the the registration.
     * @param registrationId the Id of the registration
     * @param memberAttendanceSubmission the information about a current or new member to create the memberAttendance
     */
    public addMemberAttendance(
        registrationId: number,
        memberAttendanceSubmission: RegistrationMemberAttendanceSubmissionDto
    ): Observable<MemberAttendanceDto> {
        const url = this.ApiUri + '/registration/' + registrationId.toString() + '/attending-individual';

        return this.http.post(url, memberAttendanceSubmission).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    public batchAddMemberAttendances(
        registrationId: number,
        memberAttendancesSubmission: RegistrationMemberAttendanceSubmissionDto[]
    ): Observable<MemberAttendanceDto[]> {
        const url = this.ApiUri + '/registration/' + registrationId.toString() + '/attending-individuals';

        return this.http.post(url, memberAttendancesSubmission).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    /**
     * Adds new teamAttendance to the registration
     * This call creates (or finds) the team and attaches them the the registration.
     * If the team does not exist it is created and attached to the group.
     * @param registrationId the Id of the registration
     * @param teamAttendanceSubmission the information about a current or new team to create the teamAttendance
     */
    public addTeamAttendance(
        registrationId: number,
        teamAttendanceSubmission: RegistrationTeamAttendanceSubmissionDto
    ): Observable<TeamAttendanceDto> {
        const url = this.ApiUri + '/registration/' + registrationId.toString() + '/attending-team';

        return this.http.post(url, teamAttendanceSubmission).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    public batchAddTeamAttendances(
        registrationId: number,
        teamAttendancesSubmission: RegistrationTeamAttendanceSubmissionDto[]
    ): Observable<TeamAttendanceDto[]> {
        const url = this.ApiUri + '/registration/' + registrationId.toString() + '/attending-teams';

        return this.http.post(url, teamAttendancesSubmission).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    public removeMemberAttendance(registrationId: number, memberAttendanceId: number): Observable<MemberAttendanceDto> {

        const url = `${this.ApiUri}/registration/${registrationId.toString()}/attending-individual/${memberAttendanceId.toString()}`;

        return this.http.delete(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    public removeTeamAttendance(registrationId: number, teamAttendanceId: number): Observable<TeamAttendanceDto> {

        const url = `${this.ApiUri}/registration/${registrationId.toString()}/attending-team/${teamAttendanceId.toString()}`;

        return this.http.delete(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    public removeMemberAttendances(
        registrationId: number,
        deleteSubmission: RegistrationMemberAttendanceDeleteSubmissionDto
    ): Observable<MemberAttendanceDto[]> {

        const url = `${this.ApiUri}/registration/${registrationId.toString()}/attending-individuals/delete`;

        return this.http.post(url, deleteSubmission).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    public removeTeamAttendances(
        registrationId: number,
        deleteSubmission: RegistrationTeamAttendanceDeleteSubmissionDto
    ): Observable<TeamAttendanceDto[]> {

        const url = `${this.ApiUri}/registration/${registrationId.toString()}/attending-teams/delete`;

        return this.http.post(url, deleteSubmission).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    public batchAddMemberAttendanceActivities(
        registrationId: number,
        memberAttendanceId: number,
        addMemberAttendanceSubmissionDto: RegistrationMemberAttendanceAttachActivitiesSubmissionDto
    ): Observable<ActivityMemberAttendanceDto[]> {

        const url = `${this.ApiUri}/registration/${registrationId}/attending-individual/${memberAttendanceId}/activities`;

        return this.http.post(url, addMemberAttendanceSubmissionDto).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    public batchAddTeamAttendanceActivities(
        registrationId: number,
        teamAttendanceId: number,
        addTeamAttendanceSubmissionDto: RegistrationTeamAttendanceAttachActivitiesSubmissionDto
    ): Observable<ActivityTeamAttendanceDto[]> {

        const url = `${this.ApiUri}/registration/${registrationId}/attending-team/${teamAttendanceId}/activities`;

        return this.http.post(url, addTeamAttendanceSubmissionDto).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    public batchRemoveMemberAttendanceActivities(
        registrationId: number,
        memberAttendanceId: number,
        removeMemberAttendanceSubmissionDto: RegistrationMemberAttendanceDetachActivitiesSubmissionDto
    ): Observable<ActivityMemberAttendanceDto[]> {

        const url = `${this.ApiUri}/registration/${registrationId}/attending-individual/${memberAttendanceId}/activities/delete`;

        return this.http.post(url, removeMemberAttendanceSubmissionDto).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    public batchRemoveTeamAttendanceActivities(
        registrationId: number,
        teamAttendanceId: number,
        removeTeamAttendanceSubmissionDto: RegistrationTeamAttendanceDetachActivitiesSubmissionDto
    ): Observable<ActivityTeamAttendanceDto[]> {

        const url = `${this.ApiUri}/registration/${registrationId}/attending-team/${teamAttendanceId}/activities/delete`;

        return this.http.post(url, removeTeamAttendanceSubmissionDto).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    public submitRegistration(registrationId: number, registrationSubmitSubmissionDto: RegistrationSubmitSubmissionDto) {

        const url = `${this.ApiUri}/registration/${registrationId}/submit`;

        return this.http.put(url, registrationSubmitSubmissionDto).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    public confirmRegistration(registrationId: number, registrationConfirmSubmissionDto: RegistrationConfirmSubmissionDto) {

        const url = `${this.ApiUri}/registration/${registrationId}/confirm`;

        return this.http.put(url, registrationConfirmSubmissionDto).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    public getUpdatedPriceForRegistration(registrationId: number): any {
        const url = `${this.ApiUri}/registration/${registrationId}/current-price`;

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    private registrationDtoMap(data: any) {
        const registrationDto: RegistrationDto = data;

        return registrationDto;
    }

    private groupRegistrationOverviewDtoMap(data: any): GroupRegistrationOverview {
        const groupRegistrationOverviewDto: GroupRegistrationOverviewDto = data;

        const groupRegistrationOverview: GroupRegistrationOverview = {
            ...groupRegistrationOverviewDto,
            isDeleting: false,
            isDeleted: false,
            flowStateText: '', // this is redundant here - refactor required
            flowStateColour: '' // this is redundant here - refactor required
        };

        return groupRegistrationOverview;
    }

    private eventRegistrationOverviewDtoMap(data: any): EventRegistrationOverview {
        const eventRegistrationOverviewDto: EventRegistrationOverviewDto = data;

        const eventRegistrationOverview: EventRegistrationOverview = {
            ...eventRegistrationOverviewDto,
            isDeleting: false,
            isDeleted: false,
            flowStateText: '', // this is redundant here - refactor required
            flowStateColour: '' // this is redundant here - refactor required
        };

        return eventRegistrationOverview;
    }

    private memberAttendanceDtoMap(data: any): MemberAttendanceDto[] {
        const memberAttendanceDtos: MemberAttendanceDto[] = data;

        return memberAttendanceDtos;
    }

    private teamAttendanceDtoMap(data: any): TeamAttendanceDto[] {
        const teamAttendanceDtos: TeamAttendanceDto[] = data;

        return teamAttendanceDtos;
    }
}


import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { RepositoryBase } from 'app/core/base/repository.base';
import { AppInsightsService } from 'app/core/logging/appinsights.service';
import { ConfigurationService } from 'app/core/services/configuration.service';
import { GroupImageDto } from 'app/models/dto/GroupImageDto';
import { GroupInvoiceDto } from 'app/models/dto/GroupInvoiceDto';
import { GroupMemberDetailDto } from 'app/models/dto/GroupMemberDetailDto';
import { GroupMemberDto } from 'app/models/dto/GroupMemberDto';
import { GroupMemberSubmissionDto } from 'app/models/dto/GroupMemberSubmissionDto';
import { GroupMemberUpdateDto } from 'app/models/dto/GroupMemberUpdateDto';
import { GroupOverviewDto } from 'app/models/dto/GroupOverviewDto';
import { GroupOverviewUpdateDto } from 'app/models/dto/GroupOverviewUpdateDto';
import { GroupRegistrationDto } from 'app/models/dto/GroupRegistrationDto';
import { GroupTeamDetailDto } from 'app/models/dto/GroupTeamDetailDto';
import { GroupTeamDto } from 'app/models/dto/GroupTeamDto';
import { GroupTeamSubmissionDto } from 'app/models/dto/GroupTeamSubmissionDto';
import { GroupTeamUpdateDto } from 'app/models/dto/GroupTeamUpdateDto';
import { TeamImageSubmissionDto } from 'app/models/dto/TeamImageSubmissionDto';
import { Observable } from 'rxjs';
import { catchError, map, share } from 'rxjs/operators';

@Injectable()
export class GroupAdminRepository extends RepositoryBase {

    constructor(http: HttpClient, configurationService: ConfigurationService, appInsightsService: AppInsightsService) {
        super(http, configurationService, appInsightsService);
    }

    overview(groupId: number): Observable<GroupOverviewDto> {
        const url = this.ApiUri + '/group/' + groupId.toString() + '/overview';  // URL to web API

        return this.http.get(url).pipe(
            share(),
            map((data) => this.groupOverviewDtoMap(data)),
            catchError((err) => this.processApiError(err)));
    }

    members(groupId: number): Observable<GroupMemberDto[]> {
        const url = this.ApiUri + '/group/' + groupId.toString() + '/members';  // URL to web API

        return this.http.get(url).pipe(
            share(),
            map((data) => this.groupMemberDtoMap(data)),
            catchError((err) => this.processApiError(err)));
    }

    images(groupId: number): Observable<GroupImageDto[]> {
        const url = this.ApiUri + '/group/' + groupId.toString() + '/images';  // URL to web API

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    invoices(groupId: number): Observable<GroupInvoiceDto[]> {
        const url = this.ApiUri + '/group/' + groupId.toString() + '/invoices';  // URL to web API

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    teams(groupId: number): Observable<GroupTeamDto[]> {
        const url = this.ApiUri + '/group/' + groupId.toString() + '/teams';  // URL to web API

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    registrations(groupId: number): Observable<GroupRegistrationDto[]> {
        const url = this.ApiUri + '/group/' + groupId.toString() + '/registrations';  // URL to web API

        return this.http.get(url).pipe(
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    memberDetail(groupId: number, groupMemberId: number): Observable<GroupMemberDetailDto> {
        const url = this.ApiUri + '/group/' + groupId.toString() + '/members/' + groupMemberId.toString();  // URL to web API

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    teamDetail(groupId: number, groupTeamId: number): Observable<GroupTeamDetailDto> {
        const url = this.ApiUri + '/group/' + groupId.toString() + '/teams/' + groupTeamId.toString();  // URL to web API

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    deleteImage(groupImage: GroupImageDto) {
        const url = this.ApiUri + '/group/' + groupImage.groupId.toString() + '/groupimage/' + groupImage.groupImageId;  // URL to web API

        return this.http.delete(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    addMember(groupId: number, groupMemberSubmission: GroupMemberSubmissionDto) {
        const url = this.ApiUri + '/group/' + groupId + '/members';  // URL to web API

        return this.http.post(url, groupMemberSubmission).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    addTeam(groupId: number, groupTeamSubmission: GroupTeamSubmissionDto) {
        const url = this.ApiUri + '/group/' + groupId + '/teams';  // URL to web API

        return this.http.post(url, groupTeamSubmission).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    // note - update calls pass the id of the items to update in the body of the submission.
    updateMember(groupId: number, groupMemberUpdate: GroupMemberUpdateDto) {
        const url = this.ApiUri + '/group/' + groupId + '/members';  // URL to web API

        return this.http.put(url, groupMemberUpdate).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    // note - update calls pass the id of the items to update in the body of the submission.
    updateTeam(groupId: number, groupTeamUpdate: GroupTeamUpdateDto) {
        const url = this.ApiUri + '/group/' + groupId + '/teams';  // URL to web API

        return this.http.put(url, groupTeamUpdate).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    updateOverview(groupId: number, groupOverviewUpdate: GroupOverviewUpdateDto) {
        const url = this.ApiUri + '/group/' + groupId;  // URL to web API

        return this.http.put(url, groupOverviewUpdate).pipe(
            share(),
            catchError((err) => this.processApiError(err)));
    }

    removeMember(groupId: number, groupMember: GroupMemberDto) {
        const url = this.ApiUri + '/group/' + groupId + '/members/' + groupMember.groupMemberId;  // URL to web API

        return this.http.delete(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    removeTeam(groupId: number, groupTeam: GroupTeamDto) {
        const url = this.ApiUri + '/group/' + groupId + '/teams/' + groupTeam.teamId;  // URL to web API

        return this.http.delete(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    deleteGroup(groupId: number) {
        const url = this.ApiUri + '/group/' + groupId;  // URL to web API

        return this.http.delete(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    setPrimaryImage(groupImage: GroupImageDto) {
        const url = `${this.ApiUri}/group/${groupImage.groupId.toString()}/groupimage/${groupImage.groupImageId}/setprimary`;

        return this.http.put(url, groupImage).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    attachTeamImage(groupId: number, teamImage: TeamImageSubmissionDto) {
        const url = this.ApiUri + '/group/' + groupId + '/teamimage';  // URL to web API

        return this.http.post(url, teamImage).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    hasEnteredDertOfDerts(groupId: number) {
        const url = this.ApiUri + `/dodsubmissionauth/entered/${groupId}`;  // URL to web API

        return this.http.get(url).pipe(
            share(),
            map((data) => data as boolean),
            catchError((err) => this.processApiError(err)));
    }

    private groupOverviewDtoMap(data: any) {
        const groupOverviewDto: GroupOverviewDto = data;

        // Manipulate the image paths to gain the correct sizes - this should be done using a pipe.

        return groupOverviewDto;
    }

    private groupMemberDtoMap(data: any) {
        const groupMemberDtos: GroupMemberDto[] = data;

        return groupMemberDtos.map(gm => {
            gm['photo'] = '/assets/images/face-1.jpg';
            return gm;
        });
    }

}

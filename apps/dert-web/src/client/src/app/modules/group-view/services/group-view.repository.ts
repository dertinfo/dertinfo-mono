import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { RepositoryBase } from 'app/core/base/repository.base';
import { AppInsightsService } from 'app/core/logging/appinsights.service';
import { ConfigurationService } from 'app/core/services/configuration.service';
import { DodGroupResultsDto } from 'app/models/dto/DodGroupResultsDto';
import { DodResultComplaintDto } from 'app/models/dto/DodResultComplaintDto';
import { DodResultComplaintSubmissionDto } from 'app/models/dto/DodResultComplaintSubmissionDto';
import { GroupDto } from 'app/models/dto/GroupDto';
import { GroupRegistrationDto } from 'app/models/dto/GroupRegistrationDto';
import { Observable } from 'rxjs';
import { catchError, map, share } from 'rxjs/operators';

@Injectable()
export class GroupViewRepository extends RepositoryBase {

    constructor(http: HttpClient, configurationService: ConfigurationService, appInsightsService: AppInsightsService) {
        super(http, configurationService, appInsightsService);
    }

    getDodResults(groupId: number): Observable<DodGroupResultsDto> {
        const url = this.ApiUri + `/dodresultauth/scorecards/group/${groupId}`;  // URL to web API
        // this._environmentService.getApiBaseUrl()

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    raiseResultComplaint(groupId: number, resultComplaintSubmission: DodResultComplaintSubmissionDto): Observable<boolean> {
        const url = this.ApiUri + `/dodresultauth/group/${groupId}/addcomplaint`;  // URL to web API

        return this.http.post(url, resultComplaintSubmission).pipe(
            share(),
            map((data) => data as DodResultComplaintDto),
            catchError((err) => this.processApiError(err)));
    }

    hasEnteredDertOfDerts(groupId: number) {
        const url = this.ApiUri + `/dodsubmissionauth/entered/${groupId}`;  // URL to web API

        return this.http.get(url).pipe(
            share(),
            map((data) => data as boolean),
            catchError((err) => this.processApiError(err)));
    }

    get(groupId: number): Observable<GroupDto> {
        const url = this.ApiUri + '/group/' + groupId.toString();  // URL to web API

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
}

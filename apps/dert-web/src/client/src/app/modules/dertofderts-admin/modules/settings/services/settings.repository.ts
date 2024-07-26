import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

import { RepositoryBase } from 'app/core/base/repository.base';
import { AppInsightsService } from 'app/core/logging/appinsights.service';
import { DertOfDertSettingsDto } from 'app/models/dto/DertOfDertSettingsDto';
import { DertOfDertSettingsUpdateSubmissionDto } from 'app/models/dto/DertOfDertSettingsUpdateSubmissionDto';
import { ConfigurationService } from 'app/core/services/configuration.service';
@Injectable()
export class SettingsRepository extends RepositoryBase {

    constructor( http: HttpClient, configurationService: ConfigurationService, appInsightsService: AppInsightsService) {
        super(http, configurationService, appInsightsService);
    }

    public getSettings(): Observable<DertOfDertSettingsDto> {
        const url = this.ApiUri + '/dodsettingauth';

        return this.http.get(url).pipe(
            map((data) => data as DertOfDertSettingsDto),
            catchError((err) => this.processApiError(err)));
    }

    public updatePublicResultsForwarded(value: boolean): Observable<boolean> {
        const url = this.ApiUri + '/dodsettingauth/publicresultsforwarded';

        const update: DertOfDertSettingsUpdateSubmissionDto = {
            booleanValue: value,
            stringValue: null
        };

        return this.http.put(url, update).pipe(
            map((data) => data as boolean),
            catchError((err) => this.processApiError(err)));
    }

    public updateOfficialResultsForwarded(value: boolean): Observable<boolean> {
        const url = this.ApiUri + '/dodsettingauth/officialresultsforwarded';

        const update: DertOfDertSettingsUpdateSubmissionDto = {
            booleanValue: value,
            stringValue: null
        };

        return this.http.put(url, update).pipe(
            map((data) => data as boolean),
            catchError((err) => this.processApiError(err)));
    }

    public updateResultsPublished(value: boolean): Observable<boolean> {
        const url = this.ApiUri + '/dodsettingauth/resultspublished';

        const update: DertOfDertSettingsUpdateSubmissionDto = {
            booleanValue: value,
            stringValue: null
        };

        return this.http.put(url, update).pipe(
            map((data) => data as boolean),
            catchError((err) => this.processApiError(err)));
    }

    public updateOpenToPublic(value: boolean): Observable<boolean> {
        const url = this.ApiUri + '/dodsettingauth/opentopublic';

        const update: DertOfDertSettingsUpdateSubmissionDto = {
            booleanValue: value,
            stringValue: null
        };

        return this.http.put(url, update).pipe(
            map((data) => data as boolean),
            catchError((err) => this.processApiError(err)));
    }

    public updateValidJudgePasswords(value: string): Observable<boolean> {
        const url = this.ApiUri + '/dodsettingauth/validjudgepasswords';

        const update: DertOfDertSettingsUpdateSubmissionDto = {
            booleanValue: null,
            stringValue: value
        };

        return this.http.put(url, update).pipe(
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    public clearResultsCache(): Observable<boolean> {
        const url = this.ApiUri + '/dodresultauth/clearcache';

        return this.http.get(url).pipe(
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

}

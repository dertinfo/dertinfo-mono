
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError, map, share } from 'rxjs/operators';

import { RepositoryBase } from 'app/core/base/repository.base';
import { AppInsightsService } from 'app/core/logging/appinsights.service';
import { UserGdprInformationSubmissionDto, UserSettingsUpdateSubmissionDto } from 'app/models/dto';
import { ConfigurationService } from 'app/core/services/configuration.service';

@Injectable()
export class UserRepository extends RepositoryBase {

    constructor(http: HttpClient, configurationService: ConfigurationService, appInsightsService: AppInsightsService) {
        super(http, configurationService, appInsightsService);
    }

    overview(): Observable<any> {
        const url = this.ApiUri + '/user/overview';  // URL to web API

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    // settings(): Observable<any> {
    //     const url = this.ApiUri + '/user/settings';  // URL to web API

    //     return this.http.get(url)
    //         .share()
    //         .map((data) => data)
    //         .catch((err) => this.processApiError(err));
    // }

    updateUserSettings(userSettingsUpdateSubmissionDto: UserSettingsUpdateSubmissionDto): Observable<any> {
        const url = this.ApiUri + '/user/settings';  // URL to web API

        return this.http.put(url, userSettingsUpdateSubmissionDto).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    updateGdprInformation(userGdprInformationSubmissionDto: UserGdprInformationSubmissionDto) {
        const url = this.ApiUri + '/user/gdprinformation';  // URL to web API

        return this.http.post(url, userGdprInformationSubmissionDto).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    deleteAccount() {
        const url = this.ApiUri + '/user/deleteaccount';  // URL to web API

        return this.http.delete(url).pipe(
            share(),
            catchError((err) => this.processApiError(err)));
    }
}

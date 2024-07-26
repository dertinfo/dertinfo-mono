import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { RepositoryBase } from 'app/core/base/repository.base';
import { AppInsightsService } from 'app/core/logging/appinsights.service';
import { ConfigurationService } from 'app/core/services/configuration.service';
import { GroupMinimalSubmissionDto } from 'app/models/dto/GroupMinimalSubmissionDto';
import { catchError, map, share } from 'rxjs/operators';

@Injectable()
export class DashboardRepository extends RepositoryBase {

    constructor(http: HttpClient, configurationService: ConfigurationService, appInsightsService: AppInsightsService) {
        super(http, configurationService, appInsightsService);
    }

    createMinimal(groupSubmission: GroupMinimalSubmissionDto) {
        const url = this.ApiUri + '/group/minimal';  // URL to web API

        return this.http.post(url, groupSubmission).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }
}

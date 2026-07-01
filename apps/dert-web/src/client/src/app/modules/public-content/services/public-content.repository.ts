
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError, map, share } from 'rxjs/operators';

import { RepositoryBase } from 'app/core/base/repository.base';
import { AppInsightsService } from 'app/core/logging/appinsights.service';
import { ConfigurationService } from 'app/core/services/configuration.service';
import { CompetitionLookupDto } from 'app/models/dto';

@Injectable()
export class PublicContentRepository extends RepositoryBase {

    constructor(http: HttpClient, configurationService: ConfigurationService, appInsightsService: AppInsightsService) {
        super(http, configurationService, appInsightsService);
    }

    getResultsLookup(): Observable<CompetitionLookupDto> {
        const url = this.ApiUri + `/results/lookup`;  // URL to web API

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    getResultsByCompetitionAndActivity(competitionId: number, resultType: string) {
        const url = this.ApiUri + `/results/${competitionId}/${resultType}`;  // URL to web API

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }
}

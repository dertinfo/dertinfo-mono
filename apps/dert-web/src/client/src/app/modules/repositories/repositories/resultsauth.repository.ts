
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError, map, share } from 'rxjs/operators';

import { RepositoryBase } from 'app/core/base/repository.base';
import { AppInsightsService } from 'app/core/logging/appinsights.service';
import { DanceDetailDto } from 'app/models/dto';
import { ConfigurationService } from 'app/core/services/configuration.service';

@Injectable()
export class ResultsAuthRepository extends RepositoryBase {

    constructor(http: HttpClient, configurationService: ConfigurationService, appInsightsService: AppInsightsService) {
        super(http, configurationService, appInsightsService);
    }

    getResultsByCompetitionAndTeam(competitionId: number, teamId: number): Observable<Array<DanceDetailDto>> {
        const url = this.ApiUri + `/resultsauth/team/${teamId}/${competitionId}`;  // URL to web API

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }
}

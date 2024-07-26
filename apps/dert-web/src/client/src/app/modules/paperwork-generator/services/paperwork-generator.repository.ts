
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError, map, share} from 'rxjs/operators';

import { RepositoryBase } from 'app/core/base/repository.base';
import { AppInsightsService } from 'app/core/logging/appinsights.service';
import { ConfigurationService } from 'app/core/services/configuration.service';
import { ScoreSheetDto } from 'app/models/dto';

@Injectable()
export class PaperworkGeneratorRepository extends RepositoryBase {

    constructor( http: HttpClient, configurationService: ConfigurationService, appInsightsService: AppInsightsService) {
        super(http, configurationService, appInsightsService);
    }

    /**
     * Returns the overview of the competition information
     */
    scoresheetsPopulated(competitionId: number): Observable<Array<ScoreSheetDto>> {
        const url = this.ApiUri + '/paperwork/scoresheets/' + competitionId.toString() + '/populated';  // URL to web API

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)), );
    }

    scoresheetsSpares(competitionId: number): any {
        const url = this.ApiUri + '/paperwork/scoresheets/' + competitionId.toString() + '/spares';  // URL to web API

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)), );
    }

    signInSheets(eventId: number): any {
        const url = this.ApiUri + '/paperwork/signinsheets/' + eventId.toString() + '';  // URL to web API

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)), );
    }

}

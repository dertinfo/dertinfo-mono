import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError, map, share } from 'rxjs/operators';

import { RepositoryBase } from 'app/core/base/repository.base';
import { AppInsightsService } from 'app/core/logging/appinsights.service';
import { DodTeamCollatedResultPairDto } from 'app/models/dto/DodCollatedResultPairDto';
import { DodIdentifyJudgeSubmissionDto } from 'app/models/dto/DodIdentifyJudgeSubmissionDto';
import { DodIdentifyJudgeSubmissionResponseDto } from 'app/models/dto/DodIdentifyJudgeSubmissionResponseDto';
import { DodRecoverSessionDto } from 'app/models/dto/DodRecoverSessionDto';
import { DodRecoverSessionSubmissionDto } from 'app/models/dto/DodRecoverSessionSubmissionDto';
import { DodResultDto } from 'app/models/dto/DodResultDto';
import { DodResultSubmissionDto } from 'app/models/dto/DodResultSubmissionDto';
import { DodSubmissionDto } from 'app/models/dto/DodSubmissionDto';
import { DodTalkDto } from 'app/models/dto/DodTalkDto';
import { ConfigurationService } from 'app/core/services/configuration.service';

@Injectable()
export class Repository extends RepositoryBase {

    constructor( http: HttpClient, configurationService: ConfigurationService, appInsightsService: AppInsightsService) {
        super(http, configurationService, appInsightsService);
    }

    /**
     * Returns the full list of submissions for dert of derts
     */
    listSubmissions(): Observable<DodSubmissionDto[]> {
        const url = this.ApiUri + '/dodsubmission';

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    /**
     * Returns the full list of talks for dert of derts
     */
    listTalks(): Observable<DodTalkDto[]> {
        const url = this.ApiUri + '/dodtalk';

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    submitScores(dodResultsSubmission: DodResultSubmissionDto): Observable<DodResultDto> {
        const url = this.ApiUri + '/dodresult';

        return this.http.post(url, dodResultsSubmission).pipe(
            share(),
            map((data) => data as DodResultDto));
            // note - catch bad request here and ensure that we don't loose the submission data.
            //      - we need to handle error cases specifically here. This is so we don't loose
            //      - large amount of comments. You can test this by forcing an error on the booleans
            //      - in the submission by setting them to null.
    }

    identifyJudge(dodIdentifyJudgeSubmission: DodIdentifyJudgeSubmissionDto) {
        const url = this.ApiUri + '/doduser/identifyjudge';

        return this.http.post(url, dodIdentifyJudgeSubmission).pipe(
            share(),
            map((data) => data as DodIdentifyJudgeSubmissionResponseDto));
            // note - error handling bubbles.
    }

    recoverSession(submission: DodRecoverSessionSubmissionDto) {
        const url = this.ApiUri + '/doduser/recoversession';

        return this.http.post(url, submission).pipe(
            share(),
            map((data) => data as DodRecoverSessionDto));
    }

    getResults() {
        const url = this.ApiUri + '/dodresult';

        return this.http.get(url).pipe(
            share(),
            map((data) => data as DodTeamCollatedResultPairDto),
            catchError((err) => this.processApiError(err)));
    }
}

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError, map, share } from 'rxjs/operators';

import { RepositoryBase } from 'app/core/base/repository.base';
import { AppInsightsService } from 'app/core/logging/appinsights.service';
import { GroupDto } from 'app/models/dto';
import { DodBlockUserSubmissionDto } from 'app/models/dto/DodBlockUserSubmissionDto';
import { DodGroupResultsDto } from 'app/models/dto/DodGroupResultsDto';
import { DodJudgeInfoDto } from 'app/models/dto/DodJudgeInfoDto';
import { DodResultComplaintDto } from 'app/models/dto/DodResultComplaintDto';
import { DodResultRejectComplaintSubmissionDto } from 'app/models/dto/DodResultRejectComplaintSubmissionDto';
import { DodResultValidateComplaintSubmissionDto } from 'app/models/dto/DodResultValidateComplaintSubmissionDto';
import { DodSubmissionDto } from 'app/models/dto/DodSubmissionDto';
import { DodSubmissionSubmissionDto } from 'app/models/dto/DodSubmissionSubmissionDto';
import { DodSubmissionUpdateSubmissionDto } from 'app/models/dto/DodSubmissionUpdateSubmissionDto';
import { DodTalkDto } from 'app/models/dto/DodTalkDto';
import { DodTalkSubmissionDto } from 'app/models/dto/DodTalkSubmissionDto';
import { DodTalkUpdateSubmissionDto } from 'app/models/dto/DodTalkUpdateSubmissionDto';
import { DodUserResultsDto } from 'app/models/dto/DodUserResultsDto';
import { ConfigurationService } from 'app/core/services/configuration.service';

@Injectable()
export class Repository extends RepositoryBase {

    constructor(http: HttpClient, configurationService: ConfigurationService, appInsightsService: AppInsightsService) {
        super(http, configurationService, appInsightsService);
    }

    /**
     * Returns the full list of submissions for dert of derts
     */
    listSubmissions(): Observable<DodSubmissionDto[]> {
        const url = this.ApiUri + '/dodsubmissionauth';

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    /**
     * Returns the full list of submissions for dert of derts
     */
    listTalks(): Observable<DodTalkDto[]> {
        const url = this.ApiUri + '/dodtalkauth';

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    /**
     * Returns the full list of submissions for dert of derts
     */
    listGroups(): Observable<GroupDto[]> {
        const url = this.ApiUri + '/dodsubmissionauth/groups';

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    /**
     * Returns the full list of submissions for dert of derts
     */
    listJudges(): Observable<Array<DodJudgeInfoDto>> {
        const url = this.ApiUri + '/doduserauth/judges';

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    listComplaints(openComplaints: boolean): Observable<Array<DodResultComplaintDto>> {
        const openUrl = this.ApiUri + '/dodresultauth/opencomplaints';
        const closedUrl = this.ApiUri + '/dodresultauth/closedComplaints';
        const url = openComplaints ? openUrl : closedUrl;

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    addSubmission(dodSubmissionSubmission: DodSubmissionSubmissionDto): Observable<DodSubmissionDto> {
        const url = this.ApiUri + '/dodsubmissionauth';

        return this.http.post(url, dodSubmissionSubmission).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    addTalk(dodTalkSubmission: DodTalkSubmissionDto): Observable<DodTalkDto> {
        const url = this.ApiUri + '/dodtalkauth';

        return this.http.post(url, dodTalkSubmission).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    updateSubmission(dodSubmissionUpdateSubmission: DodSubmissionUpdateSubmissionDto) {
        const url = this.ApiUri + '/dodsubmissionauth';

        return this.http.put(url, dodSubmissionUpdateSubmission).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    updateTalk(dodTalkUpdateSubmission: DodTalkUpdateSubmissionDto) {
        const url = this.ApiUri + '/dodtalkauth';

        return this.http.put(url, dodTalkUpdateSubmission).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    deleteSubmission(dodSubmissionId: number): Observable<DodSubmissionDto> {
        const url = this.ApiUri + '/dodsubmissionauth/' + dodSubmissionId.toString();

        return this.http.delete(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    deleteTalk(dodTalkId: number): Observable<DodTalkDto> {
        const url = this.ApiUri + '/dodtalkauth/' + dodTalkId.toString();

        return this.http.delete(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    validateComplaint(submission: DodResultValidateComplaintSubmissionDto) {
        const url = this.ApiUri + '/dodresultauth/validatecomplaint';

        return this.http.put(url, submission).pipe(
            share(),
            map((data) => data as boolean),
            catchError((err) => this.processApiError(err)));
    }

    rejectComplaint(submission: DodResultRejectComplaintSubmissionDto) {
        const url = this.ApiUri + '/dodresultauth/rejectcomplaint';

        return this.http.put(url, submission).pipe(
            share(),
            map((data) => data as boolean),
            catchError((err) => this.processApiError(err)));
    }

    blockUser(submission: DodBlockUserSubmissionDto) {
        const url = this.ApiUri + '/doduserauth/blockuser';

        return this.http.put(url, submission).pipe(
            share(),
            map((data) => data as boolean),
            catchError((err) => this.processApiError(err)));
    }

    getScoreCardsBySubmission(submissionId: number): Observable<DodGroupResultsDto> {
        const url = this.ApiUri + `/dodresultauth/scorecards/submission/${submissionId}`;

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    getScoreCardsByJudge(judgeId: number): Observable<DodUserResultsDto> {
        const url = this.ApiUri + `/dodresultauth/scorecards/judge/${judgeId}`;

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }
}

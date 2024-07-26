
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError, map, share } from 'rxjs/operators';

import { RepositoryBase } from 'app/core/base/repository.base';
import { AppInsightsService } from 'app/core/logging/appinsights.service';
import { ConfigurationService } from 'app/core/services/configuration.service';
import {
    CompetitionAttachEntryAttributeDto,
    CompetitionAttachJudgesSubmissionDto,
    CompetitionAttachVenuesSubmissionDto,
    CompetitionDetachJudgesSubmissionDto,
    CompetitionDetachVenuesSubmissionDto,
    CompetitionEntryAttributeDto,
    CompetitionOverviewDto,
    CompetitionSettingsDto,
    CompetitionSettingsUpdateSubmissionDto,
    DanceAdditionSubmissionDto,
    DanceCheckedUpdateSubmissionDto,
    DanceDetailDto,
    DanceResultsSubmissionDto,
    GroupTeamCompetitionEntryDto,
    JudgeDto,
    JudgeSlotInformationDto,
    JudgeSubmissionDto,
    JudgeUpdateSubmissionDto,
    RangeReportDto,
    ScoreCategoryDto,
    ScoreCategoryUpdateSubmissionDto,
    ScoreSetDto,
    ScoreSetUpdateSubmissionDto,
    VenueAllocationDto,
    VenueDto,
    VenueSubmissionDto,
    VenueUpdateSubmissionDto
} from 'app/models/dto';

@Injectable()
export class CompetitionRepository extends RepositoryBase {

    constructor( http: HttpClient, configurationService: ConfigurationService, appInsightsService: AppInsightsService) {
        super(http, configurationService, appInsightsService);
    }

    /**
     * Returns the overview of the competition information
     */
    overview(competitionId: number): Observable<CompetitionOverviewDto> {
        const url = this.ApiUri + '/competition/' + competitionId.toString() + '/overview';  // URL to web API

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    /**
     * Returns the settings part of the competition information
     */
    settings(competitionId: number): Observable<CompetitionSettingsDto> {
        const url = this.ApiUri + '/competition/' + competitionId.toString() + '/settings';  // URL to web API

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    /**
     * Returns the score sets for the specified competition
     */
    scoresets(competitionId: number): Observable<Array<ScoreSetDto>> {
        const url = this.ApiUri + '/competition/' + competitionId.toString() + '/scoreSets';  // URL to web API

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    /**
     * Returns the score categories for the specified competition
     */
    scorecategories(competitionId: number): Observable<Array<ScoreCategoryDto>> {
        const url = this.ApiUri + '/competition/' + competitionId.toString() + '/scorecategories';  // URL to web API

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    venues(competitionId: number): Observable<Array<VenueAllocationDto>> {
        const url = this.ApiUri + '/competition/' + competitionId.toString() + '/venues';  // URL to web API

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    judges(competitionId: number): Observable<Array<JudgeDto>> {
        const url = this.ApiUri + '/competition/' + competitionId.toString() + '/judges';  // URL to web API

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    entryAttributes(competitionId: number): Observable<Array<CompetitionEntryAttributeDto>> {
        const url = this.ApiUri + '/competition/' + competitionId.toString() + '/entryattributes';  // URL to web API

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    entrants(competitionId: number): Observable<Array<GroupTeamCompetitionEntryDto>> {
        const url = this.ApiUri + '/competition/' + competitionId.toString() + '/entrants';  // URL to web API

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    dances(competitionId: number): Observable<Array<DanceDetailDto>> {
        const url = this.ApiUri + '/competition/' + competitionId.toString() + '/dances';  // URL to web API

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    allJudges(competitionId: number): Observable<Array<JudgeDto>> {
        const url = this.ApiUri + '/competition/' + competitionId.toString() + '/lookup/judges';  // URL to web API

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    allVenues(competitionId: number): Observable<Array<VenueDto>> {
        const url = this.ApiUri + '/competition/' + competitionId.toString() + '/lookup/venues';  // URL to web API

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    danceResult(danceId: number): any {
        const url = this.ApiUri + '/dance/' + danceId.toString();  // URL to web API

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    judgeSlotInformation(danceId: number): any {
        const url = this.ApiUri + '/dance/' + danceId.toString() + '/judgeslotinfo';  // URL to web API

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    danceResultsCollated(competitionId: number): any {
        const url = this.ApiUri + '/resultsauth/' + competitionId.toString();  // URL to web API

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    // #region Add Methods

    addVenue(competitionId: number, venueSubmission: VenueSubmissionDto): Observable<VenueAllocationDto> {
        const url = this.ApiUri + '/competition/' + competitionId.toString() + '/venue';  // URL to web API

        return this.http.post(url, venueSubmission).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    addJudge(competitionId: number, judgeSubmission: JudgeSubmissionDto): Observable<JudgeDto> {
        const url = this.ApiUri + '/competition/' + competitionId.toString() + '/judge';  // URL to web API

        return this.http.post(url, judgeSubmission).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    addDance(competitionId: number, danceAdditionSubmission: DanceAdditionSubmissionDto): any {
        const url = this.ApiUri + '/competition/' + competitionId.toString() + '/dance';  // URL to web API

        return this.http.post(url, danceAdditionSubmission).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    // #endregion Add Methods

    // #region Update Methods

    updateSettings(competitionId: number, competitionUpdate: CompetitionSettingsUpdateSubmissionDto) {
        const url = this.ApiUri + '/competition/' + competitionId.toString() + '/settings';  // URL to web API

        return this.http.put(url, competitionUpdate).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    updateScoreSet(competitionId: number, scoreSetId: number, scoreSetUpdate: ScoreSetUpdateSubmissionDto) {
        const url = `${this.ApiUri}/competition/${competitionId.toString()}/scoreset/${scoreSetId.toString()}`;  // URL to web API

        return this.http.put(url, scoreSetUpdate).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    updateScoreCategory(competitionId: number, scoreCategoryId: number, scoreCategoryUpdate: ScoreCategoryUpdateSubmissionDto) {
        const url = `${this.ApiUri}/competition/${competitionId.toString()}/scorecategory/${scoreCategoryId.toString()}`;  // URL to web API

        return this.http.put(url, scoreCategoryUpdate).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    updateVenue(competitionId: number, venueId: number, venueUpdate: VenueUpdateSubmissionDto) {
        const url = `${this.ApiUri}/competition/${competitionId.toString()}/venue/${venueId.toString()}`;  // URL to web API

        return this.http.put(url, venueUpdate).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    updateJudge(competitionId: number, judgeId: number, judgeUpdate: JudgeUpdateSubmissionDto) {
        const url = `${this.ApiUri}/competition/${competitionId.toString()}/judge/${judgeId.toString()}`;  // URL to web API

        return this.http.put(url, judgeUpdate).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    updateDanceScores(danceResultsSubmission: DanceResultsSubmissionDto): any {
        const url = `${this.ApiUri}/dance`;  // URL to web API

        return this.http.post(url, danceResultsSubmission).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    updateDanceHasScoredChecked(danceId: number, danceScoresCheckedSubmission: DanceCheckedUpdateSubmissionDto): any {
        const url = `${this.ApiUri}/dance/${danceId}/hasscoreschecked`;  // URL to web API

        return this.http.put(url, danceScoresCheckedSubmission).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    updateJudgeSlotInformation(danceId: number, judgeSlotInformationSubmission: Array<JudgeSlotInformationDto>): any {

        const url = `${this.ApiUri}/dance/${danceId}/judgeslotinfo`;  // URL to web API

        return this.http.put(url, judgeSlotInformationSubmission).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    // #endregion Update Methods

    batchAddJudgeAllocation(
        competitionId: number,
        judgeAllocationSubmission: CompetitionAttachJudgesSubmissionDto
    ): Observable<JudgeDto[]> {
        const url = this.ApiUri + '/competition/' + competitionId.toString() + '/judges/attach';

        return this.http.post(url, judgeAllocationSubmission).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    batchRemoveJudgeAllocation(
        competitionId: number,
        judgeAllocationSubmission: CompetitionDetachJudgesSubmissionDto
    ): Observable<JudgeDto[]> {
        const url = this.ApiUri + '/competition/' + competitionId.toString() + '/judges/detach';

        return this.http.post(url, judgeAllocationSubmission).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    batchAddVenueAllocation(competitionId: number, submission: CompetitionAttachVenuesSubmissionDto): Observable<VenueDto[]> {
        const url = this.ApiUri + '/competition/' + competitionId.toString() + '/venues/attach';

        return this.http.post(url, submission).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    batchRemoveVenueAllocation(competitionId: number, submission: CompetitionDetachVenuesSubmissionDto): Observable<VenueDto[]> {
        const url = this.ApiUri + '/competition/' + competitionId.toString() + '/venues/detach';

        return this.http.post(url, submission).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    attachAttributeToEntrant(
        competitionId: number,
        competitionEntryId: number,
        attachSubmission: CompetitionAttachEntryAttributeDto
        ): Observable<any> {
        const url = `${this.ApiUri}/competition/${competitionId.toString()}/entrant/${competitionEntryId.toString()}/attribute`;

        return this.http.put(url, attachSubmission).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    detachAttributeFromEntrant(competitionId: number, competitionEntryId: number, attributeId: number): Observable<any> {
        const url = `${this.ApiUri}/competition/${competitionId.toString()}/entrant/${competitionEntryId.toString()}/attribute/${attributeId.toString()}`;

        return this.http.delete(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    populateCompetition(competitionId: number): any {
        const url = `${this.ApiUri}/competition/${competitionId.toString()}/populate`;

        return this.http.put(url, null).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    resetCompetition(competitionId: number): any {
        const url = `${this.ApiUri}/competition/${competitionId.toString()}/reset`;

        return this.http.put(url, null).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    generateCompetitionDances(competitionId: number): any {
        const url = `${this.ApiUri}/competition/${competitionId.toString()}/generatedances`;

        return this.http.put(url, null).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    getJudgeRangeReport(competitionId: number, judgeId: number): Observable<RangeReportDto> {
        const url = `${this.ApiUri}/judgereports/${competitionId.toString()}/${judgeId.toString()}`;

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }
}

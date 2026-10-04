
import { Injectable } from '@angular/core';
import { AuthService } from 'app/core/authentication/auth.service';
import {map} from 'rxjs/operators';
import { CompetitionAdminTracker } from './competition-admin.tracker';

import { MatSnackBar } from '@angular/material/snack-bar';
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
    DanceScorePartDto,
    GroupTeamCompetitionEntryDto,
    JudgeDto,
    JudgeSlotInformationDto,
    JudgeSubmissionDto,
    JudgeUpdateSubmissionDto,
    ScoreCategoryDto,
    ScoreCategoryUpdateSubmissionDto,
    ScoreSetDto,
    ScoreSetUpdateSubmissionDto,
    TeamCollatedFullResultDto,
    VenueAllocationDto,
    VenueDto,
    VenueSubmissionDto,
    VenueUpdateSubmissionDto
} from 'app/models/dto';
import { CompetitionRepository } from 'app/modules/repositories';
import { forkJoin ,  Observable, of } from 'rxjs';

@Injectable()
export class CompetitionAdminConductor {

    private _snackBarDuration: number = 1200;

    constructor(
        public authService: AuthService,
        private _tracker: CompetitionAdminTracker,

        private _competitionRepo: CompetitionRepository,
        private snackBar: MatSnackBar
    ) { }

    public setOverview(eventOverviewDto: any) {

        if (this._tracker.overview && this._tracker.overview.id !== eventOverviewDto.id) {
            this._tracker.reset();
        }

        this._tracker.overview = eventOverviewDto;
    }

    // #region Init

    public initJudges(competitionId: number): Observable<Array<JudgeDto>> {

        if (!this._tracker.hasLoadedJudges()) {
            const obs$ = this._competitionRepo.judges(competitionId);
            const subs = obs$.subscribe((judges) => {
                subs.unsubscribe();
                this._tracker.judges = judges;
            });

            return obs$;
        }

        return of(this._tracker.judges);

        /*
        This Mechanism is returning data rather than responding to change.
        It is not the way we want to do things but required due to timescales.
        */
    }

    public initAllJudges() {

        if (!this._tracker.hasLoadedAllJudges()) {
            const subs = this._competitionRepo.allJudges(this._tracker.competitionId).subscribe((allJudges) => {
                subs.unsubscribe();
                this._tracker.allJudges = allJudges;
            });
        }
    }

    public initAllVenues() {

        if (!this._tracker.hasLoadedAllVenues()) {
            const subs = this._competitionRepo.allVenues(this._tracker.competitionId).subscribe((allVenues) => {

                subs.unsubscribe();
                this._tracker.allVenues = allVenues;
            });
        }
    }

    public initEntrants() {

        if (!this._tracker.hasLoadedEntrants()) {
            const subs = this._competitionRepo.entrants(this._tracker.competitionId).subscribe((entrants) => {
                subs.unsubscribe();
                this._tracker.entrants = entrants;
            });
        }
    }

    public initVenues() {
        if (!this._tracker.hasLoadedVenues()) {
            const subs = this._competitionRepo.venues(this._tracker.competitionId).subscribe((venues) => {
                subs.unsubscribe();
                this._tracker.venues = venues;
            });
        }
    }

    public initDanceResultsCollated(force: boolean = false): any {
        if (!this._tracker.hasLoadedCollatedResults() || force) {
            const subs = this._competitionRepo.danceResultsCollated(this._tracker.competitionId).subscribe((collatedResults) => {
                subs.unsubscribe();
                this._tracker.collatedResults = collatedResults;
            });
        }
    }

    public initDances(force: boolean = false): any {
        if (!this._tracker.hasLoadedDances() || force) {
            const subs = this._competitionRepo.dances(this._tracker.competitionId).subscribe((dances) => {
                subs.unsubscribe();
                this._tracker.dances = dances;
            });
        }
    }

    // "Apply" used when data is loaded from a resolver
    public applyOverview(overview: CompetitionOverviewDto) {

        this._tracker.overview = overview;
    }

    // "Apply" used when data is loaded from a resolver
    public applySettings(settings: CompetitionSettingsDto) {

        this._tracker.settings = settings;
    }

    // "Apply" used when data is loaded from a resolver
    public applyScoreSets(scoreSets: Array<ScoreSetDto>) {

        this._tracker.scoreSets = scoreSets;
    }

    // "Apply" used when data is loaded from a resolver
    public applyScoreCategories(scoreCategories: Array<ScoreCategoryDto>) {

        this._tracker.scoreCategories = scoreCategories;
    }

    // "Apply" used when data is loaded from a resolver
    public applyVenues(venues: Array<VenueAllocationDto>) {

        this._tracker.venues = venues;
    }

    // "Apply" used when data is loaded from a resolver
    public applyEntrants(entrants: Array<GroupTeamCompetitionEntryDto>) {

        this._tracker.entrants = entrants;
    }

    // "Apply" used when data is loaded from a resolver
    public applyJudges(judges: Array<JudgeDto>) {

        this._tracker.judges = judges;
    }

    // "Apply" used when data is loaded from a resolver
    public applyEntryAttributes(entryAttributes: Array<CompetitionEntryAttributeDto>) {

        this._tracker.entryAttributes = entryAttributes;
    }

    // "Apply" used when data is loaded from a resolver
    public applyDances(dances: Array<DanceDetailDto>) {
        // note - these are not full. They do not contin the score sheets.

        this._tracker.dances = dances;
    }

    // "Apply" used when data is loaded from a resolver
    public applyDanceResult(danceResult: DanceDetailDto, setFocussed: boolean = false) {

        this._tracker.addDanceResult(danceResult, setFocussed);
    }

    // "Apply" used when data is loaded from a resolver
    public applyJudgeSlotInformation(danceId: number, judgeSlotInformation: Array<JudgeSlotInformationDto>, setFocussed: boolean = false) {

        this._tracker.addDanceSplitResult(danceId, judgeSlotInformation, setFocussed);
    }

    // "Apply" used when data is loaded from a resolver
    public applyDanceResultsCollated(collatedResults: TeamCollatedFullResultDto[]) {

        this._tracker.collatedResults = collatedResults;
    }

    // #endregion

    // #region Public Methods

    public addVenue(venueSubmission: VenueSubmissionDto): Observable<VenueSubmissionDto> {

        const venueAddObs = this._competitionRepo.addVenue(this._tracker.competitionId, venueSubmission);

        const subs = venueAddObs.subscribe((newVenue: VenueAllocationDto) => {
            subs.unsubscribe();
            this._tracker.addVenue(newVenue);
        });

        return venueAddObs;
    }

    public addJudge(judgeSubmission: JudgeSubmissionDto): Observable<JudgeSubmissionDto> {

        const judgeAddObs = this._competitionRepo.addJudge(this._tracker.competitionId, judgeSubmission);

        const subs = judgeAddObs.subscribe((newJudge: JudgeDto) => {
            subs.unsubscribe();
            this._tracker.addJudge(newJudge);
        });

        return judgeAddObs;
    }

    public addDance(danceAdditionSubmission: DanceAdditionSubmissionDto): any {

        const danceAddObs$ = this._competitionRepo.addDance(this._tracker.competitionId, danceAdditionSubmission);

        const subs = danceAddObs$.subscribe((newDance: DanceDetailDto) => {
            subs.unsubscribe();
            this._tracker.addDance(newDance);
        });

        return danceAddObs$;
    }

    public updateSettings(settingsUpdate: CompetitionSettingsUpdateSubmissionDto) {

        const subs = this._competitionRepo.updateSettings(this._tracker.competitionId, settingsUpdate).subscribe(() => {
            subs.unsubscribe();
            this._tracker.updateSettings(settingsUpdate);
            this.snackBar.open('Settings Updated', 'close', { duration: this._snackBarDuration });
        });
    }

    public updateScoreSet(scoreSetId: any, scoreSetUpdate: ScoreSetUpdateSubmissionDto): Observable<ScoreSetDto> {

        const update$ = this._competitionRepo.updateScoreSet(this._tracker.competitionId, scoreSetId, scoreSetUpdate);

        const subs = update$.subscribe(() => {
            subs.unsubscribe();
            this._tracker.updateScoreSet(scoreSetId, scoreSetUpdate);
            this.snackBar.open('ScoreSet Updated', 'close', { duration: this._snackBarDuration });
        });

        return update$;
    }

    public updateScoreCategory(scoreCategoryId: any, scoreCategoryUpdate: ScoreCategoryUpdateSubmissionDto): Observable<ScoreCategoryDto> {

        const update$ = this._competitionRepo.updateScoreCategory(this._tracker.competitionId, scoreCategoryId, scoreCategoryUpdate);

        const subs = update$.subscribe(() => {
            subs.unsubscribe();
            this._tracker.updateScoreCategory(scoreCategoryId, scoreCategoryUpdate);
            this.snackBar.open('ScoreCategory Updated', 'close', { duration: this._snackBarDuration });
        });

        return update$;
    }

    public updateVenue(venueId: any, venueUpdate: VenueUpdateSubmissionDto): Observable<VenueAllocationDto> {

        const update$ = this._competitionRepo.updateVenue(this._tracker.competitionId, venueId, venueUpdate);

        const subs = update$.subscribe(() => {
            subs.unsubscribe();
            this._tracker.updateVenue(venueId, venueUpdate);
            this.snackBar.open('Venue Updated', 'close', { duration: this._snackBarDuration });
        });

        return update$;
    }

    public updateJudge(judgeId: any, judgeUpdate: JudgeUpdateSubmissionDto): Observable<JudgeDto> {

        const update$ = this._competitionRepo.updateJudge(this._tracker.competitionId, judgeId, judgeUpdate);

        const subs = update$.subscribe(() => {
            subs.unsubscribe();
            this._tracker.updateJudge(judgeId, judgeUpdate);
            this.snackBar.open('Judge Updated', 'close', { duration: this._snackBarDuration });
        });

        return update$;
    }

    public updateJudgeSlotInfomation(
        danceId: number,
        judgeSlotInformationUpdate: Array<JudgeSlotInformationDto>
        ): Observable<Array<JudgeSlotInformationDto>> {

        const update$ = this._competitionRepo.updateJudgeSlotInformation(danceId, judgeSlotInformationUpdate);

        const subs = update$.subscribe(() => {
            subs.unsubscribe();
            this._tracker.updateJudgeSlotInformation(danceId, judgeSlotInformationUpdate);
            this.snackBar.open('Dance Score Parts Updated', 'close', { duration: this._snackBarDuration });
        });

        return update$;
    }

    public toggleJudgeAllocations(editedJudges: JudgeDto[]): Observable<JudgeDto[]> {

        const allDone$: Observable<any>[] = [];

        const changeReport = this._tracker.identifyJudgeAllocationChanges(editedJudges);

        const obs1 = this.detachJudges(changeReport.deletes);
        const obs2 = this.attachJudges(changeReport.adds);

        if (obs1 !== null) { allDone$.push(obs1); }
        if (obs2 !== null) { allDone$.push(obs2); }

        const obsAll = forkJoin(allDone$);
        obsAll.subscribe(result => {

            result[0].map((jDto) => {
                this._tracker.removeJudge(jDto);
            });
            result[1].map((jDto) => {
                this._tracker.addJudge(jDto);
            });

            this.snackBar.open('Judges Updated', 'close', { duration: this._snackBarDuration });
        });

        return obsAll;
    }

    public toggleVenueAllocations(editedVenues: VenueDto[]): Observable<VenueDto[]> {

        const allDone$: Observable<any>[] = [];

        const changeReport = this._tracker.identifyVenueAllocationChanges(editedVenues);

        const obs1 = this.detachVenues(changeReport.deletes);
        const obs2 = this.attachVenues(changeReport.adds);

        if (obs1 !== null) { allDone$.push(obs1); }
        if (obs2 !== null) { allDone$.push(obs2); }

        const obsAll = forkJoin(allDone$);
        obsAll.subscribe(result => {

            result[0].map((vDto) => {
                this._tracker.removeVenue(vDto);
            });
            result[1].map((vDto) => {
                this._tracker.addVenue(vDto);
            });

            this.snackBar.open('Venues Updated', 'close', { duration: this._snackBarDuration });
        });

        return obsAll;
    }

    public removeEntryAttributeFromEntrant(attributeId: number, competitionEntryId: number): Observable<any> {

        const obs$ = this._competitionRepo.detachAttributeFromEntrant(this._tracker.competitionId, competitionEntryId, attributeId);

        const subs = obs$.subscribe(() => {
            subs.unsubscribe();
            this._tracker.detachEntryAttributeFromEntry(competitionEntryId, attributeId);
        });

        return obs$;
    }

    public applyEntryAttributeToEntrant(attributeId: number, competitionEntryId: number): Observable<any> {

        const attachSubmission: CompetitionAttachEntryAttributeDto = {
            entryAttributeId: attributeId
        };
        const obs$ = this._competitionRepo.attachAttributeToEntrant(this._tracker.competitionId, competitionEntryId, attachSubmission);

        const subs = obs$.subscribe(() => {
            subs.unsubscribe();
            this._tracker.attachEntryAttributeToEntry(competitionEntryId, attributeId);
        });

        return obs$;
    }

    public submitScores(danceResultsSubmission: DanceResultsSubmissionDto): any {

        const obs$ = this._competitionRepo.updateDanceScores(danceResultsSubmission);

        const subs = obs$.subscribe((danceResult: DanceDetailDto) => {
            subs.unsubscribe();

            this._tracker.updateDanceResult(danceResult);
            this.snackBar.open('Scores Updated', 'close', { duration: this._snackBarDuration });
        });
    }

    public updateDanceHasScoresChecked(danceId: number, danceHasScoresCheckedSubmission: DanceCheckedUpdateSubmissionDto): any {

        const obs$ = this._competitionRepo.updateDanceHasScoredChecked(danceId, danceHasScoresCheckedSubmission);

        const subs = obs$.subscribe(() => {
            subs.unsubscribe();

            this._tracker.updateDanceResultHasScoresChecked(danceId, danceHasScoresCheckedSubmission.hasScoresChecked);
            this.snackBar.open('Scores Checked Changed', 'close', { duration: this._snackBarDuration });
        });
    }

    public clear() {
        this._tracker.reset();
    }

    public populateCompetition(competitionId: number): Observable<any> {

        const obs$ = this._competitionRepo.populateCompetition(this._tracker.competitionId);

        const subs = obs$.subscribe(() => {
            subs.unsubscribe();

            const overview$ = this._competitionRepo.overview(this._tracker.competitionId);
            const entrants$ = this._competitionRepo.entrants(this._tracker.competitionId);

            const obsAll$ = forkJoin([overview$, entrants$]);
            const obsAllMapped$ = obsAll$.pipe(map((data: any[]) => {
                return {
                    overview: data[0],
                    entrants: data[1]
                };
            }));

            const subsAll = obsAllMapped$.subscribe((response) => {
                subsAll.unsubscribe();
                this.applyOverview(response.overview);
                this.applyEntrants(response.entrants);
            });
        });

        return obs$;
    }

    public resetCompetition(competitionId: number): any {

        const obs$ = this._competitionRepo.resetCompetition(this._tracker.competitionId);

        const subs = obs$.subscribe(() => {
            subs.unsubscribe();

            const overview$ = this._competitionRepo.overview(this._tracker.competitionId);
            const entrants$ = this._competitionRepo.entrants(this._tracker.competitionId);
            const dances$ = this._competitionRepo.dances(this._tracker.competitionId);
            const danceResultCollated$ = this._competitionRepo.danceResultsCollated(this._tracker.competitionId);
            // note - we can cheapen this if the dances are not already loaded.

            const obsAll$ = forkJoin([overview$, entrants$, dances$, danceResultCollated$]);
            const obsAllMapped$ = obsAll$.pipe(map((data: any[]) => {
                return {
                    overview: data[0],
                    entrants: data[1],
                    dances: data[2],
                    danceResultCollated: data[3]
                };
            }));

            const subsAll = obsAllMapped$.subscribe((response) => {
                subsAll.unsubscribe();
                this.applyOverview(response.overview);
                this.applyEntrants(response.entrants);
                this.applyDances(response.dances);
                this.applyDanceResultsCollated(response.danceResultCollated);
            });
        });

        return obs$;
    }

    public generateCompetitionDances(competitionId: number): any {

        const obs$ = this._competitionRepo.generateCompetitionDances(this._tracker.competitionId);

        const subs = obs$.subscribe(() => {
            subs.unsubscribe();

            const overview$ = this._competitionRepo.overview(this._tracker.competitionId);
            const dances$ = this._competitionRepo.dances(this._tracker.competitionId);

            const obsAll$ = forkJoin([overview$, dances$]);
            const obsAllMapped$ = obsAll$.pipe(map((data: any[]) => {
                return {
                    overview: data[0],
                    dances: data[1]
                };
            }));

            const subsAll = obsAllMapped$.subscribe((response) => {
                subsAll.unsubscribe();
                this.applyOverview(response.overview);
                this.applyDances(response.dances);
            });
        });

        return obs$;
    }

    // #endregion

    private attachJudges(judges: Array<JudgeDto>): Observable<JudgeDto[]> {

        if (judges.length > 0) {
            const submission: CompetitionAttachJudgesSubmissionDto = {
                judgeIds: judges.map(j => j.id)
            };

            return this._competitionRepo.batchAddJudgeAllocation(this._tracker.competitionId, submission);
        } else {
            return of([]);
        }
    }

    private detachJudges(judges: Array<JudgeDto>): Observable<JudgeDto[]> {

        if (judges.length > 0) {
            const submission: CompetitionDetachJudgesSubmissionDto = {
                judgeIds: judges.map(j => j.id)
            };

            return this._competitionRepo.batchRemoveJudgeAllocation(this._tracker.competitionId, submission);
        } else {
            return of([]);
        }
    }

    private attachVenues(venues: Array<VenueDto>): Observable<VenueDto[]> {

        if (venues.length > 0) {
            const submission: CompetitionAttachVenuesSubmissionDto = {
                venueIds: venues.map(j => j.id)
            };

            return this._competitionRepo.batchAddVenueAllocation(this._tracker.competitionId, submission);
        } else {
            return of([]);
        }
    }

    private detachVenues(venues: Array<VenueDto>): Observable<VenueDto[]> {

        if (venues.length > 0) {
            const submission: CompetitionDetachVenuesSubmissionDto = {
                venueIds: venues.map(j => j.id)
            };

            return this._competitionRepo.batchRemoveVenueAllocation(this._tracker.competitionId, submission);
        } else {
            return of([]);
        }
    }

}

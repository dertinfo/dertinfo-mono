// Angular & RxJS
import { Injectable } from '@angular/core';
import {
    CompetitionEntryAttributeDto,
    CompetitionOverviewDto,
    CompetitionSettingsDto,
    CompetitionSettingsUpdateSubmissionDto,
    DanceDetailDto,
    GroupTeamCompetitionEntryDto,
    JudgeDto,
    JudgeSlotInformationDto,
    JudgeUpdateSubmissionDto,
    ScoreCategoryDto,
    ScoreCategoryUpdateSubmissionDto,
    ScoreSetDto,
    ScoreSetUpdateSubmissionDto,
    TeamCollatedFullResultDto,
    VenueAllocationDto,
    VenueDto,
    VenueUpdateSubmissionDto
} from 'app/models/dto';
import { BehaviorSubject } from 'rxjs';

// Types

@Injectable()
export class CompetitionAdminTracker {

    // #region Observables and Subjects
    private _overview = new BehaviorSubject<CompetitionOverviewDto>(null);
    private _settings = new BehaviorSubject<CompetitionSettingsDto>(null);
    private _scoreSets = new BehaviorSubject<Array<ScoreSetDto>>([]);
    private _scoreCategories = new BehaviorSubject<Array<ScoreCategoryDto>>([]);
    private _venues = new BehaviorSubject<Array<VenueAllocationDto>>([]);
    private _entrants = new BehaviorSubject<Array<GroupTeamCompetitionEntryDto>>([]);
    private _judges = new BehaviorSubject<Array<JudgeDto>>([]);
    private _entryAttributes = new BehaviorSubject<Array<CompetitionEntryAttributeDto>>([]);
    private _dances = new BehaviorSubject<Array<DanceDetailDto>>([]);
    private _allJudges = new BehaviorSubject<Array<JudgeDto>>([]);
    private _allVenues = new BehaviorSubject<Array<VenueDto>>([]);
    private _focussedDanceResult = new BehaviorSubject<DanceDetailDto>(null);
    private _focussedJudgeSlotInfomation = new BehaviorSubject<Array<JudgeSlotInformationDto>>([]);
    private _collatedResults = new BehaviorSubject<Array<TeamCollatedFullResultDto>>([]);

    get overview$() { return this._overview.asObservable(); }
    get settings$() { return this._settings.asObservable(); }
    get scoreSets$() { return this._scoreSets.asObservable(); }
    get scoreCategories$() { return this._scoreCategories.asObservable(); }
    get venues$() { return this._venues.asObservable(); }
    get entrants$() { return this._entrants.asObservable(); }
    get judges$() { return this._judges.asObservable(); }
    get entryAttributes$() { return this._entryAttributes.asObservable(); }
    get dances$() { return this._dances.asObservable(); }
    get allJudges$() { return this._allJudges.asObservable(); }
    get allVenues$() { return this._allVenues.asObservable(); }
    get focussedDanceResult$() { return this._focussedDanceResult.asObservable(); }
    get focussedJudgeSlotInformation$() { return this._focussedJudgeSlotInfomation.asObservable(); }
    get collatedResult$() { return this._collatedResults.asObservable(); }

    // #endregion

    private _memoryStore: {
        overview: CompetitionOverviewDto,
        settings: CompetitionSettingsDto,
        scoreSets: Array<ScoreSetDto>,
        scoreCategories: Array<ScoreCategoryDto>,
        venues: Array<VenueAllocationDto>,
        entrants: Array<GroupTeamCompetitionEntryDto>,
        judges: Array<JudgeDto>,
        entryAttributes: Array<CompetitionEntryAttributeDto>,
        dances: Array<DanceDetailDto>,
        allJudges: Array<JudgeDto>,
        allVenues: Array<VenueDto>,
        danceResults: { [id: number]: DanceDetailDto },
        judgeSlotInformation: { [id: number]: Array<JudgeSlotInformationDto> },
        collatedResults: Array<TeamCollatedFullResultDto>,
    };

    // #region Properties Getters

    public get competitionId(): number {
        return this._memoryStore.overview.id;
    }

    public get overview() {
        return this._memoryStore.overview;
    }

    public get settings() {
        return this._memoryStore.settings;
    }

    public get scoreSets() {
        return this._memoryStore.scoreSets;
    }

    public get scoreCategories() {
        return this._memoryStore.scoreCategories;
    }

    public get venues() {
        return this._memoryStore.venues;
    }

    public get entrants() {
        return this._memoryStore.entrants;
    }

    public get judges() {
        return this._memoryStore.judges;
    }

    public get entryAttributes() {
        return this._memoryStore.entryAttributes;
    }

    public get dances() {
        return this._memoryStore.dances;
    }

    public get allJudges() {
        return this._memoryStore.allJudges;
    }

    public get allVenues() {
        return this._memoryStore.allVenues;
    }

    public get danceResults() {
        return this._memoryStore.danceResults;
    }

    public get judgeSlotInformation() {
        return this._memoryStore.judgeSlotInformation;
    }

    public get collatedResults() {
        return this._memoryStore.collatedResults;
    }

    // #endregion

    // #region Properties Setters

    public set overview(overview: CompetitionOverviewDto) {
        this._memoryStore.overview = overview;
        this._overview.next(this._memoryStore.overview);
    }

    public set settings(settings: CompetitionSettingsDto) {
        this._memoryStore.settings = settings;
        this._settings.next(this._memoryStore.settings);
    }

    public set scoreSets(scoreSets: Array<ScoreSetDto>) {
        this._memoryStore.scoreSets = scoreSets;
        this._scoreSets.next(this._memoryStore.scoreSets);
    }

    public set scoreCategories(scoreCategories: Array<ScoreCategoryDto>) {
        this._memoryStore.scoreCategories = scoreCategories;
        this._scoreCategories.next(this._memoryStore.scoreCategories);
    }

    public set venues(venues: Array<VenueAllocationDto>) {
        this._memoryStore.venues = venues;
        this._venues.next(this._memoryStore.venues);
    }

    public set entrants(entrants: Array<GroupTeamCompetitionEntryDto>) {
        this._memoryStore.entrants = entrants;
        this._entrants.next(this._memoryStore.entrants);
    }

    public set judges(judges: Array<JudgeDto>) {
        this._memoryStore.judges = judges;
        this._judges.next(this._memoryStore.judges);
    }

    public set entryAttributes(entryAttributes: Array<CompetitionEntryAttributeDto>) {
        this._memoryStore.entryAttributes = entryAttributes;
        this._entryAttributes.next(this._memoryStore.entryAttributes);
    }

    public set dances(dances: Array<DanceDetailDto>) {
        this._memoryStore.dances = dances;
        this._dances.next(this._memoryStore.dances);
    }

    public set allJudges(allJudges: Array<JudgeDto>) {
        this._memoryStore.allJudges = allJudges;
        this._allJudges.next(this._memoryStore.allJudges);
    }

    public set allVenues(allVenues: Array<VenueDto>) {
        this._memoryStore.allVenues = allVenues;
        this._allVenues.next(this._memoryStore.allVenues);
    }

    public set collatedResults(collatedResults: Array<TeamCollatedFullResultDto>) {
        this._memoryStore.collatedResults = collatedResults;
        this._collatedResults.next(this._memoryStore.collatedResults);
    }

    // #endregion

    constructor() {
        this.reset();
    }

    // #region Properties hasLoaded()

    public hasLoadedOverview(): boolean {
        return this._memoryStore.overview !== null;
    }

    public hasLoadedSettings(): boolean {
        return this._memoryStore.settings !== null;
    }

    public hasLoadedScoreSets(): boolean {
        return this._memoryStore.scoreSets !== null;
    }

    public hasLoadedScoreCategories(): boolean {
        return this._memoryStore.scoreCategories !== null;
    }

    public hasLoadedVenues(): boolean {
        return this._memoryStore.venues !== null;
    }

    public hasLoadedEntrants(): boolean {
        return this._memoryStore.entrants !== null;
    }

    public hasLoadedJudges(): boolean {
        return this._memoryStore.judges !== null;
    }

    public hasLoadedEntryAttributes(): boolean {
        return this._memoryStore.entryAttributes !== null;
    }

    public hasLoadedDances(): boolean {
        return this._memoryStore.dances !== null;
    }

    public hasLoadedAllJudges(): boolean {
        return this._memoryStore.allJudges !== null;
    }

    public hasLoadedAllVenues(): boolean {
        return this._memoryStore.allVenues !== null;
    }

    public hasLoadedDanceResult(danceId): boolean {
        return null !== this._memoryStore.danceResults[danceId] && undefined !== this._memoryStore.danceResults[danceId];
    }

    public hasLoadedJudgeSlotInformation(danceId): boolean {
        return null !== this._memoryStore.judgeSlotInformation[danceId] && undefined !== this._memoryStore.judgeSlotInformation[danceId];
    }

    public hasLoadedCollatedResults(): boolean {
        return this._memoryStore.collatedResults !== null;
    }

    // #endregion

    public reset() {
        this._memoryStore = {
            overview: null,
            settings: null,
            scoreSets: null,
            scoreCategories: null,
            venues: null,
            entrants: null,
            judges: null,
            entryAttributes: null,
            dances: null,
            allJudges: null,
            allVenues: null,
            danceResults: {},
            judgeSlotInformation: {},
            collatedResults: null
        };
    }

    public addVenue(venue: VenueAllocationDto) {

        this._memoryStore.venues.push(venue);
        this._venues.next(this._memoryStore.venues);
    }

    public addJudge(judge: JudgeDto) {

        this._memoryStore.judges.push(judge);
        this._judges.next(this._memoryStore.judges);
    }

    public addDance(dance: DanceDetailDto): any {

        this._memoryStore.dances.push(dance);
        this._dances.next(this._memoryStore.dances);
    }

    public addDanceResult(danceResult: DanceDetailDto, setFocussed: boolean): void {
        this._memoryStore.danceResults[danceResult.danceId] = danceResult;

        if (setFocussed) {
            this._focussedDanceResult.next(this._memoryStore.danceResults[danceResult.danceId]);
        }
    }

    public addDanceSplitResult(danceId: number, danceSplitResult: Array<JudgeSlotInformationDto>, setFocussed: boolean): void {
        this._memoryStore.judgeSlotInformation[danceId] = danceSplitResult;

        if (setFocussed) {
            this._focussedJudgeSlotInfomation.next(this._memoryStore.judgeSlotInformation[danceId]);
        }
    }

    public removeJudge(judge: JudgeDto) {

        // Remove the judge
        const found = this._memoryStore.judges.find(j => j.id === judge.id);
        const index = this._memoryStore.judges.indexOf(found);
        this._memoryStore.judges.splice(index, 1);

        // Deallocate on the client from slots.
        if (this.hasLoadedVenues()) {
            this._memoryStore.venues.map(v => v.judgeSlots.map(js => {

                let judgeDeallocated = false;

                if (js.judgeId === judge.id) {
                    js.judgeId = 0;
                    js.judgeName = null;
                    judgeDeallocated = true;
                }

                if (judgeDeallocated) {
                    v.judgesAllocated = false;
                }

            }));
        }

        this._judges.next(this._memoryStore.judges);
        this._venues.next(this._memoryStore.venues);
    }

    public removeVenue(venue: VenueAllocationDto) {

        // Remove the judge
        const found = this._memoryStore.venues.find(j => j.id === venue.id);
        const index = this._memoryStore.venues.indexOf(found);
        this._memoryStore.venues.splice(index, 1);

        this._venues.next(this._memoryStore.venues);
    }

    public updateSettings(update: CompetitionSettingsUpdateSubmissionDto) {

        this._memoryStore.settings.noOfJudgesPerVenue = update.noOfJudgesPerVenue;
        this._memoryStore.settings.resultsPublished = update.resultsPublished;
        this._memoryStore.settings.resultsCollated = update.resultsCollated;
        this._memoryStore.settings.inTestingMode = update.inTestingMode;
        this._memoryStore.settings.allowAdHocDanceAddition = update.allowAdHocDanceAddition;

        // We should have interface flags on the overview as this one determines the add button on the dances screen.
        this._memoryStore.overview.summary.allowAdHocDanceAddition = update.allowAdHocDanceAddition;
        this._memoryStore.overview.summary.resultsPublished = update.resultsPublished;

        // If we change the dance results published then this changes the lock status of the cached dances.
        Object.keys(this._memoryStore.danceResults).forEach(key => {
            const danceId = parseInt(key, 10);
            const danceDetail = this._memoryStore.danceResults[danceId];
            danceDetail.hasScoresLocked = update.resultsPublished;
        });

        this._settings.next(this._memoryStore.settings);
    }

    public updateScoreSet(scoreSetId: number, scoreSetUpdate: ScoreSetUpdateSubmissionDto) {

        const scoreSet = this._memoryStore.scoreSets.find(ss => ss.scoreSetId === scoreSetId);
        scoreSet.name = scoreSetUpdate.name;

        this._scoreSets.next(this._memoryStore.scoreSets);
    }

    public updateVenue(venueId: number, venueUpdate: VenueUpdateSubmissionDto) {

        const venue = this._memoryStore.venues.find(v => v.id === venueId);
        venue.name = venueUpdate.name;

        // Update the ids and names on the judge slots
        venueUpdate.judgeSlotUpdates.map((jsu) => {
            if (jsu.judgeId > 0) {
                const judge = this.judges.find(j => j.id === jsu.judgeId);
                const judgeSlot = venue.judgeSlots.find(js => js.id === jsu.id);

                judgeSlot.judgeId = judge.id;
                judgeSlot.judgeName = judge.name;
            }
        });

        // If all the slots are allocated mark the venue as allocated.
        const unallocatedSlots = venue.judgeSlots.filter(js => js.judgeId === 0);
        venue.judgesAllocated = unallocatedSlots.length === 0;

        this._venues.next(this._memoryStore.venues);
    }

    public updateJudge(judgeId: number, judgeUpdate: JudgeUpdateSubmissionDto) {

        const judge = this._memoryStore.judges.find(j => j.id === judgeId);
        judge.name = judgeUpdate.name;
        judge.telephone = judgeUpdate.telephone;
        judge.email = judgeUpdate.email;

        this._judges.next(this._memoryStore.judges);
    }

    public updateScoreCategory(scoreCategoryId: number, scoreCategoryUpdate: ScoreCategoryUpdateSubmissionDto) {

        const scoreCategory = this._memoryStore.scoreCategories.find(ss => ss.scoreCategoryId === scoreCategoryId);
        scoreCategory.name = scoreCategoryUpdate.name;
        scoreCategory.maxMarks = scoreCategoryUpdate.maxMarks;
        scoreCategory.tag = scoreCategoryUpdate.tag;
        scoreCategory.sortOrder = scoreCategoryUpdate.sortOrder;

        this._scoreCategories.next(this._memoryStore.scoreCategories);
    }

    public updateDanceResult(updatedDanceResult: DanceDetailDto): any {

        // If we have already loaded the dances we need to update these.
        if (this.hasLoadedDances()) {
            const index = this._memoryStore.dances.findIndex(t => t.danceId === updatedDanceResult.danceId);
            const found = this._memoryStore.dances[index];

            found.hasScoresEntered = true;

            this._dances.next(this._memoryStore.dances);
        }

        const detail = this._memoryStore.danceResults[updatedDanceResult.danceId];

        this._memoryStore.danceResults[updatedDanceResult.danceId] = updatedDanceResult;

        this._dances.next(this._memoryStore.dances);
        this._focussedDanceResult.next(this._memoryStore.danceResults[updatedDanceResult.danceId]);
    }

    public updateDanceResultHasScoresChecked(danceId: any, hasScoresChecked: boolean): any {

        // If we have already loaded the dances we need to update these.
        if (this.hasLoadedDances()) {
            const index = this._memoryStore.dances.findIndex(t => t.danceId === danceId);
            const found = this._memoryStore.dances[index];

            found.hasScoresChecked = hasScoresChecked;

            this._dances.next(this._memoryStore.dances);
        }

        const detail = this._memoryStore.danceResults[danceId];
        detail.hasScoresChecked = hasScoresChecked;

        this._focussedDanceResult.next(this._memoryStore.danceResults[danceId]);
    }

    public updateJudgeSlotInformation(danceId: number, judgeSlotInformation: Array<JudgeSlotInformationDto>) {

        if (this.hasLoadedJudgeSlotInformation(danceId)) {
            this._memoryStore.judgeSlotInformation[danceId] = judgeSlotInformation;
            this._focussedJudgeSlotInfomation.next(this._memoryStore.judgeSlotInformation[danceId]);
        }
    }

    public attachEntryAttributeToEntry(competitionEntryId: number, attributeId: number): void {
        const entryAttribute = this._memoryStore.entryAttributes.find(ea => ea.id === attributeId);
        const entrant = this._memoryStore.entrants.find(e => e.competitionEntryId === competitionEntryId);

        entrant.entryAttributes.push(entryAttribute);
    }

    public detachEntryAttributeFromEntry(competitionEntryId: number, attributeId: number): void {
        const entrant = this._memoryStore.entrants.find(e => e.competitionEntryId === competitionEntryId);

        const found = entrant.entryAttributes.find(ea => ea.id === attributeId);
        const index = entrant.entryAttributes.indexOf(found);
        entrant.entryAttributes.splice(index, 1);
    }

    public identifyJudgeAllocationChanges(changes: JudgeDto[]) {
        const adds: JudgeDto[] = [];
        const deletes: JudgeDto[] = [];

        changes.forEach(changedJudge => {
            const judge = this._memoryStore.judges.find(j => j.id === changedJudge.id);

            if (judge) {
                deletes.push(judge);
            } else {
                adds.push(changedJudge);
            }
        });

        return { adds, deletes };
    }

    public identifyVenueAllocationChanges(changes: VenueDto[]) {
        const adds: VenueDto[] = [];
        const deletes: VenueDto[] = [];

        changes.forEach(changedVenue => {
            const venue = this._memoryStore.venues.find(j => j.id === changedVenue.id);

            if (venue) {
                deletes.push(venue);
            } else {
                adds.push(changedVenue);
            }
        });

        return { adds, deletes };
    }
}

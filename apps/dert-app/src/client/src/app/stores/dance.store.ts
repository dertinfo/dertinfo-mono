import { Injectable } from '@angular/core';
import { Observable, Subject, Subscription } from 'rxjs';
import { List } from 'immutable';
import { ILogger } from '../providers/diagnostics/logger';

// stores & services
import { VenueStore } from '../stores/venue.store';
import { DanceService } from '../services/dance.service';
import {
    DanceDetailDto,
    DanceImageSubmissionDto,
    DanceMarkingSheetDto,
    DanceResultsSubmissionDto,
    VenueDanceDto,
    VenueDto
} from '../models/dto/index';




@Injectable()
export class DanceStore {

    // Fields
    private _myDances: List<VenueDanceDto>;

    private _primaryVenueChangedSubscription: Subscription;

    private _primaryDanceSubject: Subject<DanceDetailDto> = new Subject<DanceDetailDto>();
    private _dancesSubject: Subject<List<VenueDanceDto>> = new Subject<List<VenueDanceDto>>();

    // Properties
    get primaryDance$(): Observable<DanceDetailDto> {
        return this._primaryDanceSubject.asObservable();
    }

    get dances$(): Observable<List<VenueDanceDto>> {
        return this._dancesSubject.asObservable();
    }

    // #####################################################################
    // # LIFECYCLE
    // #####################################################################

    constructor(private _logger: ILogger, private _venueStore: VenueStore, private _danceService: DanceService) {
        this._logger.trace('DanceStore - constructor ', ['DanceStore']);

        this.subscribe();
    }


    // #####################################################################
    // # SUBSCRIPTIONS
    // #####################################################################

    private subscribe() {
        this._logger.trace('VenueStore - subscribe ', ['VenueStore']);

        /* Application Authentication Change */
        this._primaryVenueChangedSubscription = this._venueStore.primaryVenue$.subscribe(
            (data) => this.primaryVenueChangedNext(data),
            (err) => this.primaryVenueChangedError(err),
            () => this.primaryVenueChangedComplete()
        );
    }

    private primaryVenueChangedNext(venue: VenueDto) {
        this._logger.trace('VenueStore - primaryVenueChangedNext ', ['VenueStore']);

        if (venue) {
            // we don't need to do this - this.loadDancesByVenueId(venue.id);
            // We do not want to immedately load the dances when the venue is changed.
            // - we only want to do this when we enter the page. Achieved by hooking to beahviour subject.
        } else {
            this._dancesSubject.next(List([]));
        }
    }

    private primaryVenueChangedError(err: any) {
        this._logger.trace('VenueStore - primaryVenueChangedError ', ['VenueStore']);

        if (this._primaryVenueChangedSubscription) {
            this._primaryVenueChangedSubscription.unsubscribe();
        }
    }

    private primaryVenueChangedComplete() {
        this._logger.trace('VenueStore - primaryVenueChangedComplete ', ['VenueStore']);

        if (this._primaryVenueChangedSubscription) {
            this._primaryVenueChangedSubscription.unsubscribe();
        }
    }

    // #####################################################################
    // # PRIVATE
    // #####################################################################

    public loadDancesByVenueId(venueId) {
        this._logger.trace('DanceStore - loadDancesByVenueId ', ['DanceStore']);

        this._danceService.getDancesByVenueId(venueId).subscribe(
            (data) => {
                this._logger.trace('DanceStore - loadDancesByVenueId - next', ['DanceStore']);
                const dancesArr = <VenueDanceDto[]>data;
                this._myDances = List(dancesArr);
                this._dancesSubject.next(this._myDances);
            },
            (err) => {
                this._logger.error('DanceStore - loadDanceDetail - error', err);
            },
            () => {
                this._logger.trace('DanceStore - loadDancesByVenueId - complete', ['VenueDanceStoreStore']);

            }
        );
    }

    public loadDanceDetail(danceId: number) {
        this._logger.trace('DanceStore - loadDanceDetail ', ['DanceStore']);

        this._danceService.getDance(danceId).subscribe(
            (data: DanceDetailDto) => {
                this._logger.trace('DanceStore - loadDanceDetail - next', ['DanceStore']);
                this._primaryDanceSubject.next(data);
            },
            (err) => {
                this._logger.error('DanceStore - loadDanceDetail - error', err);
            },
            () => {
                this._logger.trace('DanceStore - loadDanceDetail - complete', ['VenueDanceStoreStore']);

            }
        );
    }

    submitScores(danceScoresSubmission: DanceResultsSubmissionDto): Observable<DanceDetailDto> {
        this._logger.trace('DanceStore - submitScores ', ['DanceStore']);

        const submission: Observable<DanceDetailDto> = this._danceService.submitScores(danceScoresSubmission);

        submission.subscribe(
            (danceDetailDto: DanceDetailDto) => {

                // Update the list
                const dances: List<VenueDanceDto> = this._myDances;
                const storedDance: VenueDanceDto = dances.find((venueDanceDto: VenueDanceDto) => danceScoresSubmission.danceId === venueDanceDto.danceId);

                storedDance.danceId = danceDetailDto.danceId;
                storedDance.competitionId = danceDetailDto.competitionId;
                storedDance.competitionName = danceDetailDto.competitionName;
                storedDance.teamId = danceDetailDto.teamId;
                storedDance.teamName = danceDetailDto.teamName;
                storedDance.hasScoresEntered = danceDetailDto.hasScoresEntered;
                storedDance.hasScoresChecked = danceDetailDto.hasScoresChecked;
                storedDance.scoresEnteredBy = danceDetailDto.scoresEnteredBy;

                // let changedList = dances.set(index, newVenueDanceDto);

                // Push to observables
                this._primaryDanceSubject.next(danceDetailDto);
                this._dancesSubject.next(dances);

            }
        );

        return submission;
    }

    uploadDancePhoto(danceImageSubmissionDto: DanceImageSubmissionDto): Observable<DanceMarkingSheetDto> {
        this._logger.trace('DanceStore - uploadDancePhoto ', ['DanceStore']);

        const submission: Observable<DanceMarkingSheetDto> = this._danceService.uploadPhoto(danceImageSubmissionDto);

        return submission;
    }

    removeDancePhoto(danceId: number, markingSheetId: number): Observable<DanceMarkingSheetDto> {
        this._logger.trace('DanceStore - removeDancePhoto ', ['DanceStore']);

        const submission: Observable<DanceMarkingSheetDto> = this._danceService.deletePhoto(danceId, markingSheetId);

        return submission;
    }


}

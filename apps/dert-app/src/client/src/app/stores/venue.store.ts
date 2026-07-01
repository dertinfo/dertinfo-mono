
import { Injectable } from '@angular/core';
import { BehaviorSubject, Subscription } from 'rxjs';
import { List } from 'immutable';
import { ILogger } from '../providers/diagnostics/logger';


// stores & services
import { VenueService } from '../services/venue.service';
import { VenueDto } from '../models/dto/VenueDto';
import { EventStore } from './event.store';
import { EventDto } from '../models/dto/EventDto';
import { AuthService } from '../authentication/auth.service';

@Injectable()
export class VenueStore {

    // Fields
    private _primaryVenue: BehaviorSubject<VenueDto> = new BehaviorSubject(null);
    private _venues: BehaviorSubject<List<VenueDto>> = new BehaviorSubject(List([]));

    private _authChangedSubscription: Subscription;
    private _primaryEventSubscription: Subscription;

    // Properties
    get primaryVenue$() {
        return this._primaryVenue.asObservable();
    }

    get primaryVenue() {
        return this._primaryVenue.value;
    }

    get venues$() {
        return this._venues.asObservable();
    }



    // #####################################################################
    // # LIFECYCLE
    // #####################################################################

    // Constructor
    constructor(private _logger: ILogger, private _authService: AuthService, private _eventStore: EventStore, private _venueService: VenueService) {
        this._logger.trace('VenueStore - constructor ', ['VenueStore']);

        this.subscribe();
        this.init();
    }

    private init() {
        this._logger.trace('VenueStore - init ', ['VenueStore']);
    }

    // #####################################################################
    // # SUBSCRIPTIONS
    // #####################################################################

    private subscribe() {
        this._logger.trace('VenueStore - subscribe ', ['VenueStore']);

        /* Application Authentication Change */
        this._authChangedSubscription = this._authService.authChanged$.subscribe(
            (data) => this.userAuthenticatedSubNext(data),
            (err) => this.userAuthenticatedSubError(err),
            () => this.userAuthenticatedSubComplete()
        );

        /* Application Event Change */
        this._primaryEventSubscription = this._eventStore.primaryEvent$.subscribe(
            (data: EventDto) => this.primaryEventChangedNext(data),
            (err) => this.primaryEventChangedError(err),
            () => this.primaryEventChangedComplete()
        );
    }

    private userAuthenticatedSubNext(isAuthenticated: boolean) {
        this._logger.trace('VenueStore - userAuthenticatedSubNext ', ['VenueStore']);

        if (!isAuthenticated) {
            // Authenticated changed to false. Clear the venue store data
            this._venues.next(List([]));
            this._primaryVenue.next(null);
        }
    }

    private userAuthenticatedSubError(err: any) {
        this._logger.trace('VenueStore - userAuthenticatedSubError ', ['VenueStore']);

        if (this._authChangedSubscription) {
            this._authChangedSubscription.unsubscribe();
        }
    }

    private userAuthenticatedSubComplete() {
        this._logger.trace('VenueStore - userAuthenticatedSubComplete ', ['VenueStore']);

        if (this._authChangedSubscription) {
            this._authChangedSubscription.unsubscribe();
        }
    }

    private primaryEventChangedNext(evt: EventDto) {
        this._logger.trace('VenueStore - primaryEventChangedNext ', ['VenueStore'], evt);

        if (evt) {
            this.loadVenuesByCurrentUser(evt.id);
        }
    }

    private primaryEventChangedError(err: any) {
        this._logger.trace('VenueStore - primaryEventChangedError ', ['VenueStore']);

        if (this._primaryEventSubscription) {
            this._primaryEventSubscription.unsubscribe();
        }
    }

    private primaryEventChangedComplete() {
        this._logger.trace('VenueStore - primaryEventChangedComplete ', ['VenueStore']);

        if (this._primaryEventSubscription) {
            this._primaryEventSubscription.unsubscribe();
        }
    }

    // #####################################################################
    // # PRIVATE
    // #####################################################################

    private loadVenuesByCurrentUser(eventId: number) {
        this._logger.trace('VenueStore - loadVenuesByCurrentUser ', ['VenueStore']);

        this._venueService.getVenues(eventId).subscribe(
            (venues: VenueDto[]) => {
                this._logger.trace('VenueStore - loadVenuesByCurrentUser - next', ['VenueStore']);
                this._venues.next(List(venues));
                if (venues.length > 0) {
                    this._primaryVenue.next(venues[0]);
                } else {
                    this._primaryVenue.next(null);
                }
            },
            (err) => {
                this._logger.error('VenueStore - loadVenuesByCurrentUser - error', err);
            },
            () => {
                this._logger.trace('VenueStore - loadVenuesByCurrentUser - complete', ['VenueStore']);

            }
        );

    }

    // #####################################################################
    // # PUBLIC
    // #####################################################################

    public setPrimaryVenue(venue: VenueDto) {
        this._logger.trace('VenueStore - setPrimaryVenue ', ['VenueStore']);

        /* todo - ensure primary venue in current list */
        this._primaryVenue.next(venue);
    }
}


import { Injectable } from '@angular/core';
import { BehaviorSubject, Subscription } from 'rxjs';
import { List } from 'immutable';
import { ILogger } from '../providers/diagnostics/logger';


// stores & services
import { EventService } from '../services/event.service';
import { EventDto } from '../models/dto/EventDto';
import { AuthService } from '../authentication/auth.service';

@Injectable()
export class EventStore {

    // Fields
    private _primaryEvent: BehaviorSubject<EventDto> = new BehaviorSubject(null);
    private _events: BehaviorSubject<List<EventDto>> = new BehaviorSubject(List([]));

    private _authChangedSubscription: Subscription;

    // Properties
    get primaryEvent$() {
        return this._primaryEvent.asObservable();
    }

    get events$() {
        return this._events.asObservable();
    }

    get currentEventId(): number {
        if (this._primaryEvent.value) {
            return this._primaryEvent.value.id;
        } else {
            return 0;
        }
    }

    // #####################################################################
    // # LIFECYCLE
    // #####################################################################

    // Constructor
    constructor(private _logger: ILogger, private _eventService: EventService, private _authService: AuthService) {
        this._logger.trace('EventStore - constructor ', ['EventStore']);

        this.subscribe();
        this.init();
    }

    private init() {
        this._logger.trace('EventStore - init ', ['EventStore']);

        if (this._authService.isAuthenticated()) {
            this.loadEventsByCurrentUser();
        }

    }

    // #####################################################################
    // # SUBSCRIPTIONS
    // #####################################################################

    private subscribe() {
        this._logger.trace('EventStore - subscribe ', ['EventStore']);

        /* Application Authentication Change */
        this._authChangedSubscription = this._authService.authChanged$.subscribe(
            (data) => this.userAuthenticatedSubNext(data),
            (err) => this.userAuthenticatedSubError(err),
            () => this.userAuthenticatedSubComplete()
        );
    }

    private userAuthenticatedSubNext(isAuthenticated: boolean) {
        this._logger.trace('EventStore - userAuthenticatedSubNext ', ['EventStore']);

        if (isAuthenticated) {
            // Authenticated changed to true. Prepare events for user
            this.loadEventsByCurrentUser();
        } else {
            // Authenticated changed to false. Clear the event store data
            this._events.next(List([]));
            this._primaryEvent.next(null);
        }
    }

    private userAuthenticatedSubError(err: any) {
        this._logger.trace('EventStore - userAuthenticatedSubError ', ['EventStore']);

        if (this._authChangedSubscription) {
            this._authChangedSubscription.unsubscribe();
        }
    }

    private userAuthenticatedSubComplete() {
        this._logger.trace('EventStore - userAuthenticatedSubComplete ', ['EventStore']);

        if (this._authChangedSubscription) {
            this._authChangedSubscription.unsubscribe();
        }
    }

    // #####################################################################
    // # PRIVATE
    // #####################################################################

    private loadEventsByCurrentUser() {
        this._logger.trace('EventStore - loadEventsByCurrentUser ', ['EventStore']);

        this._eventService.getEvents().subscribe(
            (events: EventDto[]) => {
                this._logger.trace('EventStore - loadEventsByCurrentUser - next', ['EventStore']);
                this._events.next(List(events));
                if (events.length > 0) {
                    this._primaryEvent.next(events[0]);
                } else {
                    this._primaryEvent.next(null);
                }
            },
            (err) => {
                this._logger.error('EventStore - loadEventsByCurrentUser - error', err);
            },
            () => {
                this._logger.trace('EventStore - loadEventsByCurrentUser - complete', ['EventStore']);

            }
        );

    }

    // #####################################################################
    // # PUBLIC
    // #####################################################################

    public setPrimaryEvent(event: EventDto) {
        this._logger.trace('EventStore - setPrimaryEvent ', ['EventStore']);

        /* todo - ensure primary event in current list */
        this._primaryEvent.next(event);
    }
}

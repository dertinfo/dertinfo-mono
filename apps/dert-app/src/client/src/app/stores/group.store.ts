
import { Injectable } from '@angular/core';
import { BehaviorSubject, Subscription } from 'rxjs';
import { List } from 'immutable';
import { ILogger } from '../providers/diagnostics/logger';


// stores & services
import { GroupService } from '../services/group.service';
import { GroupDto } from '../models/dto/GroupDto';
import { AuthService } from '../authentication/auth.service';

@Injectable()
export class GroupStore {

    // Fields
    private _groups: BehaviorSubject<List<GroupDto>> = new BehaviorSubject(List([]));

    private _authChangedSubscription: Subscription;

    // Properties
    get groups$() {
        return this._groups.asObservable();
    }

    // #####################################################################
    // # LIFECYCLE
    // #####################################################################

    // Constructor
    constructor(private _logger: ILogger, private _authService: AuthService, private _groupService: GroupService) {
        this._logger.trace('GroupStore - constructor ', ['GroupStore']);

        this.subscribe();
        this.init();
    }

    private init() {
        this._logger.trace('GroupStore - init ', ['GroupStore']);

        // Load on request not on init
        // this.loadGroupsByCurrentUser();
    }

    // #####################################################################
    // # SUBSCRIPTIONS
    // #####################################################################

    private subscribe() {
        this._logger.trace('GroupStore - subscribe ', ['GroupStore']);

        /* Application Authentication Change */
        this._authChangedSubscription = this._authService.authChanged$.subscribe(
            (data) => this.userAuthenticatedSubNext(data),
            (err) => this.userAuthenticatedSubError(err),
            () => this.userAuthenticatedSubComplete()
        );
    }

    private userAuthenticatedSubNext(isAuthenticated: boolean) {
        this._logger.trace('GroupStore - userAuthenticatedSubNext ', ['GroupStore']);

        if (isAuthenticated) {
            // Reload the groups is auth changed
            this.loadGroupsByCurrentUser();
        } else {
            // Authenticated changed to false. Clear the group store data
            this._groups.next(List([]));
        }
    }

    private userAuthenticatedSubError(err: any) {
        this._logger.trace('GroupStore - userAuthenticatedSubError ', ['GroupStore']);

        if (this._authChangedSubscription) {
            this._authChangedSubscription.unsubscribe();
        }
    }

    private userAuthenticatedSubComplete() {
        this._logger.trace('GroupStore - userAuthenticatedSubComplete ', ['GroupStore']);

        if (this._authChangedSubscription) {
            this._authChangedSubscription.unsubscribe();
        }
    }

    // #####################################################################
    // # PRIVATE
    // #####################################################################



    // #####################################################################
    // # PUBLIC
    // #####################################################################

    public loadGroupsByCurrentUser() {
        this._logger.trace('GroupStore - loadGroupsByCurrentUser ', ['GroupStore']);

        this._groupService.getGroups().subscribe(
            (groups: GroupDto[]) => {
                this._logger.trace('GroupStore - loadGroupsByCurrentUser - next', ['GroupStore']);
                this._groups.next(List(groups));
            },
            (err) => {
                this._logger.error('GroupStore - loadGroupsByCurrentUser - error', err);
            },
            () => {
                this._logger.trace('GroupStore - loadGroupsByCurrentUser - complete', ['GroupStore']);
            }
        );

    }
}

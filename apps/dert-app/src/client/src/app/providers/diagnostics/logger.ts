import { Injectable } from '@angular/core';

export abstract class ILogger {
    trace: (msg: Object, tags?: string[], obj?: any) => void;
    debug: (msg: Object, tags?: string[]) => void;
    log: (msg: Object, tags?: string[]) => void;
    error: (msg: Object, obj?: any) => void;
}

/**
 * ConsoleLogger - a console loggin version of the simple interface..
 *               - added just for now the help with the development process.
 *               - logs can be marked as trace to insepect workflow but be disabled in here.
 *               - it was intended that the log messages could be tagged and then output filtered to areas of functionality
 */
@Injectable()
export class ConsoleLogger implements ILogger {

    private _traceEnabled = false;
    private _debugEnabled = false;

    private _serviceFilterTags = [
        /*'VenueService',
        'ImageService',
        'DanceService',
        */'EnvironmentService', /*
        'EventService',
        'GroupService'
        */'AuthService',
        'LockService',
        'AuthStartupService'/*,
        'SnapshotService'*/
    ];

    private _storeFilterTags = [
        'ApplicationStore',
        'DanceStore',
        'EventStore',
        'VenueStore'
    ];

    private _pipeFilterTags = [/*
        'ImageSizePipe'
    */];

    private _pagesFilterTags = [

        'HomePage', /*
        'ProfilePage',
        */'SettingsPage', /*,
        'SideMenuPage',
        'VenuePage',*/
        'DanceListPage',
        'DanceDetailPage'
    ];

    private _componentFilterTags = [
        'PhotoButton',
        'PhotoModal',
        'PhotoWidget'
    ];

    private filterTags = this._serviceFilterTags.concat(this._storeFilterTags, this._pipeFilterTags, this._pagesFilterTags, this._componentFilterTags);

    public trace(message: Object, tags?: string[], obj?: any) {
        if (this._traceEnabled && this.allowAfterFilters(tags)) {

            if (obj !== undefined) {
                console.log(message + '###');
                console.log(obj);
            } else {
                console.log(message);
            }
        }
    }

    public debug(message: Object, tags?: string[]) {
        if (this._debugEnabled) {
            console.log(message);
        }
    }

    public log(message: Object, tags?: string[]) {
        console.log(message);
    }

    public error(message: Object, obj: any) {
        if (obj !== undefined) {
            console.log(message + '###');
            console.error(obj);
        } else {
            console.error(message);
        }
    }

    private allowAfterFilters(inTags): boolean {
        if (this.filterTags.length === 0) {
            return true;
        } else {
            let filterMatch = false;
            this.filterTags.forEach((filter) => {
                if (inTags) {
                    filterMatch = inTags.indexOf(filter) > -1 ? true : filterMatch;
                }
            });

            return filterMatch;
        }
    }
}

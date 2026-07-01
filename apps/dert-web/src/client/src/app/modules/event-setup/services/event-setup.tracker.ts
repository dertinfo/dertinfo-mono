// Angular & RxJS
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

// Types
import { InformationPanelData } from 'app/models/app';
import { UserData } from 'app/models/auth/userdata.model';
import { EventOverviewDto } from 'app/models/dto';

// Requires
declare function require(url: string);
const help = require('../../../../assets/help/app-event-configure.en.json');

@Injectable()
export class EventSetupTracker {

    // #region Observables and Subjects

    private _userData = new BehaviorSubject<UserData>(null);
    private _eventOverview = new BehaviorSubject<EventOverviewDto>(null);
    private _configurationHelp = new BehaviorSubject<InformationPanelData>(help['event-details']);

    get userData$() { return this._userData.asObservable(); }
    get eventOverview$() { return this._eventOverview.asObservable(); }
    get configurationHelp$() { return this._configurationHelp.asObservable(); }

    // #endregion

    private _memoryStore: {
        overview: EventOverviewDto
        userData: UserData,
        infoPanelData: InformationPanelData
    };

    // #region Properties Getters

    public get eventId(): number {
        return this._memoryStore.overview.id;
    }

    public get overview() {
        return this._memoryStore.overview;
    }

    // #endregion

    // #region Properties Setters

    public set overview(overview: EventOverviewDto) {
        this._memoryStore.overview = overview;
        this._eventOverview.next(this._memoryStore.overview);
    }

    public set userData(userData: UserData) {
        this._memoryStore.userData = userData;
        this._userData.next(this._memoryStore.userData);
    }

    public set help(infoPanelData: InformationPanelData) {
        this._memoryStore.infoPanelData = infoPanelData;
        this._configurationHelp.next(this._memoryStore.infoPanelData);
    }

    // #endregion

    // #region Properties hasLoaded()

    public hasLoadedOverview(): boolean {
        return this._memoryStore.overview !== null;
    }

    public hasLoadedUserData(): boolean {
        return this._memoryStore.userData !== null;
    }

    // #endregion

    public reset() {

        this._memoryStore = {
            overview: null,
            userData: null,
            infoPanelData: help['event-details']
        };

        this._userData.next(null);
        this._eventOverview.next(null);
        this._configurationHelp.next(help['event-details']);

    }
}

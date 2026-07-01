// Angular & RxJS
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

// Types
import { InformationPanelData } from 'app/models/app';
import { UserData } from 'app/models/auth/userdata.model';
import { GroupOverviewDto } from 'app/models/dto';

// Requires
declare function require(url: string);
const help = require('../../../../assets/help/app-group-configure.en.json');

@Injectable()
export class GroupSetupTracker {

    // #region Observables and Subjects

    private _userData = new BehaviorSubject<UserData>(null);
    private _groupOverview = new BehaviorSubject<GroupOverviewDto>(null);
    private _configurationHelp = new BehaviorSubject<InformationPanelData>(help['group-details']);

    get userData$() { return this._userData.asObservable(); }
    get groupOverview$() { return this._groupOverview.asObservable(); }
    get configurationHelp$() { return this._configurationHelp.asObservable(); }

    // #endregion

    private _memoryStore: {
        overview: GroupOverviewDto
        userData: UserData,
        infoPanelData: InformationPanelData
    };

    // #region Properties Getters

    public get groupId(): number {
        return this._memoryStore.overview.id;
    }

    public get overview() {
        return this._memoryStore.overview;
    }

    // #endregion

    // #region Properties Setters

    public set overview(overview: GroupOverviewDto) {
        this._memoryStore.overview = overview;
        this._groupOverview.next(this._memoryStore.overview);
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
            infoPanelData: help['group-details']
        };

        this._userData.next(null);
        this._groupOverview.next(null);
        this._configurationHelp.next(help['group-details']);

    }
}

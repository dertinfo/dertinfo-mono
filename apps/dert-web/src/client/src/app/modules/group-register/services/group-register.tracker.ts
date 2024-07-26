// Angular & RxJS
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

// Types
import { InformationPanelData } from 'app/models/app';
import { UserData } from 'app/models/auth/userdata.model';
import { EventDto, GroupDto } from 'app/models/dto';

// Requires
declare function require(url: string);
const help = require('../../../../assets/help/app-group-register.en.json');

@Injectable()
export class GroupRegisterTracker {

    // #region Observables and Subjects

    private _userData = new BehaviorSubject<UserData>(null);
    private _groupDto = new BehaviorSubject<GroupDto>(null);
    private _eventDto = new BehaviorSubject<EventDto>(null);
    private _configurationHelp = new BehaviorSubject<InformationPanelData>(help['event-details']);

    get userData$() { return this._userData.asObservable(); }
    get groupDto$() { return this._groupDto.asObservable(); }
    get eventDto$() { return this._eventDto.asObservable(); }
    get configurationHelp$() { return this._configurationHelp.asObservable(); }

    // #endregion

    private _memoryStore: {
        userData: UserData,
        groupDto: GroupDto,
        eventDto: EventDto,
        infoPanelData: InformationPanelData
    };

    // #region Properties Getters

    public get groupId(): number {
        return this._memoryStore.groupDto.id;
    }

    public get eventId(): number {
        return this._memoryStore.eventDto.id;
    }

    public get groupDto(): GroupDto {
        return this._memoryStore.groupDto;
    }

    public get eventDto(): EventDto {
        return this._memoryStore.eventDto;
    }

    // #endregion

    // #region Properties Setters

    public set groupDto(groupDto: GroupDto) {
        this._memoryStore.groupDto = groupDto;
        this._groupDto.next(this._memoryStore.groupDto);
    }

    public set eventDto(eventDto: EventDto) {
        this._memoryStore.eventDto = eventDto;
        this._eventDto.next(this._memoryStore.eventDto);
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

    public hasLoadedGroupDto(): boolean {
        return this._memoryStore.groupDto !== null;
    }

    public hasLoadedEventDto(): boolean {
        return this._memoryStore.eventDto !== null;
    }

    public hasLoadedUserData(): boolean {
        return this._memoryStore.userData !== null;
    }

    // #endregion

    public reset() {

        this._memoryStore = {
            groupDto: null,
            eventDto: null,
            userData: null,
            infoPanelData: help['event-details']
        };

        this._userData.next(null);
        this._groupDto.next(null);
        this._eventDto.next(null);
        this._configurationHelp.next(help['event-details']);

    }

}

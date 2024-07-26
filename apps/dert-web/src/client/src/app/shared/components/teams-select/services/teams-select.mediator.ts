// Angular & RxJs
import { Injectable } from '@angular/core';
import { BehaviorSubject, Subject } from 'rxjs';

import { GroupTeamDto, GroupTeamSubmissionDto } from 'app/models/dto';

@Injectable()
export class TeamSelectMediator {

    // The subjects on the child side need to be behaviour subjects as the parent may populate before the child subscription
    private _fullSetSubject: BehaviorSubject<GroupTeamDto[]> = new BehaviorSubject<GroupTeamDto[]>([]);
    private _selectedSetSubject: BehaviorSubject<GroupTeamDto[]> = new BehaviorSubject<GroupTeamDto[]>([]);

    private _newItemSubject: Subject<GroupTeamSubmissionDto> = new Subject<GroupTeamSubmissionDto>();
    private _changedItemsSubject: Subject<GroupTeamDto[]> = new Subject<GroupTeamDto[]>();

    private _closeModalSubject: Subject<null> = new Subject<null>();

    public fullSetChange$ = this._fullSetSubject.asObservable();
    public selectedChange$ = this._selectedSetSubject.asObservable();

    public newItemChange$ = this._newItemSubject.asObservable();
    public itemsChange$ = this._changedItemsSubject.asObservable();

    public closeModal$ = this._closeModalSubject.asObservable();

    public showSelect = true;
    public showCreate = true;

    constructor() {

    }

    public announceNew(myNew: GroupTeamSubmissionDto) {
        this._newItemSubject.next(myNew);
    }

    public announceChanges(myEdited: GroupTeamDto[]) {
        this._changedItemsSubject.next(myEdited);
    }

    public announceCloseModal() {
        this._closeModalSubject.next();
    }

    public applyFullSet(all: GroupTeamDto[]) {
        this._fullSetSubject.next(all);
    }

    public applySelectedSet(selected: GroupTeamDto[]) {
        this._selectedSetSubject.next(selected);
    }

    public hideSelect() {
        this.showSelect = false;
    }

    public hideCreate() {
        this.showCreate = false;
    }

    public close() {
        this.showCreate = false;
    }
}

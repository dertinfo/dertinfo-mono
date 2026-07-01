// Angular & RxJs
import { Injectable } from '@angular/core';
import { BehaviorSubject ,  Observable ,  Subject } from 'rxjs';

import { GroupMemberDto, GroupMemberSubmissionDto } from 'app/models/dto';

@Injectable()
export class MemberSelectMediator {

    // The subjects on the child side need to be behaviour subjects as the parent may populate before the child subscription
    private _fullSetSubject: BehaviorSubject<GroupMemberDto[]> = new BehaviorSubject<GroupMemberDto[]>([]);
    private _selectedSetSubject: BehaviorSubject<GroupMemberDto[]> = new BehaviorSubject<GroupMemberDto[]>([]);

    private _newItemSubject: Subject<GroupMemberSubmissionDto> = new Subject<GroupMemberSubmissionDto>();
    private _changedItemsSubject: Subject<GroupMemberDto[]> = new Subject<GroupMemberDto[]>();

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

    public announceNew(myNew: GroupMemberSubmissionDto) {
        this._newItemSubject.next(myNew);
    }

    public announceChanges(myEdited: GroupMemberDto[]) {
        this._changedItemsSubject.next(myEdited);
    }

    public announceCloseModal() {
        this._closeModalSubject.next();
    }

    public applyFullSet(all: GroupMemberDto[]) {
        this._fullSetSubject.next(all);
    }

    public applySelectedSet(selected: GroupMemberDto[]) {
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

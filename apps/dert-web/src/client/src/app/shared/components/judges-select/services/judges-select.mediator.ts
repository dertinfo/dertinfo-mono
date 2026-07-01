// Angular & RxJs
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subject } from 'rxjs';

import { JudgeDto, JudgeSubmissionDto } from 'app/models/dto';

@Injectable()
export class JudgesSelectMediator {

    // The subjects on the child side need to be behaviour subjects as the parent may populate before the child subscription
    private _fullSetSubject: BehaviorSubject<JudgeDto[]> = new BehaviorSubject<JudgeDto[]>([]);
    private _selectedSetSubject: BehaviorSubject<JudgeDto[]> = new BehaviorSubject<JudgeDto[]>([]);

    private _newItemSubject: Subject<JudgeSubmissionDto> = new Subject<JudgeSubmissionDto>();
    private _changedItemsSubject: Subject<JudgeDto[]> = new Subject<JudgeDto[]>();

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

    public announceNew(myNew: JudgeSubmissionDto) {
        this._newItemSubject.next(myNew);
    }

    public announceChanges(myEdited: JudgeDto[]) {
        this._changedItemsSubject.next(myEdited);
    }

    public announceCloseModal() {
        this._closeModalSubject.next();
    }

    public applyFullSet(all: JudgeDto[]) {
        this._fullSetSubject.next(all);
    }

    public applySelectedSet(selected: JudgeDto[]) {
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

// Angular & RxJs
import { Injectable } from '@angular/core';
import { BehaviorSubject ,  Observable ,  Subject } from 'rxjs';

import { VenueDto, VenueSubmissionDto } from 'app/models/dto';

@Injectable()
export class VenuesSelectMediator {

    // The subjects on the child side need to be behaviour subjects as the parent may populate before the child subscription
    private _fullSetSubject: BehaviorSubject<VenueDto[]> = new BehaviorSubject<VenueDto[]>([]);
    private _selectedSetSubject: BehaviorSubject<VenueDto[]> = new BehaviorSubject<VenueDto[]>([]);

    private _newItemSubject: Subject<VenueSubmissionDto> = new Subject<VenueSubmissionDto>();
    private _changedItemsSubject: Subject<VenueDto[]> = new Subject<VenueDto[]>();

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

    public announceNew(myNew: VenueSubmissionDto) {
        this._newItemSubject.next(myNew);
    }

    public announceChanges(myEdited: VenueDto[]) {
        this._changedItemsSubject.next(myEdited);
    }

    public announceCloseModal() {
        this._closeModalSubject.next();
    }

    public applyFullSet(all: VenueDto[]) {
        this._fullSetSubject.next(all);
    }

    public applySelectedSet(selected: VenueDto[]) {
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

import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve } from '@angular/router';
import { Observable } from 'rxjs';
import { GroupViewRepository } from './group-view.repository';

@Injectable()
export class GroupViewDodResolver implements Resolve<Observable<boolean>> {
    constructor(private _groupRepo: GroupViewRepository) { }

    resolve(activatedRoute: ActivatedRouteSnapshot) {
        const id = parseInt(activatedRoute.paramMap.get('id'), 10);
        return this._groupRepo.hasEnteredDertOfDerts(id);
    }
}

import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve } from '@angular/router';
import { Observable } from 'rxjs';
import { GroupAdminRepository } from './group-admin.repository';

@Injectable()
export class GroupAdminDodResolver implements Resolve<Observable<boolean>> {
    constructor(private _groupRepo: GroupAdminRepository) { }

    resolve(activatedRoute: ActivatedRouteSnapshot) {
        const id = parseInt(activatedRoute.paramMap.get('id'), 10);
        return this._groupRepo.hasEnteredDertOfDerts(id);
    }
}

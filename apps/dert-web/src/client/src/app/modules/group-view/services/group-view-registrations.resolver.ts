import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve } from '@angular/router';
import { Observable } from 'rxjs';

import { GroupRegistrationDto } from 'app/models/dto';
import { GroupViewRepository } from './group-view.repository';

@Injectable()
export class GroupViewRegistrationsResolver implements Resolve<Observable<GroupRegistrationDto[]>> {
    constructor(private _groupRepo: GroupViewRepository) { }

    resolve(activatedRoute: ActivatedRouteSnapshot) {
        const id = parseInt(activatedRoute.paramMap.get('id'), 10);
        return this._groupRepo.registrations(id);
    }
}

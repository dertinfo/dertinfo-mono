import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot } from '@angular/router';
import { Observable } from 'rxjs';

import { GroupRegistrationDto } from 'app/models/dto';
import { GroupViewRepository } from './group-view.repository';

@Injectable()
export class GroupViewRegistrationsResolver  {
    constructor(private _groupRepo: GroupViewRepository) { }

    resolve(activatedRoute: ActivatedRouteSnapshot) {
        const id = parseInt(activatedRoute.paramMap.get('id'), 10);
        return this._groupRepo.registrations(id);
    }
}

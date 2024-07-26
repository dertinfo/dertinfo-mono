import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve } from '@angular/router';
import { Observable } from 'rxjs';

import { GroupRegistrationDto, GroupRegistrationOverviewDto } from 'app/models/dto';
import { RegistrationRepository } from '../../repositories';

import { GroupRegistrationOverview } from 'app/models/app';

@Injectable()
export class RegistrationByGroupResolver implements Resolve<Observable<GroupRegistrationOverview>> {
    constructor(
        private _registrationRepo: RegistrationRepository,

    ) { }

    resolve(activatedRoute: ActivatedRouteSnapshot) {

        const id = parseInt(activatedRoute.paramMap.get('id'), 10);
        return this._registrationRepo.getGroupRegistrationOverview(id);
    }
}

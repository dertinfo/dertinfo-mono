import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve } from '@angular/router';
import { EventRegistrationOverview } from 'app/models/app';
import { RegistrationRepository } from 'app/modules/repositories/repositories/registration.repository';
import { Observable } from 'rxjs';

@Injectable()
export class RegistrationByEventsResolver implements Resolve<Observable<EventRegistrationOverview>> {
    constructor(
        private _registrationRepo: RegistrationRepository,

    ) { }

    resolve(activatedRoute: ActivatedRouteSnapshot) {

        const id = parseInt(activatedRoute.paramMap.get('id'), 10);
        return this._registrationRepo.getEventRegistrationOverview(id);
    }
}

// Angular
import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot } from '@angular/router';
import { Observable, of } from 'rxjs';

// Types
import { RegistrationRepository } from 'app/modules/repositories';
import { GroupViewTracker } from '../../services/group-view.tracker';

@Injectable()
export class RegistrationResolver  {
    constructor(
        private _registrationRepo: RegistrationRepository,
        private _tracker: GroupViewTracker,

    ) { }

    resolve(activatedRoute: ActivatedRouteSnapshot) {

        const registrationId = parseInt(activatedRoute.paramMap.get('registrationId'), 10);

        if (this._tracker.hasLoadedTeamAttendances(registrationId)) {
            return of(this._tracker.getTeamAttendanceForRegistration(registrationId));
        } else {
            return this._registrationRepo.getAttendingTeams(registrationId);
        }
    }
}

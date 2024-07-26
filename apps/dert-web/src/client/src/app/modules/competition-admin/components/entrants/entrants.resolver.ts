// Angular
import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve } from '@angular/router';
import { Observable, of } from 'rxjs';

// Types
import { CompetitionRepository } from 'app/modules/repositories';
import { CompetitionAdminTracker } from '../../services/competition-admin.tracker';

@Injectable()
export class EntrantsResolver implements Resolve<Observable<any>> {
    constructor(
        private _competitionRepo: CompetitionRepository,
        private _tracker: CompetitionAdminTracker,

    ) { }

    resolve(activatedRoute: ActivatedRouteSnapshot) {

        const id = parseInt(activatedRoute.parent.paramMap.get('id'), 10);

        if (this._tracker.hasLoadedEntrants() && this._tracker.competitionId === id) {

            return of(this._tracker.entrants);

        } else {

            const entrants$ = this._competitionRepo.entrants(id);

            return entrants$;
        }
    }
}

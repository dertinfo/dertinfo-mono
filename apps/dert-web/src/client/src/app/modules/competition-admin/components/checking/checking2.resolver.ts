// Angular
import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve } from '@angular/router';
import { Observable, of } from 'rxjs';

// Types
import { CompetitionRepository } from 'app/modules/repositories';
import { CompetitionAdminTracker } from '../../services/competition-admin.tracker';

@Injectable()
export class CheckingResolver2 implements Resolve<Observable<any>> {
    constructor(
        private _competitionRepo: CompetitionRepository,
        private _tracker: CompetitionAdminTracker,

    ) { }

    resolve(activatedRoute: ActivatedRouteSnapshot) {

        const id = parseInt(activatedRoute.parent.paramMap.get('id'), 10);
        const danceId = parseInt(activatedRoute.paramMap.get('danceId'), 10);

        if (this._tracker.hasLoadedJudgeSlotInformation(danceId) && this._tracker.competitionId === id) {

            return of(this._tracker.judgeSlotInformation[danceId]);

        } else {

            const danceSplit$ = this._competitionRepo.judgeSlotInformation(danceId);

            return danceSplit$;
        }
    }
}

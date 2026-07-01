
// Angular
import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve } from '@angular/router';
import { forkJoin ,  Observable, of } from 'rxjs';
import {map} from 'rxjs/operators';

// Types
import { CompetitionSettingsDto } from 'app/models/dto';
import { CompetitionRepository } from 'app/modules/repositories';
import { CompetitionAdminTracker } from '../../services/competition-admin.tracker';

@Injectable()
export class ScoringResolver implements Resolve<Observable<any>> {
    constructor(
        private _competitionRepo: CompetitionRepository,
        private _tracker: CompetitionAdminTracker,

    ) { }

    resolve(activatedRoute: ActivatedRouteSnapshot) {

        const id = parseInt(activatedRoute.parent.paramMap.get('id'), 10);

        if (this._tracker.hasLoadedScoreSets() && this._tracker.hasLoadedScoreCategories() && this._tracker.competitionId === id) {
            return of(
                {
                    scoreSets: this._tracker.scoreSets,
                    scoreCategories: this._tracker.scoreCategories
                }
            );
        } else {

            const sets$ = this._competitionRepo.scoresets(id);
            const categories$ = this._competitionRepo.scorecategories(id);

            const obsAll = forkJoin([sets$, categories$]);

            return obsAll.pipe(map((data: any[]) => {
                return {
                    scoreSets: data[0],
                    scoreCategories: data[1]
                };
            }));
        }
    }
}

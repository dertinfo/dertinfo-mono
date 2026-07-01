// Angular
import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve } from '@angular/router';
import { Observable } from 'rxjs';

import { CompetitionRepository } from '../../repositories';

@Injectable()
export class CompetitionAdminResolver implements Resolve<Observable<any>> {
    constructor(
        private _competitionRepo: CompetitionRepository,

    ) { }

    resolve(activatedRoute: ActivatedRouteSnapshot) {

        const id = parseInt(activatedRoute.paramMap.get('id'), 10);
        return this._competitionRepo.overview(id);
    }
}

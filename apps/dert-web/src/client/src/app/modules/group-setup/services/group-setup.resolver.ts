import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot } from '@angular/router';
import { GroupOverviewDto } from 'app/models/dto';
import { Observable } from 'rxjs';
import { GroupSetupRepository } from './group-setup.repository';

@Injectable()
export class GroupSetupResolver  {
    constructor(
        private _groupRepo: GroupSetupRepository,
    ) { }

    resolve(activatedRoute: ActivatedRouteSnapshot) {

        const id = parseInt(activatedRoute.paramMap.get('id'), 10);
        return this._groupRepo.overview(id);
    }
}

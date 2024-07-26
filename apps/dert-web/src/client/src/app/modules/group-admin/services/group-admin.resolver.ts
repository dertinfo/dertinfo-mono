import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve } from '@angular/router';

import { GroupOverviewDto } from 'app/models/dto';
import { Observable } from 'rxjs';
import { GroupAdminRepository } from './group-admin.repository';

@Injectable()
export class GroupAdminResolver implements Resolve<Observable<GroupOverviewDto>> {
    constructor(private _groupRepo: GroupAdminRepository) { }

    resolve(activatedRoute: ActivatedRouteSnapshot) {
        const id = parseInt(activatedRoute.paramMap.get('id'), 10);
        return this._groupRepo.overview(id);
    }
}

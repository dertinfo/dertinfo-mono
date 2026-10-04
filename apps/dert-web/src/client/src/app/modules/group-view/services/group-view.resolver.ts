import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot } from '@angular/router';
import { GroupDto } from 'app/models/dto';
import { Observable } from 'rxjs';
import { GroupViewRepository } from './group-view.repository';

@Injectable()
export class GroupViewResolver  {
    constructor(private _groupRepo: GroupViewRepository) { }

    resolve(activatedRoute: ActivatedRouteSnapshot) {
        const id = parseInt(activatedRoute.paramMap.get('id'), 10);
        return this._groupRepo.get(id);
    }
}

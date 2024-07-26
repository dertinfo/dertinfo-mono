import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve } from '@angular/router';
import { Observable } from 'rxjs';

import { EventOverviewDto } from 'app/models/dto';
import { EventRepository } from '../../repositories';

@Injectable()
export class EventAdminResolver implements Resolve<Observable<EventOverviewDto>> {
    constructor(private _eventRepo: EventRepository) { }

    resolve(activatedRoute: ActivatedRouteSnapshot) {
        const id = parseInt(activatedRoute.paramMap.get('id'), 10);
        return this._eventRepo.overview(id);
    }
}

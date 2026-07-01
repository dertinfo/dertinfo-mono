import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve } from '@angular/router';
import { Observable } from 'rxjs';

import { EventOverviewDto } from 'app/models/dto';
import { EventRepository } from 'app/modules/repositories/repositories/event.repository';

@Injectable()
export class EventSetupResolver implements Resolve<Observable<EventOverviewDto>> {
    constructor(
        private _eventRepo: EventRepository,

    ) { }

    resolve(activatedRoute: ActivatedRouteSnapshot) {

        const id = parseInt(activatedRoute.paramMap.get('id'), 10);
        return this._eventRepo.overview(id);
    }
}

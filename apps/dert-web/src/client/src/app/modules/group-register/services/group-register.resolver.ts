
import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve } from '@angular/router';
import { EventDto, GroupDto } from 'app/models/dto';
import { EventRepository } from 'app/modules/repositories/repositories/event.repository';
import { forkJoin, Observable } from 'rxjs';
import { GroupRegisterRepository } from './group-register.repository';

@Injectable()
export class GroupRegisterResolver implements Resolve<Observable<[GroupDto, EventDto]>> {
    constructor(
        private _groupRepo: GroupRegisterRepository,
        private _eventRepo: EventRepository,

    ) { }

    resolve(activatedRoute: ActivatedRouteSnapshot) {

        const groupId = parseInt(activatedRoute.paramMap.get('groupId'), 10);
        const eventId = parseInt(activatedRoute.paramMap.get('eventId'), 10);

        const groupDto$ = this._groupRepo.get(groupId);
        const eventDto$ = this._eventRepo.get(eventId);

        return forkJoin([groupDto$, eventDto$]);
    }
}

export class GroupRegisterOverview {
    groupDto: GroupDto;
    eventDto: EventDto;
}

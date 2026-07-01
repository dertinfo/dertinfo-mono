// Angular
import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve } from '@angular/router';
import { Observable, of } from 'rxjs';

// Types
import { EventRepository } from 'app/modules/repositories';
import { EventAdminTracker } from '../../services/event-admin.tracker';

@Injectable()
export class EmailTemplatesResolver implements Resolve<Observable<any>> {
    constructor(
        private _eventRepo: EventRepository,
        private _tracker: EventAdminTracker,

    ) { }

    resolve(activatedRoute: ActivatedRouteSnapshot) {

        const eventId = parseInt(activatedRoute.parent.paramMap.get('id'), 10);

        if (this._tracker.hasEmailTemplates() && this._tracker.eventId === eventId) {

            return of(this._tracker.emailTemplates);

        } else {

            const emailTemplates$ = this._eventRepo.emailTemplates(eventId);

            return emailTemplates$;
        }
    }
}

// Angular
import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve } from '@angular/router';
import { Observable, of, zip } from 'rxjs';

import { GroupListCache } from 'app/modules/repositories';
import { EventListCache } from 'app/modules/repositories';

// Types

@Injectable()
export class DashboardResolver implements Resolve<Observable<any>> {
    constructor(
        private _groupListCache: GroupListCache,
        private _eventListCache: EventListCache
    ) { }

    resolve() {
        const groups$ = this._groupListCache.list();
        const events$ = this._eventListCache.list();

        const zipped$ =  zip(groups$, events$);

        return zipped$;
    }
}

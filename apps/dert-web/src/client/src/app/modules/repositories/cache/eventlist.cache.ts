import { Injectable } from '@angular/core';
import { EventDto } from 'app/models/dto';
import { Observable } from 'rxjs';
import { publishReplay, refCount } from 'rxjs/operators';
import { EventListRepository } from './eventlist.repository';

/**
 * Further information on this approach to caching can be found at:
 * - https://blog.angularindepth.com/fastest-way-to-cache-for-lazy-developers-angular-with-rxjs-444a198ed6a6
 */
@Injectable()
export class EventListCache {

    private _eventDtos: Observable<Array<EventDto>>;

    constructor(private _repository: EventListRepository) { }

    /**
     * Cache on the list response
     */
    public list(): Observable<Array<EventDto>> {

        if (!this._eventDtos) {
            this._eventDtos = this._repository.list().pipe(
                publishReplay(1), // this tells Rx to cache the latest emitted
                refCount() // and this tells Rx to keep the Observable alive as long as there are any Subscribers
            );
        }

        return this._eventDtos;
    }

    /**
     * Method to clear the cache.
     */
    public clearCache() {
        this._eventDtos = null;
    }
}

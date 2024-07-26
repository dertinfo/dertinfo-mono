import { Injectable } from '@angular/core';
import { GroupDto } from 'app/models/dto';
import { Observable } from 'rxjs';
import { publishReplay, refCount } from 'rxjs/operators';
import { GroupListRepository } from './grouplist.repository';

/**
 * Further information on this approach to caching can be found at:
 * - https://blog.angularindepth.com/fastest-way-to-cache-for-lazy-developers-angular-with-rxjs-444a198ed6a6
 */
@Injectable()
export class GroupListCache {

    private _groupDtos: Observable<Array<GroupDto>>;

    constructor(private _repository: GroupListRepository) { }

    /**
     * Cache on the list response
     */
    public list(): Observable<Array<GroupDto>> {

        if (!this._groupDtos) {
            this._groupDtos = this._repository.list().pipe(
                publishReplay(1), // this tells Rx to cache the latest emitted
                refCount() // and this tells Rx to keep the Observable alive as long as there are any Subscribers
            );
        }

        return this._groupDtos;
    }

    /**
     * Method to clear the cache.
     */
    public clearCache() {
        this._groupDtos = null;
    }
}

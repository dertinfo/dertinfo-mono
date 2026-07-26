import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from '@angular/router';
import { Observable, of } from 'rxjs';
import { WarmupService } from '../services/warmup.service';

/**
 * @deprecated Prefer WarmupGuard. Kept as a no-op warm check for any residual resolve: bindings.
 * Does not navigate — navigation from resolve caused the stuck-warmup race.
 */
@Injectable({ providedIn: 'root' })
export class WarmupResolver implements Resolve<Observable<any>> {
    constructor(
        private warmupService: WarmupService,
    ) { }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        return of(this.warmupService.isApiWarm());
    }
}

// Angular
import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve, Router, RouterStateSnapshot } from '@angular/router';
import { Observable, of } from 'rxjs';

import { WarmupService } from '../services/warmup.service';

// Types

@Injectable({ providedIn: 'root' })
export class WarmupResolver implements Resolve<Observable<any>> {
    constructor(
        private warmupService: WarmupService,
        private router: Router
    ) { }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {

        if (this.warmupService.isApiWarm()) {
            return of(true);
        } else {
            this.router.navigate(['/session/warmup'], {skipLocationChange: true});
            this.warmupService.giveItAKick(state);
            return of(false);
        }
    }
}

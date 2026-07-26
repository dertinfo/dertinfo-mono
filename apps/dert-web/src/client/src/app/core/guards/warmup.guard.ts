import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable, of } from 'rxjs';
import { WarmupService } from '../services/warmup.service';

/**
 * Warmup flow step 1 — CanActivate (not Resolve).
 *
 * Navigating from inside a resolver races the router and can leave the user stuck on
 * /session/warmup with sessionwarm already true. The guard owns a single redirect:
 * cold → store pending URL → go to warmup UI; warm → allow the route.
 */
@Injectable({ providedIn: 'root' })
export class WarmupGuard implements CanActivate {

    constructor(
        private warmupService: WarmupService,
        private router: Router,
    ) { }

    canActivate(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot,
    ): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {

        if (this.warmupService.isApiWarm()) {
            return true;
        }

        this.warmupService.setPendingUrl(state.url);
        return this.router.createUrlTree(['/session/warmup']);
    }
}

import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { AuthService as Auth0AngularService } from '@auth0/auth0-angular';
import { Observable } from 'rxjs';
import { filter, map, switchMap, take } from 'rxjs/operators';

/**
 * Protects authenticated routes.
 *
 * Must wait for Auth0 SDK bootstrap (`isLoading$` → false) before reading
 * `isAuthenticated$`. A sync check races page refresh: localStorage session
 * is still restoring, so the guard would wrongly send users to /auth/signin
 * (and Auth0 consent) on every reload.
 *
 * Unauthenticated users → /auth/signin → AuthService.login() → Universal Login.
 */
@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {

  constructor(
    private router: Router,
    private auth0: Auth0AngularService,
  ) { }

  canActivate(
    _route: ActivatedRouteSnapshot,
    _state: RouterStateSnapshot,
  ): Observable<boolean | UrlTree> {
    return this.auth0.isLoading$.pipe(
      filter((loading) => loading === false),
      take(1),
      switchMap(() => this.auth0.isAuthenticated$.pipe(take(1))),
      map((isAuthenticated) => {
        if (isAuthenticated) {
          return true;
        }
        return this.router.createUrlTree(['/auth/signin']);
      }),
    );
  }
}

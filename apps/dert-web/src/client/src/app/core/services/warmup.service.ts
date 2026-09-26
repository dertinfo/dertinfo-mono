import { Injectable, NgZone } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { AuthService } from 'app/core/authentication/auth.service';
import { ConfigurationService } from 'app/core/services/configuration.service';

/**
 * API cold-start warmup.
 *
 * Why: a cold Azure/API instance can leave the UI unresponsive for ~40s. We ping GET {api}/status
 * once per browser session before loading authenticated dashboard data.
 *
 * Flow (do NOT navigate from a Resolve — that races the router):
 *  1. WarmupGuard (CanActivate) sees cold → stores pending URL → navigates to /session/warmup.
 *  2. WarmupComponent / giveItAKick requires a usable access token, then calls GET /status.
 *  3. On success → sessionStorage.sessionwarm=true → navigateByUrl(pending, { replaceUrl: true }).
 *  4. Warm path → the same token check, then the guard allows activation.
 *
 * A dead Auth0 cache must not stay on this screen. The status call is on the Auth0 interceptor
 * allow-list, so a refresh token Auth0 will reject fails the kick before HTTP 200. That failure,
 * and a hung renew, clear the saved session and send the user to sign-in.
 */
@Injectable({ providedIn: 'root' })
export class WarmupService {

    private readonly tokenTimeoutMs = 10000;

    private _apiCalled: boolean = false;
    private _apiResponded: boolean = false;

    /** Destination to resume after a successful kick (set by WarmupGuard). */
    private _pendingUrl: string = '/dashboard';

    constructor(
        private configurationService: ConfigurationService,
        private http: HttpClient,
        private router: Router,
        private authService: AuthService,
        private zone: NgZone,
    ) { }

    public isApiWarm() {
        let isWarm = this._apiCalled && this._apiResponded;
        if (!isWarm) {
            isWarm = this.getSessionWarm();
        }

        return isWarm;
    }

    public getPendingUrl(): string {
        return this._pendingUrl || '/dashboard';
    }

    public setPendingUrl(url: string) {
        // Avoid treating the warmup route itself as the destination (refresh / direct visit).
        if (!url || url.indexOf('/session/warmup') === 0) {
            this._pendingUrl = '/dashboard';
            return;
        }
        this._pendingUrl = url;
    }

    /**
     * Require a usable access token, then kick the API and continue to the pending URL.
     */
    public giveItAKick(continueToUrl?: string) {
        const destination = continueToUrl || this.getPendingUrl();
        this.setPendingUrl(destination);
        this._apiCalled = true;

        this.requireAccessToken().then(
            () => this.kickApi(),
            () => this.recoverDeadSession(),
        );
    }

    /**
     * Already-warm path (refresh on /session/warmup). Still requires a usable access token
     * so a dead cache cannot skip Warmup and hang in the dashboard.
     */
    public continueWhenSessionValid(url?: string) {
        const target = url || this.getPendingUrl();
        this.requireAccessToken().then(
            () => {
                this.setSessionWarm();
                this.zone.run(() => this.router.navigateByUrl(target, { replaceUrl: true }));
            },
            () => this.recoverDeadSession(),
        );
    }

    public continueTo(url?: string) {
        this.continueWhenSessionValid(url);
    }

    private kickApi() {
        const url = this.configurationService.baseApiUrl + `/status`;
        const subs = this.http.get(url).subscribe({
            next: () => {
                console.log('Warmup kick succeeded');
                this._apiResponded = true;
                this.setSessionWarm();
                this.zone.run(() => this.router.navigateByUrl(this.getPendingUrl(), { replaceUrl: true }));
                subs.unsubscribe();
            },
            error: () => {
                console.log('Warmup kick failed');
                subs.unsubscribe();
            },
            complete: () => {
                console.log('Warmup kick completed');
            },
        });
    }

    private requireAccessToken(): Promise<void> {
        return new Promise((resolve, reject) => {
            let settled = false;
            const timer = setTimeout(() => {
                if (settled) {
                    return;
                }
                settled = true;
                reject(new Error('Warmup access token timed out'));
            }, this.tokenTimeoutMs);

            this.authService.ensureAccessTokenCached().then(
                () => {
                    if (settled) {
                        return;
                    }
                    settled = true;
                    clearTimeout(timer);
                    resolve();
                },
                () => {
                    if (settled) {
                        return;
                    }
                    settled = true;
                    clearTimeout(timer);
                    reject(new Error('Warmup access token failed'));
                },
            );
        });
    }

    private recoverDeadSession() {
        console.log('Warmup session cannot be renewed; clearing saved session');
        sessionStorage.removeItem('sessionwarm');
        this._apiResponded = false;
        this.zone.run(() => this.authService.abandonStaleSessionAndLogin());
    }

    private setSessionWarm() {
        sessionStorage.setItem('sessionwarm', JSON.stringify(true));
    }

    private getSessionWarm(): boolean {
        const sessionData = sessionStorage.getItem('sessionwarm');
        return sessionData ? JSON.parse(sessionData) : false;
    }
}

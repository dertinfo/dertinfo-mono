import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { ConfigurationService } from 'app/core/services/configuration.service';

/**
 * API cold-start warmup.
 *
 * Why: a cold Azure/API instance can leave the UI unresponsive for ~40s. We ping GET {api}/status
 * once per browser session before loading authenticated dashboard data.
 *
 * Flow (do NOT navigate from a Resolve — that races the router):
 *  1. WarmupGuard (CanActivate) sees cold → stores pending URL → navigates to /session/warmup.
 *  2. WarmupComponent / giveItAKick calls GET /status (unauthenticated).
 *  3. On success → sessionStorage.sessionwarm=true → navigateByUrl(pending, { replaceUrl: true }).
 *  4. Warm path → guard allows activation; dashboard resolvers run with a warm API.
 */
@Injectable({ providedIn: 'root' })
export class WarmupService {

    private _apiCalled: boolean = false;
    private _apiResponded: boolean = false;

    /** Destination to resume after a successful kick (set by WarmupGuard). */
    private _pendingUrl: string = '/dashboard';

    constructor(
        private configurationService: ConfigurationService,
        private http: HttpClient,
        private router: Router,
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
     * Kick the API and continue to the pending URL (or continueToUrl if provided).
     */
    public giveItAKick(continueToUrl?: string) {
        const destination = continueToUrl || this.getPendingUrl();
        this.setPendingUrl(destination);

        const url = this.configurationService.baseApiUrl + `/status`;
        this._apiCalled = true;

        const subs = this.http.get(url).subscribe({
            next: () => {
                console.log('Warmup kick succeeded');
                this._apiResponded = true;
                this.setSessionWarm();
                this.continueTo(this.getPendingUrl());
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

    public continueTo(url?: string) {
        const target = url || this.getPendingUrl();
        this.router.navigateByUrl(target, { replaceUrl: true });
    }

    private setSessionWarm() {
        sessionStorage.setItem('sessionwarm', JSON.stringify(true));
    }

    private getSessionWarm(): boolean {
        const sessionData = sessionStorage.getItem('sessionwarm');
        return sessionData ? JSON.parse(sessionData) : false;
    }
}

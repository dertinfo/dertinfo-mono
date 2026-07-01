import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router, RouterStateSnapshot } from '@angular/router';
import { ConfigurationService } from 'app/core/services/configuration.service';

/**
 * This service hold the state as to whether the Api has been called in this session.
 * If the Api is cold then the user interface appears unresponsive for approximately 40s.
 * THis happens when the API is completely cold. To combat this then if the API has not
 * been called in this session we'll redirect the user to a page that states getting things ready
 * This will give time for the api to warm up. Rather than be non responsive.
 *
 * This service holds the state as to whether the api has been called or not.
 */
@Injectable({ providedIn: 'root' })
export class WarmupService {

    private _apiCalled: boolean = false;
    private _apiResponded: boolean = false;

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

    public giveItAKick(continueTo: RouterStateSnapshot) {
        const url = this.configurationService.baseApiUrl + `/status`;
        const obs$ = this.http.get(url);

        this._apiCalled = true;

        const subs = obs$.subscribe(
            () => {
                console.log('Warmup kick succeeded');
                this._apiResponded = true;
                this.setSessionWarm();
                this.continueTo(continueTo);
                subs.unsubscribe();
            },
            () => {
                console.log('Warmup kick failed');
                subs.unsubscribe();
            },
            () => {
                console.log('Warmup kick completed');
            }
        );
    }

    private continueTo(continueTo: RouterStateSnapshot) {
        this.router.navigateByUrl(continueTo.url);
    }

    private setSessionWarm() {
        sessionStorage.setItem('sessionwarm', JSON.stringify(true));
    }

    private getSessionWarm(): boolean {
        const sessionData = sessionStorage.getItem('sessionwarm');
        return sessionData ? JSON.parse(sessionData) : false;

    }
}

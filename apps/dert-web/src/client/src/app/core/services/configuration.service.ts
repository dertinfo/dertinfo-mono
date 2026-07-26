import { Injectable } from '@angular/core';
import { HttpBackend, HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export class EnvironmentConfig {
    apiUrl: string;
    auth0CallbackUrl: string;
    auth0ClientId: string;
    auth0Audience: string;
    auth0TenantDomain: string;
    appInsightsTelemetryKey: string;
    allowedDomains: Array<string>;
}

/**
 * Loads environment-specific settings used by Auth0 and the API.
 *
 * Local: assets/app.config.json (callback http://localhost:44200).
 * Staging/prod builds: environment.test.ts / environment.prod.ts (production flag true).
 * Auth0 client id / domain / audience: always from GET {apiUrl}/clientconfiguration/web.
 *
 * AuthClientConfig.set in app.module APP_INITIALIZER consumes these values after loadConfig resolves.
 *
 * Important: bootstrap config HTTP must use HttpBackend (no interceptors). Otherwise AuthHttpInterceptor
 * can deadlock APP_INITIALIZER while Auth0 config is still being loaded.
 */
@Injectable({ providedIn: 'root' })
export class ConfigurationService {

    /** HttpClient that bypasses HTTP_INTERCEPTORS — used only during bootstrap config load. */
    private readonly rawHttp: HttpClient;

    private config: EnvironmentConfig = {
        // All values are replaced at runtime
        apiUrl: '',
        auth0CallbackUrl: '',
        auth0ClientId: '',
        auth0Audience: '',
        auth0TenantDomain: '',
        appInsightsTelemetryKey: '',
        allowedDomains: []
    };

    constructor(httpBackend: HttpBackend) {
        this.rawHttp = new HttpClient(httpBackend);
    }

    public get baseApiUrl(): string {
        return this.config.apiUrl;
    }

    public get auth0CallbackUrl(): string {
        return this.config.auth0CallbackUrl;
    }

    public get auth0ClientId(): string {
        return this.config.auth0ClientId;
    }

    public get auth0Audience(): string {
        return this.config.auth0Audience;
    }

    public get auth0TenantDomain(): string {
        return this.config.auth0TenantDomain;
    }

    public get appInsightsTelemetryKey(): string {
        return this.config.appInsightsTelemetryKey;
    }

    public get configuration(): EnvironmentConfig {
        return this.config;
    }

    public get allowedDomains(): Array<string> {
        return this.config.allowedDomains;
    }

    public loadConfig(): Promise<EnvironmentConfig> {

        console.log('Loading configuration...');

        const localConfigLoaded$ = new Observable<EnvironmentConfig>(observer => {

            if (environment.production) {

                // Staging (environment.test) and production (environment.prod) — build-time file replacements.
                console.log('Applying production/staging configuration');
                this.config.apiUrl = environment.apiUrl;
                this.config.auth0CallbackUrl = environment.auth0CallbackUrl;
                this.config.allowedDomains = environment.allowedDomains;
                observer.next(this.config);
                observer.complete();

            } else {

                // Local / Codespaces — runtime JSON (callback ports fixed for Auth0 Allowed Callback URLs).
                console.log('Loading Local/Codespaces configuration');
                this.getLocalConfiguration().subscribe({
                    next: (localData) => {
                        console.log('Applying Local/Codespaces Configuration');
                        this.config.apiUrl = localData.apiUrl;
                        this.config.auth0CallbackUrl = localData.auth0CallbackUrl;
                        this.config.allowedDomains = localData.allowedDomains;
                        observer.next(this.config);
                        observer.complete();
                    },
                    error: (err) => observer.error(err),
                });
            }
        });

        return new Promise((resolve, reject) => {

            localConfigLoaded$.subscribe({
                next: () => {

                    // Remote Auth0 ids — same endpoint for all environments; values differ by API App Config / secrets.
                    console.log('Loading Remote Configuration');
                    const subs = this.getRemoteConfiguration().subscribe({
                        next: (remoteData) => {
                            console.log('Applying Remote Configuration');
                            this.config.appInsightsTelemetryKey = remoteData['appInsightsTelemetryKey'];
                            this.config.auth0Audience = remoteData['auth0Audience'];
                            this.config.auth0ClientId = remoteData['auth0ClientId'];
                            this.config.auth0TenantDomain = remoteData['auth0TenantDomain'];

                            subs.unsubscribe();
                            resolve(this.config);
                        },
                        error: (err) => {
                            subs.unsubscribe();
                            reject(err);
                        },
                    });
                },
                error: (err) => reject(err),
            });
        });
    }

    public getLocalConfiguration(): Observable<any> {
        return this.rawHttp.get('assets/app.config.json');
    }

    public getRemoteConfiguration(): Observable<any> {
        return this.rawHttp.get(`${this.config.apiUrl}/clientconfiguration/web`);
    }
}

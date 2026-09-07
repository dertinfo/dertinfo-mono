import { Injectable } from '@angular/core';
import { HttpBackend, HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

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
 * API base, Auth0 callback, and allowed domains: always assets/app.config.json
 * (checked-in local values, or overwritten in dist by CD per GitHub Environment).
 * Auth0 client id / domain / audience: GET {apiUrl}/clientconfiguration/web.
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

        return new Promise((resolve, reject) => {

            this.getRuntimeConfiguration().subscribe({
                next: (runtimeData) => {
                    console.log('Applying runtime configuration from assets/app.config.json');
                    this.config.apiUrl = runtimeData.apiUrl;
                    this.config.auth0CallbackUrl = runtimeData.auth0CallbackUrl;
                    this.config.allowedDomains = runtimeData.allowedDomains;

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

    public getRuntimeConfiguration(): Observable<any> {
        return this.rawHttp.get('assets/app.config.json');
    }

    public getRemoteConfiguration(): Observable<any> {
        return this.rawHttp.get(`${this.config.apiUrl}/clientconfiguration/web`);
    }
}

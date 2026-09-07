import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { jwtOptions } from '../jwt-utils';

export class EnvironmentConfig {
    apiUrl: string;
    auth0CallbackUrl: string;
    auth0ClientId: string;
    auth0Audience: string;
    auth0TenantDomain: string;
    appInsightsTelemetryKey: string;
    allowedDomains: Array<string>;
}

@Injectable({ providedIn: 'root' })
export class ConfigurationService {

    private config: EnvironmentConfig = {
        apiUrl: '',
        auth0CallbackUrl: '',
        auth0ClientId: '',
        auth0Audience: '',
        auth0TenantDomain: '',
        appInsightsTelemetryKey: '',
        allowedDomains: []
    };

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

    public loadConfig(http): Promise<EnvironmentConfig> {

        console.log('Loading configuration...');

        return new Promise((resolve, reject) => {

            this.getRuntimeConfiguration(http).subscribe({
                next: (runtimeData) => {
                    console.log('Applying runtime configuration from assets/app.config.json');
                    this.config.apiUrl = runtimeData.apiUrl;
                    this.config.auth0CallbackUrl = runtimeData.auth0CallbackUrl;
                    this.config.allowedDomains = runtimeData.allowedDomains;

                    console.log('Assigning Allowed Domains');
                    const newDomains = this.config.allowedDomains.map(domain => domain.toLowerCase());
                    jwtOptions.updateAllowedDomains(newDomains);

                    console.log('Loading Remote Configuration');
                    const subs = this.getRemoteConfiguration(http).subscribe({
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

    public getRuntimeConfiguration(http): Observable<any> {
        return http.get('assets/app.config.json');
    }

    public getRemoteConfiguration(http): Observable<any> {
        return http.get(`${this.config.apiUrl}/clientconfiguration/app`);
    }
}

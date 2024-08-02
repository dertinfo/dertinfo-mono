import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { environment } from '../../environments/environment';

export class EnvironmentConfig {
    apiUrl: string;
    auth0CallbackUrl: string;
    auth0ClientId: string;
    auth0Audience: string;
    auth0TenantDomain: string;
    appInsightsTelemetryKey: string;
}

@Injectable({ providedIn: 'root' })
export class ConfigurationService {

    private config: EnvironmentConfig = {
        // All values are replaced at runtime
        apiUrl: '',
        auth0CallbackUrl: '',
        auth0ClientId: '',
        auth0Audience: '',
        auth0TenantDomain: '',
        appInsightsTelemetryKey: ''
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

    public loadConfig(http): Promise<EnvironmentConfig> {
        console.log('Application Loading Config');
        return this.getJSON(http)
            .pipe(
                tap(
                    data => {
                        // Get the configuration from the api specified in the envionment.ts file
                        this.config.appInsightsTelemetryKey = data['appInsightsTelemetryKey'];
                        this.config.auth0Audience = data['auth0Audience'];
                        this.config.auth0CallbackUrl = data['auth0CallbackUrl'];
                        this.config.auth0ClientId = data['auth0ClientId'];
                        this.config.auth0TenantDomain = data['auth0TenantDomain'];
                    }
                )
            )
            .toPromise();
    }

    public getJSON(http): Observable<any> {
        // Was: this used to get configuration from the functionapp
        // return http.get('/api/ClientConfigurationHttp');

        // Now: we're using a static file to specify the primary API from which to get configuration
        this.config.apiUrl = environment.apiUrl;
        return http.get(`${this.config.apiUrl}/clientconfiguration/app`);
    }
}

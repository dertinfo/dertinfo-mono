import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

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
                        this.config.apiUrl = data['PwaClient:Api:Uri'];
                        this.config.appInsightsTelemetryKey = data['PwaClient:AppInsights:TelemetryId'];
                        this.config.auth0Audience = data['PwaClient:Auth0:Audience'];
                        this.config.auth0CallbackUrl = data['PwaClient:Auth0:CallbackUrl'];
                        this.config.auth0ClientId = data['PwaClient:Auth0:ClientId'];
                        this.config.auth0TenantDomain = data['PwaClient:Auth0:Domain'];
                    }
                )
            )
            .toPromise();
    }

    public getJSON(http): Observable<any> {
        return http.get('/api/ClientConfigurationHttp');
    }
}

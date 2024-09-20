import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { switchMap, tap } from 'rxjs/operators';
import { jwtOptions } from '../jwt-utils';
import { environment } from 'src/environments/environment';

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
        // All values are replaced at runtime
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

        console.log('configurationservice - loadConfig - start');

        return this.getLocalConfiguration(http)
            .pipe(
                tap(localData => {
                    this.config.apiUrl = localData['apiUrl'];
                    this.config.auth0CallbackUrl = localData['auth0CallbackUrl'];
                    this.config.allowedDomains = localData['allowedDomains'];

                    console.log('configurationservice - loadlocal - completed');

                    var newDomains = this.config.allowedDomains.map(domain => domain.toLowerCase());
                    jwtOptions.updateAllowedDomains(newDomains);

                    console.log('configurationservice - loadlocal - updateddomainsassigned');
                }),
                switchMap(localData =>
                    this.getRemoteConfiguration(http).pipe(
                        tap(remoteData => {
                            this.config.appInsightsTelemetryKey = remoteData['appInsightsTelemetryKey'];
                            this.config.auth0Audience = remoteData['auth0Audience'];
                            // this.config.auth0CallbackUrl = remoteData['auth0CallbackUrl']; // note - due to adopting codespaces this is not known at the API so we define in the client
                            this.config.auth0ClientId = remoteData['auth0ClientId'];
                            this.config.auth0TenantDomain = remoteData['auth0TenantDomain'];

                            console.log('configurationservice - loadremote - completed');
                        })
                    )
                )
            )
            .toPromise();
    }

    public getLocalConfiguration(http): Observable<any> {
        // Was: this used to get configuration from the functionapp
        // return http.get('/api/ClientConfigurationHttp');

        // Now: we're using a static file to specify the primary API from which to get configuration
        // this.config.apiUrl = environment.apiUrl;
        // return http.get(`${this.config.apiUrl}/clientconfiguration/app`);

        // When running in codespaces and locally this is the configuration file to use
        // In Codespaces: This file is changed with information from the created codespace using the codespace name.
        // In local: We use the file as it is to connect to local ports. 
        var configuration = http.get("assets/app.config.json");

        // If we're in production, this service is hosted in Azure Static Web App which builds the service on deployment.
        // The deployed production setup does not use containerisation ans uses the native functionality of the Azure Static Web App.
        // Part of this pipeline is to rebuild the service using the angular CLI and therefore the docker build mechansism are not relevant
        if (environment.production) {
            configuration.apiUrl = environment.apiUrl;
            configuration.auth0CallbackUrl = environment.auth0CallbackUrl;
            configuration.allowedDomains = environment.allowedDomains;
        }

        return configuration;
    }

    public getRemoteConfiguration(http): Observable<any> {

        return http.get(`${this.config.apiUrl}/clientconfiguration/app`);
    }
}

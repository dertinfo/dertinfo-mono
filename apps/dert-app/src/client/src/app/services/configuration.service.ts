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

        console.log('Loading configuration...');

        // Create an observable that will be notified when the local settings are loaded
        const localConfigLoaded$ = new Observable<EnvironmentConfig>(observer => {

            if (environment.production) {

                // In the production scenario all our services are deployed natively to Azure Static Web App.
                // The build pipeline for this is ADO. Which builds the app before deployment with the ralavenat --configuration flag
                // This means that for "test" and "production" the configuration is set at build time and not runtime.
                console.log('Applying production configuration');
                this.config.apiUrl = environment.apiUrl;
                this.config.auth0CallbackUrl = environment.auth0CallbackUrl;
                this.config.allowedDomains = environment.allowedDomains;
                observer.next(this.config);
                observer.complete();

            } else {

                // When running in codespaces and locally this is the configuration file to use
                // In Codespaces: This file is changed with information from the created codespace using the codespace name.
                // In local: We use the file as it is to connect to local ports.
                console.log('Loading Local/Codespaces configuration');
                this.getLocalConfiguration(http).subscribe(localData => {
                    console.log('Applying Local/Codespaces Configuration');
                    this.config.apiUrl = localData.apiUrl;
                    this.config.auth0CallbackUrl = localData.auth0CallbackUrl;
                    this.config.allowedDomains = localData.allowedDomains;
                    observer.next(this.config);
                    observer.complete();
                });
            }
        });

        // Return a promise that will be resolved when the remote settings are loaded
        return new Promise((resolve, reject) => {

            // When the local settings are loaded, apply the allowed domains to the jwtOptions
            localConfigLoaded$.subscribe(config => {
                
                // Set Allowed Domains - All From local configuration. 
                console.log('Assigning Allowed Domains');
                var newDomains = this.config.allowedDomains.map(domain => domain.toLowerCase());
                jwtOptions.updateAllowedDomains(newDomains);

                // Load the remote settings
                console.log('Loading Remote Configuration');
                var subs = this.getRemoteConfiguration(http).subscribe(remoteData => {
                    console.log('Applying Remote Configuration');
                    this.config.appInsightsTelemetryKey = remoteData['appInsightsTelemetryKey'];
                    this.config.auth0Audience = remoteData['auth0Audience'];
                    this.config.auth0ClientId = remoteData['auth0ClientId'];
                    this.config.auth0TenantDomain = remoteData['auth0TenantDomain'];

                    subs.unsubscribe();
                    resolve(this.config);
                });
            });
        });
    }

    public getLocalConfiguration(http): Observable<any> {
        // Was: this used to get configuration from the functionapp
        // return http.get('/api/ClientConfigurationHttp');

        // Now: we're using a static file to specify the primary API from which to get configuration
        // this.config.apiUrl = environment.apiUrl;
        // return http.get(`${this.config.apiUrl}/clientconfiguration/app`);

        var configuration = http.get("assets/app.config.json");
        return configuration;
    }

    public getRemoteConfiguration(http): Observable<any> {

        return http.get(`${this.config.apiUrl}/clientconfiguration/app`);
    }
}

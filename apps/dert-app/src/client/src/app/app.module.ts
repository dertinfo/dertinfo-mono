import { APP_INITIALIZER, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouteReuseStrategy } from '@angular/router';
import { HttpClient, HttpClientModule } from '@angular/common/http';

import { IonicModule, IonicRouteStrategy } from '@ionic/angular';
import { IonicStorageModule } from '@ionic/storage-angular';

import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { AppSharedModule } from './shared/shared.module';
import { ILogger, ConsoleLogger } from './providers/diagnostics/logger'
import { AppStoresModule } from './stores/stores.module';
import { AppServicesModule } from './services/services.module';
import { PipesModule } from './pipes/pipes.module';

import { JwtModule } from '@auth0/angular-jwt';
import { AuthModule } from './authentication/auth.module';
import { AuthService } from './authentication/auth.service';
import { ConfigurationService } from './services/configuration.service';
import { AppInsightsService } from './services/appinsights.service';
import { AuthGuard } from './core/guards/auth.guard';

export function initSettings(configurationService: ConfigurationService, appInsightsService: AppInsightsService, http: HttpClient) {
  console.log('Application Initialising');
  return () => {
    return configurationService.loadConfig(http).then(() => {
      appInsightsService.initialiseInsights();
    });
  };
}

export function jwtOptionsFactory(authService: AuthService) {
  return {
      tokenGetter: () => {
          return authService.accessToken
      },
      allowedDomains: [
        'localhost:60280',
        'dertinfo-api-test.azurewebsites.net',
        'dertinfo-test-api-wa.azurewebsites.net',
        'dertinfo-api-live.azurewebsites.net',
        'dertinfo-live-api-wa.azurewebsites.net'
      ],
  };

  /** You must add valid domains the the whitelist else the token will not be provided */
}

export function getJwtToken(): string {
  return localStorage.getItem('access_token');
}

@NgModule({
  imports: [
    IonicStorageModule.forRoot(),
    BrowserModule,
    IonicModule.forRoot(),
    AppRoutingModule,
    AppSharedModule,
    AppServicesModule,
    AppStoresModule,
    AuthModule,
    HttpClientModule,
    PipesModule,
    JwtModule.forRoot({
      config: {
        tokenGetter: getJwtToken,
        allowedDomains: ['localhost:60280', 'dertinfo-api-test.azurewebsites.net', 'dertinfo-test-api-wa.azurewebsites.net', 'dertinfo-api-live.azurewebsites.net', 'dertinfo-live-api-wa.azurewebsites.net'],
      }
    }),
  ],
  declarations: [
    AppComponent
  ],
  entryComponents: [],
  providers: [
    { provide: APP_INITIALIZER, useFactory: initSettings, deps: [ConfigurationService, AppInsightsService, HttpClient], multi: true },
    { provide: RouteReuseStrategy, useClass: IonicRouteStrategy },
    { provide: ILogger, useClass: ConsoleLogger },
    AuthGuard
  ],
  bootstrap: [AppComponent],
  exports:[
    AppSharedModule,
    AppServicesModule,
    AppStoresModule,
    PipesModule
  ]
})
export class AppModule { }

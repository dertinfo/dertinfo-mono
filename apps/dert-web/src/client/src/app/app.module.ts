
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { APP_INITIALIZER, ErrorHandler, LOCALE_ID, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule } from '@angular/router';
import { JwtModule } from '@auth0/angular-jwt';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { ConfigurationService } from 'app/core/services/configuration.service';
import { RepositoriesModule } from 'app/modules/repositories';
import { CookieService } from 'ngx-cookie-service';
import { QuillModule } from 'ngx-quill';
import { AppComponent } from './app.component';
import { rootRouterConfig } from './app.routes';

import { AuthService } from './core/authentication/auth.service';
import { AuthGuard } from './core/guards/auth.guard';
import { AppInsightsService } from './core/logging/appinsights.service';
import { ErrorHandlerService } from './core/logging/errorhandler.service';
import { WarmupResolver } from './core/resolvers/warmup.resolver';
import { ClientSettingsService } from './core/services/clientsettings.service';
import { NavigationService } from './core/services/navigation.service';
import { RoutePartsService } from './core/services/route-parts.service';
import { WarmupService } from './core/services/warmup.service';
import { DashboardResolver } from './modules/dashboard/dashboard.resolver';
import { NotificationModule } from './modules/notification/notification.module';
import { AuthenticatedRegionModule } from './regions/authenticated/authenticated-region.module';
import { BlanksRegionModule } from './regions/blanks/blanks-region.module';
import { CallbacksRegionModule } from './regions/callbacks/callbacks-region.module';
import { ContentRegionModule } from './regions/content/content-region.module';
import { DertOfDertsRegionModule } from './regions/dertofderts/dertofderts-region.module';
import { SessionRegionModule } from './regions/session/session-region.module';
import { TermsRegionModule } from './regions/terms/terms-region.module';
import { AppSharedModule } from './shared/app-shared.module';

export function initSettings(configurationService: ConfigurationService, appInsightsService: AppInsightsService, http: HttpClient) {
  console.log('Application Initialising');
  return () => {
    return configurationService.loadConfig(http).then(() => {
      appInsightsService.initialiseInsights();
    });
  };
}

// AoT requires an exported function for factories
export function HttpLoaderFactory(httpClient: HttpClient) {
  return new TranslateHttpLoader(httpClient);
}

export function getJwtToken(): string {
  return localStorage.getItem('access_token');
}

@NgModule({
  imports: [
    // Angular
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    // Region imports. note - if we lazy we should be able to remove these.
    AuthenticatedRegionModule,
    CallbacksRegionModule,
    SessionRegionModule,
    BlanksRegionModule,
    ContentRegionModule,
    DertOfDertsRegionModule,
    TermsRegionModule,
    // Shared - if possible we don't want to load this here
    AppSharedModule,

    RepositoriesModule,

    NotificationModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient]
      }
    }),
    JwtModule.forRoot({
      config: {
        tokenGetter: getJwtToken,
        allowedDomains: ['localhost:60280', 'localhost:44100', 'dertinfo-api-test.azurewebsites.net', 'dertinfo-test-api-wa.azurewebsites.net', 'dertinfo-api-live.azurewebsites.net', 'dertinfo-live-api-wa.azurewebsites.net'],
      }
    }),
    RouterModule.forRoot(rootRouterConfig, { enableTracing: false, useHash: false, anchorScrolling: 'enabled', relativeLinkResolution: 'legacy' }),
    QuillModule.forRoot()
  ],
  declarations: [AppComponent],
  providers: [
    NavigationService,
    DashboardResolver,
    CookieService,
    { provide: APP_INITIALIZER, useFactory: initSettings, deps: [ConfigurationService, AppInsightsService, HttpClient], multi: true },
    { provide: LOCALE_ID, useValue: 'en-GB' },
    { provide: ErrorHandler, useClass: ErrorHandlerService }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

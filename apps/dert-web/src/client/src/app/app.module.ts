import { HttpClient, HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { APP_INITIALIZER, ErrorHandler, LOCALE_ID, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule } from '@angular/router';
import {
  AuthClientConfig,
  AuthHttpInterceptor,
  AuthModule,
} from '@auth0/auth0-angular';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { ConfigurationService } from 'app/core/services/configuration.service';
import { RepositoriesModule } from 'app/modules/repositories';
import { CookieService } from 'ngx-cookie-service';
import { QuillModule } from 'ngx-quill';
import { AppComponent } from './app.component';
import { rootRouterConfig } from './app.routes';

import { AppInsightsService } from './core/logging/appinsights.service';
import { ErrorHandlerService } from './core/logging/errorhandler.service';
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

/**
 * APP_INITIALIZER — Auth flow step 1–2:
 *  1. Load local/staging/prod callback URL + API base (assets or environment.*).
 *  2. Fetch Auth0 domain/clientId/audience from GET {api}/clientconfiguration/web.
 *  3. AuthClientConfig.set — configure @auth0/auth0-angular with refresh tokens for ALL envs.
 *
 * Same path for local (localhost:44200), staging (staging.dertinfo.co.uk), and prod (www.dertinfo.co.uk).
 */
export function initSettings(
  configurationService: ConfigurationService,
  appInsightsService: AppInsightsService,
  authClientConfig: AuthClientConfig,
) {
  console.log('Application Initialising');
  return () => {
    // loadConfig uses HttpBackend internally so AuthHttpInterceptor cannot deadlock bootstrap.
    return configurationService.loadConfig().then((config) => {
      // Step 2 — apply Auth0 SPA SDK settings after remote config is available.
      // useRefreshTokens + localstorage: renew via /oauth/token (not iframe /silent).
      authClientConfig.set({
        domain: config.auth0TenantDomain,
        clientId: config.auth0ClientId,
        authorizationParams: {
          audience: config.auth0Audience,
          redirect_uri: `${config.auth0CallbackUrl}/callback`,
          scope: 'openid profile email offline_access',
        },
        useRefreshTokens: true,
        // Prefer refresh tokens on localhost; do not fall back to consent_required iframe silent auth.
        useRefreshTokensFallback: false,
        cacheLocation: 'localstorage',
        httpInterceptor: {
          // Step 5 — AuthHttpInterceptor attaches Bearer AT to matching API URLs.
          allowedList: [
            `${config.apiUrl}/*`,
          ],
        },
      });

      appInsightsService.initialiseInsights();
    });
  };
}

// AoT requires an exported function for factories
export function HttpLoaderFactory(httpClient: HttpClient) {
  return new TranslateHttpLoader(httpClient);
}

@NgModule({
  imports: [
    // Angular
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    // Auth0 Angular SDK — placeholders overwritten in APP_INITIALIZER via AuthClientConfig.set.
    // Real domain/clientId/audience/redirect come from ConfigurationService (local + staging + prod).
    // Pinned to 2.2.3: newer 2.x pulls makeEnvironmentProviders (Angular 15+) which breaks Angular 14.
    AuthModule.forRoot({
      domain: 'placeholder.auth0.com',
      clientId: 'placeholder',
    }),
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
    RouterModule.forRoot(rootRouterConfig, { enableTracing: false, useHash: false, anchorScrolling: 'enabled', relativeLinkResolution: 'legacy' }),
    QuillModule.forRoot()
  ],
  declarations: [AppComponent],
  providers: [
    NavigationService,
    DashboardResolver,
    CookieService,
    {
      provide: APP_INITIALIZER,
      useFactory: initSettings,
      deps: [ConfigurationService, AppInsightsService, AuthClientConfig],
      multi: true,
    },
    // Attaches Authorization: Bearer <access_token> to URLs in httpInterceptor.allowedList.
    { provide: HTTP_INTERCEPTORS, useClass: AuthHttpInterceptor, multi: true },
    { provide: LOCALE_ID, useValue: 'en-GB' },
    { provide: ErrorHandler, useClass: ErrorHandlerService }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

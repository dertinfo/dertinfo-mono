import { Routes } from '@angular/router';
import { ClientSettingsResolver } from './core/resolvers/clientsettings.resolver';
import { WarmupResolver } from './core/resolvers/warmup.resolver';
import { AuthenticatedRegionRoutes } from './regions/authenticated/authenticated-region.routing';
import { BlanksRegionRoutes } from './regions/blanks/blanks-region.routing';
import { CallbacksRegionRoutes } from './regions/callbacks/callbacks-region.routing';
import { ContentRegionRoutes } from './regions/content/content-region.routing';
import { DertOfDertsRegionRoutes } from './regions/dertofderts/dertofderts-region.routing';
import { HomeRegionRoutes } from './regions/home/home-region.routing';
import { SessionRegionRoutes } from './regions/session/session-region.routing';
import { TermsRegionRoutes } from './regions/terms/terms-region.routing';

export const rootRouterConfig: Routes = [
  {
    path: '',
    redirectTo: 'home',
    pathMatch: 'full'
  },
  ...CallbacksRegionRoutes,
  ...HomeRegionRoutes,
  ...ContentRegionRoutes,
  ...DertOfDertsRegionRoutes,
  ...TermsRegionRoutes,
  ...SessionRegionRoutes,
  ...BlanksRegionRoutes,
  ...AuthenticatedRegionRoutes,
  {
    path: '**',
    redirectTo: 'session/404'
  }
];

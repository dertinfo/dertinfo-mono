import { Routes } from '@angular/router';

import { StartComponent } from './components/start/start.component';
import { EventSetupComponent } from './event-setup.component';
import { EventSetupResolver } from './services/event-setup.resolver';

const routes: Routes = [
  {
    path: '',
    redirectTo: '/dashboard',
    pathMatch: 'full'
  },
  {
    path: ':name/:id',
    component: EventSetupComponent,
    data: { title: '{{name}}', breadcrumb: '{{name}}' },
    resolve: { eventConfigureOverview: EventSetupResolver },
    children: [
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'start'
      },
      {
        path: 'start',
        component: StartComponent,
        data: { title: 'Start', breadcrumb: 'START' }
      }]
  }
];

export const EventSetupRoutes: Routes = routes;

import { Routes } from '@angular/router';
import { ErrorComponent } from './components/error/error.component';
import { ForbiddenComponent } from './components/forbidden/forbidden.component';
import { NotAuthorisedComponent } from './components/not-authorised/not-authorised.component';
import { NotFoundComponent } from './components/not-found/not-found.component';
import { WarmupComponent } from './components/warmup/warmup.component';

export const SessionRoutes: Routes = [
  {
    path: '',
    children: [{
      path: '404',
      component: NotFoundComponent,
      data: { title: 'Not Found' }
    }, {
      path: 'error',
      component: ErrorComponent,
      data: { title: 'Error' }
    }, {
      path: '401',
      component: NotAuthorisedComponent,
      data: { title: 'Not Authorised' }
    }, {
      path: '403',
      component: ForbiddenComponent,
      data: { title: 'Forbidden' }
    }, {
      path: 'warmup',
      component: WarmupComponent,
      data: { title: 'Warming up' }
    }]
  }
];

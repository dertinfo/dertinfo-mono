import { RouterModule, Routes } from '@angular/router';
import { SessionRegionComponent } from './session-region.component';

const routes: Routes = [
    {
        path: '',
        component: SessionRegionComponent,
        children: [
          {
            path: 'session',
            loadChildren: () => import('../../modules/session/session.module').then(m => m.SessionModule),
            data: { title: 'Session' }
          },
          {
            path: 'auth',
            loadChildren: () => import('../../modules/authentication/authentication.module').then(m => m.AuthenticationModule),
            data: { title: 'Session' }
          }
        ]
      }
];

export const SessionRegionRoutes = routes;

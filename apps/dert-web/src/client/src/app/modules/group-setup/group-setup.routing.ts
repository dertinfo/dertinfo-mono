import { Routes } from '@angular/router';
import { StartComponent } from './components/start/start.component';
import { GroupSetupComponent } from './group-setup.component';
import { GroupSetupResolver } from './services/group-setup.resolver';

const routes: Routes = [
  {
    path: '',
    redirectTo: '/dashboard',
    pathMatch: 'full'
  },
  {
    path: ':name/:id',
    component: GroupSetupComponent,
    data: { title: '{{name}}', breadcrumb: '{{name}}' },
    resolve: { groupConfigureOverview: GroupSetupResolver },
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

export const GroupSetupRoutes: Routes = routes;

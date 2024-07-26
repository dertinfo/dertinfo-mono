import { Routes } from '@angular/router';
import { StartComponent } from './components/start/start.component';
import { GroupRegisterComponent } from './group-register.component';
import { GroupRegisterResolver } from './services/group-register.resolver';

export const GroupRegisterRoutes: Routes = [
  {
    path: '',
    redirectTo: '/dashboard',
    pathMatch: 'full'
  },
  {
    path: ':groupId/:eventId',
    component: GroupRegisterComponent,
    data: { title: '{{name}}', breadcrumb: '{{name}}' },
    resolve: { groupRegisterOverview: GroupRegisterResolver },
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

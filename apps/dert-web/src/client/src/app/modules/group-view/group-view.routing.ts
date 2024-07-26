import { Routes } from '@angular/router';
import { DertOfDertsComponent } from './components/dertofderts/dertofderts.component';
import { GroupViewOverviewComponent } from './components/overview/overview.component';
import { GroupViewRegistrationComponent } from './components/registration/registration.component';
import { RegistrationResolver } from './components/registration/registration.resolver';
import { GroupViewComponent } from './group-view.component';
import { GroupViewDodResolver } from './services/group-view-dod.resolver';
import { GroupViewRegistrationsResolver } from './services/group-view-registrations.resolver';
import { GroupViewResolver } from './services/group-view.resolver';

 const routes: Routes = [
  {
    path: '',
    redirectTo: '/dashboard',
    pathMatch: 'full'
  },
  {
    path: ':name/:id',
    component: GroupViewComponent,
    data: { title: '{{name}}', breadcrumb: '{{name}}' },
    resolve: {
      group: GroupViewResolver,
      registrations: GroupViewRegistrationsResolver,
      showDertOfDerts: GroupViewDodResolver
    },
    children: [
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'overview'
      },
      {
        path: 'overview',
        component: GroupViewOverviewComponent,
        data: { title: 'Overview', breadcrumb: 'OVERVIEW'}
      },
      {
        path: 'registration/:registrationId',
        component: GroupViewRegistrationComponent,
        data: { title: 'Event', breadcrumb: 'EVENT'},
        resolve: { teamAttendance: RegistrationResolver },
      },
      {
        path: 'dertofderts',
        component: DertOfDertsComponent,
        data: { title: 'DertOfDerts', breadcrumb: 'DERTOFDERTS'}
      },
    ]
  }
];

export const GroupViewRoutes: Routes = routes;

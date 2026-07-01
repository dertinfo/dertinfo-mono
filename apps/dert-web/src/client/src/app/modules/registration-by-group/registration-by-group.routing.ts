import { Routes } from '@angular/router';

import { RegistrationByGroupComponent } from './registration-by-group.component';
import { GroupRegistrationActivitiesComponent } from './components/group-registration-activities/group-registration-activities.component';
import { GroupRegistrationGuestsComponent } from './components/group-registration-guests/group-registration-guests.component';
import { GroupRegistrationMembersComponent } from './components/group-registration-members/group-registration-members.component';
import { GroupRegistrationOverviewComponent } from './components/group-registration-overview/group-registration-overview.component';
import { GroupRegistrationTeamsComponent } from './components/group-registration-teams/group-registration-teams.component';
import { GroupRegistrationTicketsComponent } from './components/group-registration-tickets/group-registration-tickets.component';
import { RegistrationByGroupResolver } from './services/registration-by-group.resolver';

export const RegistrationByGroupRoutes: Routes = [
  {
    path: '',
    redirectTo: '/dashboard',
    pathMatch: 'full'
  },
  {
    path: ':id',
    component: RegistrationByGroupComponent,
    data: { title: 'Registration', breadcrumb: 'Registration' },
    resolve: { groupRegistrationOverview: RegistrationByGroupResolver },
    children: [
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'overview'
      },
      {
        path: 'overview',
        component: GroupRegistrationOverviewComponent,
        data: { title: 'Overview', breadcrumb: 'OVERVIEW' }
      },
      {
        path: 'members',
        component: GroupRegistrationMembersComponent,
        data: { title: 'Attending Members', breadcrumb: 'ATTENDINGMEMBERS' }
      },
      {
        path: 'guests',
        component: GroupRegistrationGuestsComponent,
        data: { title: 'Attending Guests', breadcrumb: 'ATTENDINGGUESTS' }
      },
      {
        path: 'teams',
        component: GroupRegistrationTeamsComponent,
        data: { title: 'Attending Teams', breadcrumb: 'ATTENDINGTEAMS' }
      },
      {
        path: 'activities',
        component: GroupRegistrationActivitiesComponent,
        data: { title: 'Activities', breadcrumb: 'ACTIVITIES' }
      },
      {
        path: 'tickets',
        component: GroupRegistrationTicketsComponent,
        data: { title: 'Tickets', breadcrumb: 'TICKETS' }
      }]
  }
];

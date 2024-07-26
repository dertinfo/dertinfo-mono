import { Routes } from '@angular/router';
import { EventRegistrationActivitiesComponent } from './components/event-registration-activities/event-registration-activities.component';
import { EventRegistrationGuestsComponent } from './components/event-registration-guests/event-registration-guests.component';
import { EventRegistrationMembersComponent } from './components/event-registration-members/event-registration-members.component';
import { EventRegistrationOverviewComponent } from './components/event-registration-overview/event-registration-overview.component';
import { EventRegistrationTeamsComponent } from './components/event-registration-teams/event-registration-teams.component';
import { EventRegistrationTicketsComponent } from './components/event-registration-tickets/event-registration-tickets.component';
import { RegistrationByEventsComponent } from './registration-by-events.component';
import { RegistrationByEventsResolver } from './services/registration-by-events.resolver';

const routes: Routes = [
  {
    path: '',
    redirectTo: '/dashboard',
    pathMatch: 'full'
  },
  {
    path: ':id',
    component: RegistrationByEventsComponent,
    data: { title: 'Registration', breadcrumb: 'Registration' },
    resolve: { eventRegistrationOverview: RegistrationByEventsResolver },
    children: [
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'overview'
      },
      {
        path: 'overview',
        component: EventRegistrationOverviewComponent,
        data: { title: 'Overview', breadcrumb: 'OVERVIEW' }
      },
      {
        path: 'members',
        component: EventRegistrationMembersComponent,
        data: { title: 'Attending Members', breadcrumb: 'ATTENDINGMEMBERS' }
      },
      {
        path: 'guests',
        component: EventRegistrationGuestsComponent,
        data: { title: 'Attending Guests', breadcrumb: 'ATTENDINGGUESTS' }
      },
      {
        path: 'teams',
        component: EventRegistrationTeamsComponent,
        data: { title: 'Attending Teams', breadcrumb: 'ATTENDINGTEAMS' }
      },
      {
        path: 'activities',
        component: EventRegistrationActivitiesComponent,
        data: { title: 'Activities', breadcrumb: 'ACTIVITIES' }
      },
      {
        path: 'tickets',
        component: EventRegistrationTicketsComponent,
        data: { title: 'Tickets', breadcrumb: 'TICKETS' }
      }]
  }
];

export const RegistrationByEventsRoutes: Routes = routes;

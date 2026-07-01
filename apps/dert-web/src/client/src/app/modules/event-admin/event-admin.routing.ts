import { Routes } from '@angular/router';

import { EventComponent } from './components/app-event.component';
import { EventCompetitionsComponent } from './components/competitions/competitions.component';
import { EventActivitiesDetailComponent } from './components/event-activities-detail/event-activities-detail.component';
import { EventActivitiesComponent } from './components/event-activities/event-activities.component';
import { EventBlankComponent } from './components/event-blank/event-blank.component';
import { EventDownloadsComponent } from './components/event-downloads/event-downloads.component';
import { EventEmailTemplatesComponent } from './components/event-email-templates/event-email-templates.component';
import { EmailTemplatesResolver } from './components/event-email-templates/event-email-templates.resolver';
import { EventGalleryComponent } from './components/event-gallery/event-gallery.component';
import { EventInvoicesComponent } from './components/event-invoices/event-invoices.component';
import { EventOverviewComponent } from './components/event-overview/event-overview.component';
import { EventRegistrationsComponent } from './components/event-registrations/event-registrations.component';
import { EventSettingsComponent } from './components/event-settings/event-settings.component';
import { PaperworkComponent } from './components/paperwork/paperwork.component';
import { EventAdminResolver } from './services/event-admin.resolver';

 const routes: Routes = [
  {
    path: '',
    redirectTo: '/dashboard',
    pathMatch: 'full'
  },
  {
    path: ':name/:id',
    component: EventComponent,
    data: { title: '{{name}}', breadcrumb: '{{name}}', breadcrumbIcon: 'event'},
    resolve: { eventOverview: EventAdminResolver },
    children: [
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'overview'
      },
      {
        path: 'overview',
        component: EventOverviewComponent,
        data: { title: 'Overview', breadcrumb: 'OVERVIEW'}
      },
      {
        path: 'settings',
        component: EventSettingsComponent,
        data: { title: 'Settings', breadcrumb: 'SETTINGS' }
      },
      {
        path: 'registrations',
        component: EventRegistrationsComponent,
        data: { title: 'Registrations', breadcrumb: 'REGISTRATIONS' }
      },
      {
        path: 'activities',
        component: EventActivitiesComponent,
        data: { title: 'Activities', breadcrumb: 'ACTIVITIES' },
        children: []
      },
      {
        path: 'activities/:title/:activityId',
        component: EventActivitiesDetailComponent,
        data: { title: '{{title}}', breadcrumb: '{{title}}', breadcrumbIcon: 'check_circle'}
      },
      {
        path: 'gallery',
        component: EventGalleryComponent,
        data: { title: 'Gallery', breadcrumb: 'GALLERY' }
      },
      {
        path: 'emailtemplates',
        component: EventEmailTemplatesComponent,
        data: { title: 'EmailTemplates', breadcrumb: 'EMAILTEMPLATES' },
        resolve: { emailTemplates: EmailTemplatesResolver }
      },
      {
        path: 'invoices',
        component: EventInvoicesComponent,
        data: { title: 'Invoices', breadcrumb: 'INVOICES' }
      },
      {
        path: 'competitions',
        component: EventCompetitionsComponent,
        data: { title: 'Competitions', breadcrumb: 'COMPETITIONS' }
      },
      {
        path: 'downloads',
        component: EventDownloadsComponent,
        data: { title: 'Downloads', breadcrumb: 'DOWNLOADS' }
      },
      {
        path: 'paperwork',
        component: PaperworkComponent,
        data: { title: 'Paperwork', breadcrumb: 'PAPERWORK' }
      },
      {
        path: 'blank',
        component: EventBlankComponent,
        data: { title: 'Blank', breadcrumb: 'BLANK' }
      }
    ]
  }
];

export const EventAdminRoutes = routes;

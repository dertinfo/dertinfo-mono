import { Routes } from '@angular/router';
import { GroupTeamsDetailComponent } from 'app/modules/group-admin/components/group-teams-detail/group-teams-detail.component';
import { GroupGalleryComponent } from './components/group-gallery/group-gallery.component';
import { GroupInvoicesComponent } from './components/group-invoices/group-invoices.component';
import { GroupMembersDetailComponent } from './components/group-members-detail/group-members-detail.component';
import { GroupMembersComponent } from './components/group-members/group-members.component';
import { GroupOverviewComponent } from './components/group-overview/group-overview.component';
import { GroupRegistrationsComponent } from './components/group-registrations/group-registrations.component';
import { GroupSettingsComponent } from './components/group-settings/group-settings.component';
import { GroupTeamsComponent } from './components/group-teams/group-teams.component';
import { GroupAdminComponent } from './group-admin.component';
import { GroupAdminDodResolver } from './services/group-admin-dod.resolver';
import { GroupAdminResolver } from './services/group-admin.resolver';

const routes: Routes = [
  {
    path: '',
    redirectTo: '/dashboard',
    pathMatch: 'full'
  },
  {
    path: ':name/:id',
    component: GroupAdminComponent,
    data: { title: '{{name}}', breadcrumb: '{{name}}' },
    resolve: {
      groupOverview: GroupAdminResolver,
      showDertOfDerts: GroupAdminDodResolver
     },
    children: [
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'overview'
      },
      {
        path: 'overview',
        component: GroupOverviewComponent,
        data: { title: 'Overview', breadcrumb: 'OVERVIEW' }
      },
      {
        path: 'settings',
        component: GroupSettingsComponent,
        data: { title: 'Settings', breadcrumb: 'SETTINGS' }
      },
      {
        path: 'teams',
        component: GroupTeamsComponent,
        pathMatch: 'full',
        data: { title: 'Teams', breadcrumb: 'TEAMS' }
      },
      {
        path: 'teams/:name/:teamId',
        component: GroupTeamsDetailComponent,
        data: { title: '{{name}}', breadcrumb: '{{name}}' }
      },
      {
        path: 'members',
        component: GroupMembersComponent,
        pathMatch: 'full',
        data: { title: 'Members', breadcrumb: 'MEMBERS' }
      },
      {
        path: 'members/:name/:groupMemberId',
        component: GroupMembersDetailComponent,
        data: { title: '{{name}}', breadcrumb: '{{name}}' }
      },
      {
        path: 'registrations',
        component: GroupRegistrationsComponent,
        data: { title: 'Registrations', breadcrumb: 'REGISTRATIONS' }
      },
      {
        path: 'gallery',
        component: GroupGalleryComponent,
        data: { title: 'Gallery', breadcrumb: 'GALLERY' }
      },
      {
        path: 'invoices',
        component: GroupInvoicesComponent,
        data: { title: 'Invoices', breadcrumb: 'INVOICES' }
      }]
  }
];

export const GroupAdminRoutes = routes;

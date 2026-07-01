import { Routes } from '@angular/router';
import { UserAccountOverviewComponent } from './components/user-account-overview/user-account-overview.component';
import { UserAccountSettingsComponent } from './components/user-account-settings/user-account-settings.component';
import { UserAccountComponent } from './user-account.component';

export const UserAccountRoutes: Routes = [
  {
    path: '',
    component: UserAccountComponent,
    data: { title: 'UserAccount', breadcrumb: 'UserAccount' },
    children: [
      {
        path: '',
        redirectTo: 'overview',
        pathMatch: 'full'
      },
      {
        path: 'overview',
        component: UserAccountOverviewComponent,
        data: { title: 'Overview', breadcrumb: 'OVERVIEW' }
      },
      {
        path: 'settings',
        component: UserAccountSettingsComponent,
        data: { title: 'Settings', breadcrumb: 'SETTINGS' }
      },
    ]
  }
];

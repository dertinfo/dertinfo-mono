import { RouterModule, Routes } from '@angular/router';

import { OverviewComponent } from './components/overview/overview.component';
import { SystemAdminComponent } from './system-admin.component';

const routes: Routes = [
    {
        path: '',
        component: SystemAdminComponent,
        children: [
            {
                path: '',
                pathMatch: 'full',
                redirectTo: 'overview'
            },
            {
                path: 'overview',
                component: OverviewComponent,
                data: { title: 'Overview', breadcrumb: 'OVERVIEW' }
            },
            {
                path: 'notifications',
                loadChildren: () => import('../notification-management/notification-management.module')
                    .then(m => m.NotificationManagementModule),
                data: { title: 'Notifications' , breadcrumb: 'NOTIFICATIONS'}
            }
        ]
    }
];

export const SystemAdminRoutes = RouterModule.forChild(routes);

import { RouterModule, Routes } from '@angular/router';

import { AdminDetailComponent } from 'app/modules/notification-management/components/detail/admin-detail.component';
import { AdminListComponent } from 'app/modules/notification-management/components/list/admin-list.component';

const routes: Routes = [
    {
        path: '',
        component: AdminListComponent,
        data: { title: 'Notifications', breadcrumb: 'NOTIFICATIONS' },
    },
    {
        path: ':notificationId',
        component: AdminDetailComponent,
        data: { title: 'Detail', breadcrumb: 'NOTIFICATIONS' },
    }
];

export const NotificationManagementRoutes = RouterModule.forChild(routes);

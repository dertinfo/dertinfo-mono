import { Routes } from '@angular/router';
import { AuthGuard } from 'app/core/guards/auth.guard';
import { ClientSettingsResolver } from 'app/core/resolvers/clientsettings.resolver';
import { WarmupResolver } from 'app/core/resolvers/warmup.resolver';
import { DashboardResolver } from 'app/modules/dashboard/dashboard.resolver';
import { NotificationCheckResolver } from 'app/modules/notification/services/notification-check.resolver';
import { AuthenticatedRegionComponent } from './authenticated-region.component';

const routes: Routes = [
    {
        path: '',
        component: AuthenticatedRegionComponent,
        canActivate: [AuthGuard],
        children: [
            {
                path: 'dashboard',
                loadChildren: () => import('../../modules/dashboard/dashboard.module').then(m => m.DashboardModule),
                data: { title: 'Dashboard', breadcrumb: 'DASHBOARD' },
                resolve: {
                    warmup: WarmupResolver,
                    notificationThumbnail: NotificationCheckResolver,
                    dashboarddata: DashboardResolver,
                },
            },
            {
                path: 'dodadmin',
                loadChildren: () => import('../../modules/dertofderts-admin/dertofderts-admin.module').then(m => m.DertOfDertsAdminModule),
                data: { title: 'Dert Of Derts Admin', breadcrumb: 'DERTOFDERTSADMIN' }
            },
            {
                path: 'competition',
                loadChildren: () => import('../../modules/competition-admin/competition-admin.module').then(m => m.CompetitionAdminModule),
                data: { title: 'Competition', breadcrumb: 'COMPETITION' }
            },
            {
                path: 'event',
                loadChildren: () => import('../../modules/event-admin/event-admin.module').then(m => m.EventAdminModule),
                data: { title: 'Event', breadcrumb: 'EVENT' }
            },
            {
                path: 'event-configure',
                loadChildren: () => import('../../modules/event-setup/event-setup.module').then(m => m.EventSetupModule),
                data: { title: 'Configure Event', breadcrumb: 'EVENT-CONFIGURE' }
            },
            {
                path: 'event-registration',
                loadChildren: () => import('../../modules/registration-by-events/registration-by-events.module').then(m => m.RegistrationByEventsModule),
                data: { title: 'Registration', breadcrumb: 'EVENT-REGISTRATION' }
            },
            {
                path: 'group',
                loadChildren: () => import('../../modules/group-admin/group-admin.module').then(m => m.GroupAdminModule),
                data: { title: 'Group', breadcrumb: 'GROUP' }
            },
            {
                path: 'group-configure',
                loadChildren: () => import('../../modules/group-setup/group-setup.module').then(m => m.GroupSetupModule),
                data: { title: 'Configure Group', breadcrumb: 'GROUP-CONFIGURE' }
            },
            {
                path: 'group-register',
                loadChildren: () => import('../../modules/group-register/group-register.module').then(m => m.GroupRegisterModule),
                data: { title: 'Register', breadcrumb: 'GROUPREGISTER' }
            },
            {
                path: 'group-view',
                resolve: {
                    clientSettings: ClientSettingsResolver
                },
                loadChildren: () => import('../../modules/group-view/group-view.module').then(m => m.GroupViewModule),
                data: { title: 'Group', breadcrumb: 'GROUP' }
            },
            {
                path: 'group-registration',
                loadChildren: () => import('../../modules/registration-by-group/registration-by-group.module').then(m => m.RegistrationByGroupModule),
                data: { title: 'Registration', breadcrumb: 'GROUPREGISTRATION' }
            },
            {
                path: 'user-account',
                loadChildren: () => import('../../modules/user-account/user-account.module').then(m => m.AppUserAccountModule),
                data: { title: 'User Account', breadcrumb: 'USERACCOUNT' }
            },
            {
                path: 'systemadmin',
                loadChildren: () => import('../../modules/system-admin/system-admin.module').then(m => m.SystemAdminModule),
                data: { title: 'System Admin', breadcrumb: 'SYSTEMADMIN' }
            },
        ]
    },
];

export const AuthenticatedRegionRoutes = routes;

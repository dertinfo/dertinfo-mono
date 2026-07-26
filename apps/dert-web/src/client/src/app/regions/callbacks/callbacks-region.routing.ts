import { RouterModule, Routes } from '@angular/router';
import { AuthCallbackComponent } from './components/auth-callback/auth-callback.component';

/**
 * OAuth redirect targets.
 * /callback — Authorization Code return (active).
 * /silent was used by auth0-js renewAuth iframes; refresh tokens replace that path.
 * Keep a redirect away from /silent so old bookmarks/Auth0 entries do not 404 mid-migration.
 */
const routes: Routes = [
    {
        path: 'callback',
        component: AuthCallbackComponent
    },
    {
        path: 'silent',
        redirectTo: 'callback',
        pathMatch: 'full'
    },
];

export const CallbacksRegionRoutes = routes;

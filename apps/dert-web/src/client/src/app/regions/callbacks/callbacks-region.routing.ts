import { RouterModule, Routes } from '@angular/router';
import { AuthCallbackComponent } from './components/auth-callback/auth-callback.component';
import { AuthSilentComponent } from './components/auth-silent/auth-silent.component';

const routes: Routes = [
    {
        path: 'callback',
        component: AuthCallbackComponent
      },
      {
        path: 'silent',
        component: AuthSilentComponent
      },
];

export const CallbacksRegionRoutes = routes;

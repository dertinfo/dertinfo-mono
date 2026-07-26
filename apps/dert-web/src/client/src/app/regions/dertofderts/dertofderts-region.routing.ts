import { Routes } from '@angular/router';
import { WarmupGuard } from 'app/core/guards/warmup.guard';
import { ClientSettingsResolver } from 'app/core/resolvers/clientsettings.resolver';
import { DertOfDertsRegionComponent } from './dertofderts-region.component';

const routes: Routes = [
  {
    path: '',
    component: DertOfDertsRegionComponent,
    children: [
      {
        path: 'dertofderts',
        canActivate: [WarmupGuard],
        resolve: {
          clientSettings: ClientSettingsResolver
        },
        loadChildren: () => import('../../modules/dertofderts-public/dertofderts-public.module').then(m => m.DertOfDertsPublicModule),
        data: { title: 'Dert Of Derts' }
      }
    ]
  }
];

export const DertOfDertsRegionRoutes = routes;

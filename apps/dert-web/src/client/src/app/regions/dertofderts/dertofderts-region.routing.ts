import { Routes } from '@angular/router';
import { ClientSettingsResolver } from 'app/core/resolvers/clientsettings.resolver';
import { WarmupResolver } from 'app/core/resolvers/warmup.resolver';
import { DertOfDertsRegionComponent } from './dertofderts-region.component';

const routes: Routes = [
  {
    path: '',
    component: DertOfDertsRegionComponent,
    children: [
      {
        path: 'dertofderts',
        resolve: {
          warmup: WarmupResolver,
          clientSettings: ClientSettingsResolver
        },
        loadChildren: () => import('../../modules/dertofderts-public/dertofderts-public.module').then(m => m.DertOfDertsPublicModule),
        data: { title: 'Dert Of Derts' }
      }
    ]
  }
];

export const DertOfDertsRegionRoutes = routes;

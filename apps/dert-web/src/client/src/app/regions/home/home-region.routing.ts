import { Routes } from '@angular/router';

export const HomeRegionRoutes: Routes = [
  {
    path: 'home',
    loadChildren: () => import('../../modules/home/home.module').then(m => m.HomeModule),
    data: { title: 'DertInfo' }
  },
];

import { Routes } from '@angular/router';
import { AuthGuard } from 'app/core/guards/auth.guard';
import { BlanksRegionComponent } from './blanks-region.component';

const routes: Routes = [
  {
    path: '',
    component: BlanksRegionComponent,
    canActivate: [AuthGuard],
    children: [
      {
        path: 'paperwork',
        loadChildren: () => import('../../modules/paperwork-generator/paperwork-generator.module').then(m => m.PaperworkGeneratorModule),
        data: { title: 'Paperwork' }
      }
    ]
  }
];

export const BlanksRegionRoutes = routes;

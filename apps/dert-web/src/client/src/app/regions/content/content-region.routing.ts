import { Routes } from '@angular/router';
import { AuthGuard } from 'app/core/guards/auth.guard';
import { ContentRegionComponent } from './content-region.component';

const routes: Routes = [
  {
    path: '',
    component: ContentRegionComponent,
    children: [
      {
        path: 'content',
        loadChildren: () => import('../../modules/public-content/public-content.module').then(m => m.PublicContentModule),
        data: { title: 'Paperwork' }
      }
    ]
  }
];

export const ContentRegionRoutes = routes;

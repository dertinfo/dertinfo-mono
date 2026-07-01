import { Routes } from '@angular/router';

import { PublicCommunityComponent } from './public-community/public-community.component';
import { PublicEventComponent } from './public-event/public-event.component';
import { PublicHistoryComponent } from './public-history/public-history.component';
import { PublicNotationsComponent } from './public-notations/public-notations.component';
import { PublicResultsComponent } from './public-results/public-results.component';

export const PublicContentRoutes: Routes = [
  { path: 'results', component: PublicResultsComponent },
  {
    path: 'history',
    children: [
      {
        path: '',
        pathMatch: 'full',
        component: PublicHistoryComponent,
      },
      {
        path: 'event/:name/:id',
        component: PublicEventComponent
      }
    ]
  },
  { path: 'community', component: PublicCommunityComponent },
  { path: 'notations', component: PublicNotationsComponent },
];

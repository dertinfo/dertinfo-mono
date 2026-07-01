import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { DanceListPage } from './dance-list.page';
import { DanceDetailPage } from './dance-detail.page';

const routes: Routes = [
  {
    path: '',
    component: DanceListPage
  },
  {
    path: ':id',
    component: DanceDetailPage
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class DancesRoutingModule {}

import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { PublicCommunityComponent } from './public-community/public-community.component';
import { PublicContentConductor } from './services/public-content.conductor';
import { PublicContentRoutes } from './public-content.routing';
import { PublicContentTracker } from './services/public-content.tracker';
import { PublicEventComponent } from './public-event/public-event.component';
import { PublicHistoryComponent } from './public-history/public-history.component';
import { PublicNotationsComponent } from './public-notations/public-notations.component';
import { PublicResultsComponent } from './public-results/public-results.component';
import { MaterialLayoutsModule } from 'app/material-bundles/material-layouts.module';
import { AppSharedModule } from 'app/shared/app-shared.module';
import { PublicContentRepository } from './services/public-content.repository';
@NgModule({
  imports: [
    AppSharedModule,
    MaterialLayoutsModule,
    CommonModule,
    RouterModule.forChild(PublicContentRoutes)
  ],
  declarations: [
    PublicCommunityComponent,
    PublicHistoryComponent,
    PublicResultsComponent,
    PublicEventComponent,
    PublicNotationsComponent
  ],
  providers: [
    PublicContentConductor,
    PublicContentTracker,
    PublicContentRepository
  ]
})
export class PublicContentModule { }

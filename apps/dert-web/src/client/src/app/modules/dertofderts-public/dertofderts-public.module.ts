import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { MaterialLayoutsModule } from 'app/material-bundles/material-layouts.module';
import { WebsiteTermsComponent } from 'app/regions/terms/components/website-terms/website-terms.component';
import { TermsRegionModule } from 'app/regions/terms/terms-region.module';
import { AppConfirmModule } from 'app/services/app-confirm/app-confirm.module';
import { AppSharedModule } from 'app/shared/app-shared.module';
import { ClosedToPublicDialogComponent } from './components/closedtopublicdialog/closedtopublicdialog.component';
import { HeaderComponent } from './components/header/header.component';
import { HomeComponent } from './components/home/home.component';
import { HowToEnterComponent } from './components/how-to-enter/how-to-enter.component';
import { HowToJudgeComponent } from './components/how-to-judge/how-to-judge.component';
import { JudgingComponent } from './components/judging/judging.component';
import { ListComponent } from './components/list/list.component';
import { MarkingCriteriaSideBarComponent } from './components/markingcriteriasidebar/markingcriteriasidebar.component';
import { RecoverSessionDialogComponent } from './components/recoversessiondialog/recoversessiondialog.component';
import { ResultsComponent } from './components/results/results.component';
import { ScoreComponent } from './components/score/score.component';
import { ScoreDialogComponent } from './components/scoredialog/scoredialog.component';
import { ScoreSubmittedDialogComponent } from './components/scoresubmitteddialog/scoresubmitteddialog.component';
import { TalksComponent } from './components/talks/talks.component';
import { TopHeaderComponent } from './components/top-header/top-header.component';
import { DertOfDertsPublicRoutes } from './dertofderts-public.routing';
import { CanDeactivateScoreGuard } from './guards/score-candeactivate.guard';
import { Conductor } from './services/dertofderts-public.conductor';
import { Repository } from './services/dertofderts-public.repository';
import { SessionService } from './services/dertofderts-public.session';
import { Tracker } from './services/dertofderts-public.tracker';

@NgModule({
  imports: [
    AppSharedModule,
    MaterialLayoutsModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    FlexLayoutModule,
    AppSharedModule,
    AppConfirmModule,
    TermsRegionModule,
    DertOfDertsPublicRoutes
  ],
  declarations: [
    HomeComponent,
    HowToEnterComponent,
    HowToJudgeComponent,
    JudgingComponent,
    ScoreComponent,
    TalksComponent,
    ResultsComponent,
    ScoreDialogComponent,
    ScoreSubmittedDialogComponent,
    RecoverSessionDialogComponent,
    ClosedToPublicDialogComponent,
    HeaderComponent,
    MarkingCriteriaSideBarComponent,
    ListComponent,
    TopHeaderComponent
  ],
  providers: [
    Conductor,
    Tracker,
    Repository,
    SessionService,
    CanDeactivateScoreGuard
  ],
  entryComponents: [
    ScoreDialogComponent,
    ScoreSubmittedDialogComponent,
    RecoverSessionDialogComponent,
    ClosedToPublicDialogComponent,
    WebsiteTermsComponent
  ]
})
export class DertOfDertsPublicModule { }

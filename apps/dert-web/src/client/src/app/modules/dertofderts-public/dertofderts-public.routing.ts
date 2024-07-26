import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { HowToEnterComponent } from './components/how-to-enter/how-to-enter.component';
import { HowToJudgeComponent } from './components/how-to-judge/how-to-judge.component';
import { JudgingComponent } from './components/judging/judging.component';
import { ResultsComponent } from './components/results/results.component';
import { ScoreComponent } from './components/score/score.component';
import { TalksComponent } from './components/talks/talks.component';
import { CanDeactivateScoreGuard } from './guards/score-candeactivate.guard';

const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'judging', component: JudgingComponent },
  { path: 'score/:id', component: ScoreComponent, canDeactivate: [CanDeactivateScoreGuard] },
  { path: 'talks', component: TalksComponent },
  { path: 'results', component: ResultsComponent },
  { path: 'howtoenter', component: HowToEnterComponent },
  { path: 'howtojudge', component: HowToJudgeComponent },
];

export const DertOfDertsPublicRoutes = RouterModule.forChild(routes);

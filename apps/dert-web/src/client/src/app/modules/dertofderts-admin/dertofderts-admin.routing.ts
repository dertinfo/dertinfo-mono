import { RouterModule, Routes } from '@angular/router';
import { AdminComplaintsComponent } from './components/admin-complaints/admin-complaints.component';
import { AdminHomeComponent } from './components/admin-home/admin-home.component';
import { AdminJudgesComponent } from './components/admin-judges/admin-judges.component';
import { AdminScorecardsComponent } from './components/admin-scorecards/admin-scorecards.component';
import { AdminSettingsComponent } from './components/admin-settings/admin-settings.component';
import { AdminSubmissionsComponent } from './components/admin-submissions/admin-submissions.component';
import { AdminTalksComponent } from './components/admin-talks/admin-talks.component';
import { DertOfDertsAdminComponent } from './dertofderts-admin.component';
import { DertOfDertsGuard } from './guards/dertofderts.guard';
import { EditSubmissionComponent } from './modules/submissions/components/edit-submission/edit-submission.component';
import { EditTalkComponent } from './modules/talks/components/edit-talk/edit-talk.component';
import { ScorecardsResolver } from './resolvers/scorecards.resolver';

const routes: Routes = [
  {
    path: '',
    component: DertOfDertsAdminComponent,
    canActivate: [DertOfDertsGuard],
    children: [
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'overview'
      },
      {
        path: 'overview',
        component: AdminHomeComponent,
        data: { title: 'Overview', breadcrumb: 'DODOVERVIEW' }
      },
      {
        path: 'submissions',
        component: AdminSubmissionsComponent,
        data: { title: 'Submissions', breadcrumb: 'DODSUBMISSIONS' }
      },
      {
        path: 'submissions/:id',
        component: EditSubmissionComponent,
        data: { title: 'Submission', breadcrumb: 'DODSUBMISSIONS' },
      },
      {
        path: 'judges',
        component: AdminJudgesComponent,
        data: { title: 'Judges', breadcrumb: 'DODJUDGES' },
      },
      {
        path: 'scorecards/j/:judgeId',
        component: AdminScorecardsComponent,
        resolve: { scorecards: ScorecardsResolver },
        data: { title: 'Score Cards By Judge', breadcrumb: 'DODSCORECARDS', cardsType: 'judge' },
      },
      {
        path: 'scorecards/s/:submissionId',
        component: AdminScorecardsComponent,
        resolve: { scorecards: ScorecardsResolver },
        data: { title: 'Score Cards By Submission', breadcrumb: 'DODSCORECARDS', cardsType: 'submission' },
      },
      {
        path: 'complaints',
        component: AdminComplaintsComponent,
        pathMatch: 'full',
        data: { title: 'Complaints', breadcrumb: 'DODCOMPLAINTS' }
      },
      {
        path: 'talks',
        component: AdminTalksComponent,
        pathMatch: 'full',
        data: { title: 'talks', breadcrumb: 'DODTALKS' },
      },
      {
        path: 'talks/:id',
        component: EditTalkComponent,
        data: { title: 'talks', breadcrumb: 'DODTALKS' },
      },
      {
        path: 'settings',
        component: AdminSettingsComponent,
        data: { title: 'settings', breadcrumb: 'DODSETTINGS' }
      }
    ]
  }
];

export const DertOfDertsAdminRoutes = RouterModule.forChild(routes);

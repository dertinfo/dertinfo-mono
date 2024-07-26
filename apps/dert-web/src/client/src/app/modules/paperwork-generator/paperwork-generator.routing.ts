import { Routes } from '@angular/router';

import { ScoreSheetsComponent } from './components/scoresheets/scoresheets.component';
import { SignInSheetsComponent } from './components/signinsheets/signinsheets.component';

export const PaperworkGeneratorRoutes: Routes = [
  {
    path: '',
    children: [{
      path: 'scoresheets/:id/populated',
      component: ScoreSheetsComponent,
      data: { title: 'ScoreSheets', spares: false },
    }, {
      path: 'scoresheets/:id/spares',
      component: ScoreSheetsComponent,
      data: { title: 'ScoreSheets', spares: true },
    }, {
      path: 'signinsheets/:id',
      component: SignInSheetsComponent,
      data: { title: 'SignInSheets' },
    }]
  }
];

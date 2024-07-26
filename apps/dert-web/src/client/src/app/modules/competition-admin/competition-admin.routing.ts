import { Routes } from '@angular/router';

import { CompetitionComponent } from './components/app-competition.component';
import { CheckingComponent } from './components/checking/checking.component';
import { CheckingResolver } from './components/checking/checking.resolver';
import { CheckingResolver2 } from './components/checking/checking2.resolver';
import { DancesComponent } from './components/dances/dances.component';
import { DancesResolver } from './components/dances/dances.resolver';
import { EntrantsComponent } from './components/entrants/entrants.component';
import { EntrantsResolver } from './components/entrants/entrants.resolver';
import { EntryAttributesComponent } from './components/entry-attributes/entryattributes.component';
import { EntryAttributesResolver } from './components/entry-attributes/entryattributes.resolver';
import { JudgesComponent } from './components/judges/judges.component';
import { JudgesResolver } from './components/judges/judges.resolver';
import { OverviewComponent } from './components/overview/overview.component';
import { PaperworkComponent } from './components/paperwork/paperwork.component';
import { ReportsComponent } from './components/reports/reports.component';
import { ResultsComponent } from './components/results/results.component';
import { ResultsResolver } from './components/results/results.resolver';
import { ScoringComponent } from './components/scoring/scoring.component';
import { ScoringResolver } from './components/scoring/scoring.resolver';
import { SettingsComponent } from './components/settings/settings.component';
import { SettingsResolver } from './components/settings/settings.resolver';
import { VenuesComponent } from './components/venues/venues.component';
import { VenuesResolver } from './components/venues/venues.resolver';
import { CompetitionAdminResolver } from './services/competition-admin.resolver';

const routes: Routes = [
  {
    path: '',
    redirectTo: '/dashboard',
    pathMatch: 'full'
  },
  {
    path: ':name/:id',
    component: CompetitionComponent,
    data: { title: '{{name}}', breadcrumb: '{{name}}' },
    resolve: { competitionOverview: CompetitionAdminResolver },
    children: [
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'overview'
      },
      {
        path: 'overview',
        component: OverviewComponent,
        data: { title: 'Overview', breadcrumb: 'OVERVIEW' },
      },
      {
        path: 'settings',
        component: SettingsComponent,
        data: { title: 'Competition Settings', breadcrumb: 'COMPETITIONSETTINGS' },
        resolve: { settings: SettingsResolver }
      },
      {
        path: 'scoring',
        component: ScoringComponent,
        data: { title: 'Competition Scoring', breadcrumb: 'COMPETITIONSCORING' },
        resolve: { scoring: ScoringResolver }
      },
      {
        path: 'venues',
        component: VenuesComponent,
        data: { title: 'Competition Venues', breadcrumb: 'COMPETITIONVENUES' },
        resolve: { venues: VenuesResolver }
      },
      {
        path: 'entrants',
        component: EntrantsComponent,
        data: { title: 'Competition Entrants', breadcrumb: 'COMPETITIONENTRANTS' },
        resolve: {
          entrants: EntrantsResolver,
          entryAttributes: EntryAttributesResolver,
          settings: SettingsResolver
        }
      },
      {
        path: 'judges',
        component: JudgesComponent,
        data: { title: 'Competition Judges', breadcrumb: 'COMPETITIONJUDGES' },
        resolve: { judges: JudgesResolver }
      },
      {
        path: 'entryattributes',
        component: EntryAttributesComponent,
        data: { title: 'Competition Entry Attributes', breadcrumb: 'COMPETITIONENTRYATTRIBUTES' },
        resolve: { entryAttributes: EntryAttributesResolver }
      },
      {
        path: 'dances',
        component: DancesComponent,
        data: { title: 'Competition Dances', breadcrumb: 'COMPETITIONDANCES' },
        resolve: { dances: DancesResolver }
      },
      {
        path: 'paperwork',
        component: PaperworkComponent,
        data: { title: 'Competition Paperwork', breadcrumb: 'COMPETITIONPAPERWORK' }
      },
      {
        path: 'results',
        component: ResultsComponent,
        data: { title: 'Competition Results', breadcrumb: 'COMPETITIONRESULTS' },
        resolve: {
          collatedDances: ResultsResolver,
          dances: DancesResolver
        }
      },
      {
        path: 'reports',
        component: ReportsComponent,
        data: { title: 'Competition Reports', breadcrumb: 'COMPETITIONREPORTS' }
      },
      {
        path: 'check/:danceId',
        component: CheckingComponent,
        data: { title: 'Check Result', breadcrumb: 'CHECKRESULT' },
        resolve: {
          danceResult: CheckingResolver,
          judgeSlotInformation: CheckingResolver2
        }
      }

    ]
  }
];

export const CompetitionAdminRoutes = routes;

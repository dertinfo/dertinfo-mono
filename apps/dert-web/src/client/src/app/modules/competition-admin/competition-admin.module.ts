// Angular
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

// Modules
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { ChartsModule } from 'ng2-charts';
import { FileUploadModule } from 'ng2-file-upload';
import { AppSharedModule } from '../../shared/app-shared.module';
import { AppCompetitionSharedModule } from './modules/app-competition-shared/app-competition-shared.module';

// Components
import { CompetitionComponent } from './components/app-competition.component';

// Modules

// Services
import { CompetitionAdminRoutes } from './competition-admin.routing';
import { DancesComponent } from './components/dances/dances.component';
import { DancesResolver } from './components/dances/dances.resolver';
import { EntrantsComponent } from './components/entrants/entrants.component';
import { EntrantsResolver } from './components/entrants/entrants.resolver';
import { EntryAttributesComponent } from './components/entry-attributes/entryattributes.component';
import { EntryAttributesResolver } from './components/entry-attributes/entryattributes.resolver';
import { JudgeCreateComponent } from './components/judges/dialogs/judge-create/judge-create.component';
import { JudgeEditComponent } from './components/judges/dialogs/judge-edit/judge-edit.component';
import { JudgesComponent } from './components/judges/judges.component';
import { JudgesResolver } from './components/judges/judges.resolver';
import { OverviewComponent } from './components/overview/overview.component';
import { PaperworkComponent } from './components/paperwork/paperwork.component';
import { ScoreCategoryEditComponent } from './components/scoring/dialogs/scorecategory-edit/scorecategory-edit.component';
import { ScoreSetEditComponent } from './components/scoring/dialogs/scoreset-edit/scoreset-edit.component';
import { ScoringComponent } from './components/scoring/scoring.component';
import { ScoringResolver } from './components/scoring/scoring.resolver';
import { SettingsComponent } from './components/settings/settings.component';
import { SettingsResolver } from './components/settings/settings.resolver';
import { VenueCreateComponent } from './components/venues/dialogs/venue-create/venue-create.component';
import { VenueEditComponent } from './components/venues/dialogs/venue-edit/venue-edit.component';
import { VenuesComponent } from './components/venues/venues.component';
import { VenuesResolver } from './components/venues/venues.resolver';
import { CompetitionAdminConductor } from './services/competition-admin.conductor';
import { CompetitionAdminResolver } from './services/competition-admin.resolver';
import { CompetitionAdminTracker } from './services/competition-admin.tracker';

import { MaterialLayoutsModule } from 'app/material-bundles/material-layouts.module';
import { JudgesSelectModule } from 'app/shared/components/judges-select/judges-select.module';
import { VenuesSelectModule } from 'app/shared/components/venues-select/venues-select.module';
import { CheckingComponent } from './components/checking/checking.component';
import { CheckingResolver } from './components/checking/checking.resolver';
import { CheckingResolver2 } from './components/checking/checking2.resolver';
import { SplitScoresGridComponent } from './components/checking/split-scores-grid/split-scores-grid.component';
import { DanceCreateComponent } from './components/dances/dialogs/dance-create/dance-create.component';
import { AttributeSelectorComponent } from './components/entrants/components/attribute-selector.component';
import { JudgeRangeReportDialogComponent } from './components/reports/judge-range-report-dialog/judge-range-report-dialog.component';
import { JudgeRangeReportComponent } from './components/reports/judge-range-report/judge-range-report.component';
import { ReportsComponent } from './components/reports/reports.component';
import { CollatedGridComponent } from './components/results/collated-grid/collated-grid.component';
import { DancesGridComponent } from './components/results/dances-grid/dances-grid.component';
import { ResultsComponent } from './components/results/results.component';
import { ResultsResolver } from './components/results/results.resolver';
@NgModule({
    imports: [
        AppSharedModule,
        MaterialLayoutsModule,
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        NgxDatatableModule,
        ChartsModule,
        FileUploadModule,
        RouterModule.forChild(CompetitionAdminRoutes),
        AppCompetitionSharedModule,
        JudgesSelectModule,
        VenuesSelectModule
    ],
    declarations: [
        CompetitionComponent,
        OverviewComponent,
        SettingsComponent,
        ScoringComponent,
        ScoreSetEditComponent,
        ScoreCategoryEditComponent,
        VenuesComponent,
        EntrantsComponent,
        JudgesComponent,
        EntryAttributesComponent,
        DancesComponent,
        PaperworkComponent,
        VenueEditComponent,
        VenueCreateComponent,
        JudgeCreateComponent,
        JudgeEditComponent,
        AttributeSelectorComponent,
        CheckingComponent,
        ResultsComponent,
        CollatedGridComponent,
        DancesGridComponent,
        DanceCreateComponent,
        SplitScoresGridComponent,
        ReportsComponent,
        JudgeRangeReportComponent,
        JudgeRangeReportDialogComponent
    ],
    exports: [],
    providers: [
        CompetitionAdminConductor,
        CompetitionAdminTracker,
        CompetitionAdminResolver,
        SettingsResolver,
        ScoringResolver,
        VenuesResolver,
        EntrantsResolver,
        JudgesResolver,
        EntryAttributesResolver,
        DancesResolver,
        CheckingResolver,
        CheckingResolver2,
        ResultsResolver
    ]
})
export class CompetitionAdminModule { }

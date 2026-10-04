import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatLegacyButtonModule as MatButtonModule } from '@angular/material/legacy-button';
import { MatLegacyCardModule as MatCardModule } from '@angular/material/legacy-card';
import { MatLegacyDialogModule as MatDialogModule } from '@angular/material/legacy-dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatLegacyInputModule as MatInputModule } from '@angular/material/legacy-input';
import { MatLegacyListModule as MatListModule } from '@angular/material/legacy-list';
import { MatLegacyRadioModule as MatRadioModule } from '@angular/material/legacy-radio';
import { MatLegacySelectModule as MatSelectModule } from '@angular/material/legacy-select';
import { MatLegacyTableModule as MatTableModule } from '@angular/material/legacy-table';
import { MatLegacyTabsModule as MatTabsModule } from '@angular/material/legacy-tabs';

import { AppSharedModule } from 'app/shared/app-shared.module';
import { AdminComplaintsComponent } from './components/admin-complaints/admin-complaints.component';

import { AdminHomeComponent } from './components/admin-home/admin-home.component';
import { AdminJudgesComponent } from './components/admin-judges/admin-judges.component';
import { AdminScorecardsComponent } from './components/admin-scorecards/admin-scorecards.component';
import { AdminSettingsComponent } from './components/admin-settings/admin-settings.component';
import { AdminSubmissionsComponent } from './components/admin-submissions/admin-submissions.component';
import { AdminTalksComponent } from './components/admin-talks/admin-talks.component';
import { DertOfDertsAdminComponent } from './dertofderts-admin.component';
import { DertOfDertsAdminRoutes } from './dertofderts-admin.routing';
import { DertOfDertsGuard } from './guards/dertofderts.guard';
import { ComplaintsModule } from './modules/complaints/complaints.module';
import { JudgesModule } from './modules/judges/judges.module';
import { SettingsModule } from './modules/settings/settings.module';
import { SubmissionsModule } from './modules/submissions/submissions.module';
import { TalksModule } from './modules/talks/talks.module';
import { ScorecardsResolver } from './resolvers/scorecards.resolver';
import { Conductor } from './services/dertofderts-admin.conductor';
import { Mediator } from './services/dertofderts-admin.mediator';
import { Repository } from './services/dertofderts-admin.repository';
import { Tracker } from './services/dertofderts-admin.tracker';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatCardModule,
    MatTableModule,
    MatTabsModule,
    MatIconModule,
    MatButtonModule,
    MatDialogModule,
    MatInputModule,
    MatListModule,
    MatRadioModule,
    MatSelectModule,
    FlexLayoutModule,
    AppSharedModule,
    ComplaintsModule,
    SubmissionsModule,
    JudgesModule,
    TalksModule,
    SettingsModule,
    DertOfDertsAdminRoutes
  ],
  declarations: [

    AdminComplaintsComponent,
    AdminHomeComponent,
    AdminJudgesComponent,
    AdminScorecardsComponent,
    AdminSettingsComponent,
    AdminSubmissionsComponent,
    AdminTalksComponent,
    DertOfDertsAdminComponent
  ],
  providers: [
    ScorecardsResolver,
    DertOfDertsGuard,
    Tracker,
    Conductor,
    Repository,
    Mediator
  ]
})
export class DertOfDertsAdminModule { }

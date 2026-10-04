import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatListModule } from '@angular/material/list';
import { MatRadioModule } from '@angular/material/radio';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { MatTabsModule } from '@angular/material/tabs';

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

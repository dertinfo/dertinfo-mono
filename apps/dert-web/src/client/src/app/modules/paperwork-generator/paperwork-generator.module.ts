import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { MaterialLayoutsModule } from 'app/material-bundles/material-layouts.module';
import { PaperworkGeneratorRepository } from 'app/modules/repositories';
import { AppSharedModule } from '../../shared/app-shared.module';
import { ScoreSheetsComponent } from './components/scoresheets/scoresheets.component';
import { SignInSheetsComponent } from './components/signinsheets/signinsheets.component';
import { PaperworkGeneratorRoutes } from './paperwork-generator.routing';
import { PaperworkGeneratorConductor } from './services/paperwork-generator.conductor';

@NgModule({
  imports: [
    MaterialLayoutsModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    AppSharedModule,
    RouterModule.forChild(PaperworkGeneratorRoutes)
  ],
  declarations: [
    ScoreSheetsComponent,
    SignInSheetsComponent
  ],
  providers: [
    PaperworkGeneratorConductor,
    PaperworkGeneratorRepository
  ]

})
export class PaperworkGeneratorModule { }

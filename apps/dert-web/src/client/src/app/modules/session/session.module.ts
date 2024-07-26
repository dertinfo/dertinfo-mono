import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { MaterialLayoutsModule } from 'app/material-bundles/material-layouts.module';
import { AppSharedModule } from 'app/shared/app-shared.module';

import { ErrorComponent } from './components/error/error.component';
import { ForbiddenComponent } from './components/forbidden/forbidden.component';
import { NotAuthorisedComponent } from './components/not-authorised/not-authorised.component';
import { NotFoundComponent } from './components/not-found/not-found.component';
import { WarmupComponent } from './components/warmup/warmup.component';
import { SessionRoutes } from './session.routing';

@NgModule({
  imports: [
    AppSharedModule,
    MaterialLayoutsModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forChild(SessionRoutes)
  ],
  declarations: [
    NotFoundComponent,
    ErrorComponent,
    NotAuthorisedComponent,
    ForbiddenComponent,
    WarmupComponent
  ]
})
export class SessionModule { }

import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';
import { RouterModule } from '@angular/router';
import { MaterialLayoutsModule } from 'app/material-bundles/material-layouts.module';
import { AppSharedModule } from 'app/shared/app-shared.module';
import { CreateTalkComponent } from './components/create-talk/create-talk.component';
import { EditTalkComponent } from './components/edit-talk/edit-talk.component';
import { TalksComponent } from './components/talks.component';
@NgModule({
  imports: [
    AppSharedModule,
    MaterialLayoutsModule,
    CommonModule,
    FormsModule,
    FlexLayoutModule,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatDialogModule,
    MatIconModule,
    MatTableModule,
    ReactiveFormsModule,
    RouterModule
  ],
  declarations: [
    TalksComponent,
    CreateTalkComponent,
    EditTalkComponent
  ],
  exports: [TalksComponent],
  entryComponents: [
    CreateTalkComponent
  ]
})
export class TalksModule { }

import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatRadioModule } from '@angular/material/radio';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { RouterModule } from '@angular/router';

import { AppSharedModule } from 'app/shared/app-shared.module';

import { DodSubmissionCreateComponent } from './components/dod-submission-create/dod-submission-create.component';
import { EditSubmissionComponent } from './components/edit-submission/edit-submission.component';
import { SubmissionsComponent } from './components/submissions.component';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        MatButtonModule,
        MatCardModule,
        MatTableModule,
        MatTableModule,
        MatIconModule,
        MatIconModule,
        MatIconModule,
        MatIconModule,
        MatInputModule,
        MatCardModule,
        MatSelectModule,
        MatRadioModule,
        ReactiveFormsModule,
        RouterModule,
        FlexLayoutModule,
        AppSharedModule,
    ],
    declarations: [
        SubmissionsComponent,
        DodSubmissionCreateComponent,
        EditSubmissionComponent
    ],
    exports: [
        SubmissionsComponent,
        EditSubmissionComponent
    ]
})
export class SubmissionsModule { }

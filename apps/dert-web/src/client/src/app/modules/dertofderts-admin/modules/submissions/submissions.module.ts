import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatLegacyButtonModule as MatButtonModule } from '@angular/material/legacy-button';
import { MatLegacyCardModule as MatCardModule } from '@angular/material/legacy-card';
import { MatIconModule } from '@angular/material/icon';
import { MatLegacyInputModule as MatInputModule } from '@angular/material/legacy-input';
import { MatLegacyRadioModule as MatRadioModule } from '@angular/material/legacy-radio';
import { MatLegacySelectModule as MatSelectModule } from '@angular/material/legacy-select';
import { MatLegacyTableModule as MatTableModule } from '@angular/material/legacy-table';
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

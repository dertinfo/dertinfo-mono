import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatLegacyButtonModule as MatButtonModule } from '@angular/material/legacy-button';
import { MatLegacyCardModule as MatCardModule } from '@angular/material/legacy-card';
import { MatLegacyDialogModule as MatDialogModule } from '@angular/material/legacy-dialog';
import { MatLegacyFormFieldModule as MatFormFieldModule } from '@angular/material/legacy-form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatLegacyInputModule as MatInputModule } from '@angular/material/legacy-input';
import { MatLegacyTableModule as MatTableModule } from '@angular/material/legacy-table';
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
    exports: [TalksComponent]
})
export class TalksModule { }

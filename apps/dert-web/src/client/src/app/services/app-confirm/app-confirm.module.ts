import { NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { MatLegacyButtonModule as MatButtonModule } from '@angular/material/legacy-button';
import { MatLegacyDialogModule as MatDialogModule } from '@angular/material/legacy-dialog';
import { AppConfirmService } from './app-confirm.service';

import { AppComfirmComponent } from './app-confirm.component';

@NgModule({
    imports: [
        MatDialogModule,
        MatButtonModule,
        FlexLayoutModule
    ],
    exports: [AppComfirmComponent],
    declarations: [AppComfirmComponent],
    providers: [AppConfirmService]
})
export class AppConfirmModule { }

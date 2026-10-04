import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';

import { OverviewComponent } from './components/overview/overview.component';
import { SystemAdminComponent } from './system-admin.component';
import { SystemAdminRoutes } from './system-admin.routing';

@NgModule({
    imports: [
        CommonModule,
        MatCardModule,
        MatIconModule,
        MatListModule,
        FlexLayoutModule,
        SystemAdminRoutes
    ],
    declarations: [
        OverviewComponent,
        SystemAdminComponent
    ],
    providers: [
    ]
})
export class SystemAdminModule { }

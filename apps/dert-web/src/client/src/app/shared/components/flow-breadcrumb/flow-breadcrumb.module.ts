import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatRadioModule } from '@angular/material/radio';
import { TranslateModule } from '@ngx-translate/core';
import { FlowBreadcrumbComponent } from './components/flow-breadcrumb.component';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    FlexLayoutModule,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatRadioModule,
    ReactiveFormsModule,
    TranslateModule
  ],
  declarations: [
    FlowBreadcrumbComponent
  ],
  providers: [],
  exports: [
    FlowBreadcrumbComponent
  ]
})
export class FlowBreadcrumbModule {}

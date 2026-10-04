import { CommonModule } from '@angular/common';
import { ModuleWithProviders, NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatMomentDateModule } from '@angular/material-moment-adapter';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatRadioModule } from '@angular/material/radio';
import { MatTabsModule } from '@angular/material/tabs';
import { MatToolbarModule } from '@angular/material/toolbar';
import { TranslateModule } from '@ngx-translate/core';
import { JudgesSelectComponent } from './components/judges-select.component';
import { JudgesSelectMediator } from './services/judges-select.mediator';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    FlexLayoutModule,
    MatCardModule,
    MatDatepickerModule,
    MatIconModule,
    MatInputModule,
    MatToolbarModule,
    MatButtonModule,
    MatMomentDateModule,
    MatRadioModule,
    MatTabsModule,
    ReactiveFormsModule,
    TranslateModule
  ],
  declarations: [
    JudgesSelectComponent
  ],
  providers: [
    JudgesSelectMediator,
  ],
  exports: [
    JudgesSelectComponent,
  ]
})
export class JudgesSelectModule {
  static forRoot(): ModuleWithProviders<JudgesSelectModule> {
    return {
      ngModule: JudgesSelectModule,
      providers: [ JudgesSelectMediator ]
    };
  }
}

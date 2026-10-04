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
import { MembersSelectComponent } from './components/members-select.component';
import { MemberSelectMediator } from './services/members-select.mediator';

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
    // removed temp - CommonDirectivesModule,
    TranslateModule
  ],
  declarations: [
    MembersSelectComponent
  ],
  providers: [
    MemberSelectMediator,
  ],
  exports: [
    MembersSelectComponent,
  ]
})
export class MembersSelectModule {
  static forRoot(): ModuleWithProviders<MembersSelectModule> {
    return {
      ngModule: MembersSelectModule,
      providers: [ MemberSelectMediator ]
    };
  }
}

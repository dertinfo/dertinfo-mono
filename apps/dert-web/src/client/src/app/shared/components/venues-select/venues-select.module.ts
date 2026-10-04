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
import { MaterialLayoutsModule } from 'app/material-bundles/material-layouts.module';
import { AppSharedModule } from 'app/shared/app-shared.module';
import { VenuesSelectComponent } from './components/venues-select.component';
import { VenuesSelectMediator } from './services/venues-select.mediator';

@NgModule({
  imports: [
    AppSharedModule,
    MaterialLayoutsModule,
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
    VenuesSelectComponent
  ],
  providers: [
    VenuesSelectMediator
  ],
  exports: [
    VenuesSelectComponent,
  ]
})
export class VenuesSelectModule {
  static forRoot(): ModuleWithProviders<VenuesSelectModule> {
    return {
      ngModule: VenuesSelectModule,
      providers: [ VenuesSelectMediator ]
    };
  }
}

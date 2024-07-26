import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { MaterialLayoutsModule } from 'app/material-bundles/material-layouts.module';
import { CookiePolicyComponent } from './components/cookie-policy/cookie-policy.component';
import { DataPolicyComponent } from './components/data-policy/data-policy.component';
import { EventConfigureTermsComponent } from './components/event-configure-terms/event-configure-terms.component';
import { GroupConfigureTermsComponent } from './components/group-configure-terms/group-configure-terms.component';
import { GroupRegisterTermsComponent } from './components/group-register-terms/group-register-terms.component';
import { WebsiteTermsComponent } from './components/website-terms/website-terms.component';
import { TermsRegionComponent } from './terms-region.component';

@NgModule({
  imports: [
    RouterModule,
    MaterialLayoutsModule,
    CommonModule
  ],
  declarations: [
    TermsRegionComponent,
    CookiePolicyComponent,
    DataPolicyComponent,
    EventConfigureTermsComponent,
    GroupConfigureTermsComponent,
    GroupRegisterTermsComponent,
    WebsiteTermsComponent,
  ],
  providers: [],
  exports: [
    TermsRegionComponent,
    CookiePolicyComponent,
    DataPolicyComponent,
    EventConfigureTermsComponent,
    GroupConfigureTermsComponent,
    GroupRegisterTermsComponent,
    WebsiteTermsComponent,
  ]
})
export class TermsRegionModule {}

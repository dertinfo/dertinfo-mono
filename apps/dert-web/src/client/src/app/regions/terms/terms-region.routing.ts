import { RouterModule, Routes } from '@angular/router';
import { CookiePolicyComponent } from './components/cookie-policy/cookie-policy.component';
import { DataPolicyComponent } from './components/data-policy/data-policy.component';
import { WebsiteTermsComponent } from './components/website-terms/website-terms.component';

const routes: Routes = [
    {
        path: 'terms',
        component: WebsiteTermsComponent
    },
    {
        path: 'datapolicy',
        component: DataPolicyComponent
    },
    {
        path: 'cookiepolicy',
        component: CookiePolicyComponent
    }
];

export const TermsRegionRoutes = routes;

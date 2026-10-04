import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Router, RouterStateSnapshot } from '@angular/router';

import { AuthService } from 'app/core/authentication/auth.service';

@Injectable()
export class DertOfDertsGuard  {

    constructor(private router: Router, private authService: AuthService) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        if (this.authService.isAuthenticated() && this.authService.userData().dertOfDertsAdmin) {
            return true;
        }
        this.router.navigate(['/session/401']);
        return false;
    }
}

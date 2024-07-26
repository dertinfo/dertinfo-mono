import { Component } from '@angular/core';
import { AuthService } from 'app/core/authentication/auth.service';

@Component({
    selector: 'app-auth-callback',
    templateUrl: './auth-callback.component.html'
})
export class AuthCallbackComponent {

    constructor(public auth: AuthService) {
        auth.handleAuthentication();
    }
}

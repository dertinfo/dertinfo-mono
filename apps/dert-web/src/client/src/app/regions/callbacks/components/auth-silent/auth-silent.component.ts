import { Component } from '@angular/core';
import { AuthService } from 'app/core/authentication/auth.service';

@Component({
    selector: 'app-auth-silent',
    templateUrl: './auth-silent.component.html'
})
export class AuthSilentComponent {

    constructor(public auth: AuthService) {

       this.auth.parseSilentResponse();
    }
}

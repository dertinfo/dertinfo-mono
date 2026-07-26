import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../../../core/authentication/auth.service';

/**
 * Public logout landing page.
 * Shows a signing-out message, then starts Auth0 logout (returnTo = site root → /home).
 */
@Component({
  selector: 'app-sign-out',
  templateUrl: './sign-out.component.html',
  styleUrls: ['./sign-out.component.css']
})
export class SignoutComponent implements OnInit {

  private logoutStarted = false;

  constructor(private authService: AuthService) { }

  ngOnInit() {
    if (this.logoutStarted) {
      return;
    }
    this.logoutStarted = true;
    this.authService.logout();
  }
}

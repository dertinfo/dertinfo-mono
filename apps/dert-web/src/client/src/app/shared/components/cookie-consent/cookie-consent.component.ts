import { Component, OnInit } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';

@Component({
  selector: 'app-cookie-consent',
  templateUrl: './cookie-consent.component.html',
  styleUrls: ['./cookie-consent.component.scss']
})
export class CookieConsentComponent implements OnInit {

  public isVisible = false;

  constructor(private cookieService: CookieService) { }

  ngOnInit() {

    // CHeck For Cookie-Consent Cookie
    const cookieValue = this.cookieService.get('cookie-consent');

    if (cookieValue !== 'consented') {
      this.isVisible = true;
    }
  }

  public onAcceptCookiesClick() {

    // Set Cookie-Consent Cookie
    this.cookieService.set( 'cookie-consent', 'consented' );

    // Put a cookie to identify that we can hide this
    this.isVisible = false;
  }
}

import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MatDrawer } from '@angular/material/sidenav';
import {
  NavigationEnd,
  ResolveEnd,
  ResolveStart,
  RouteConfigLoadEnd,
  RouteConfigLoadStart,
  Router
} from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import * as Ps from 'perfect-scrollbar';
import { Subscription } from 'rxjs';
import {filter} from 'rxjs/operators';
import { AuthService } from '../../core/authentication/auth.service';
import { UserData } from '../../models/auth/userdata.model';

@Component({
  selector: 'app-authenticated-region',
  templateUrl: './authenticated-region.component.html'
})
export class AuthenticatedRegionComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];

  private isMobile;
  @ViewChild(MatDrawer, { static: true }) private sidenav: MatDrawer;
  userData: UserData = null;
  isSidenavOpen: Boolean = false;
  isModuleLoading: Boolean = false;
  moduleLoaderSub: Subscription;

  constructor(
    private router: Router,
    public translate: TranslateService,
    public authService: AuthService,
  ) {
    // Close sidenav after route change in mobile
    router.events.pipe(filter(event => event instanceof NavigationEnd)).subscribe((routeChange: NavigationEnd) => {
      if (this.isNavOver()) {
        this.sidenav.close();
      }
    });

    // Translator init
    const browserLang: string = translate.getBrowserLang();
    translate.use(browserLang.match(/en|fr/) ? browserLang : 'en');
  }
  ngOnInit() {
    // Initialize Perfect scrollbar for sidenav
    const navigationHold = document.getElementById('scroll-area');
    Ps.initialize(navigationHold, {
      suppressScrollX: true
    });

    // Load the user data into the side nav
    if (!this.userData) {
      this.userData = this.authService.userData();
    }

    // Listen for changes to the user data
    this._subscriptions.push(this.authService.userDataChanged$.subscribe((userdata) => {
      if (userdata) {
        this.userData = userdata;
      }
    }));

    // FOR MODULE LOADER FLAG
    this._subscriptions.push(this.router.events.subscribe(event => {
      if (event instanceof RouteConfigLoadStart || event instanceof ResolveStart) {
        this.isModuleLoading = true;
      }
      if (event instanceof RouteConfigLoadEnd || event instanceof ResolveEnd) {
        this.isModuleLoading = false;
      }
    }));
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }
  isNavOver() {
    return window.matchMedia(`(max-width: 960px)`).matches;
  }
}

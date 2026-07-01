import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AuthService } from '../../../core/authentication/auth.service';
import * as domHelper from '../../../helpers/dom.helper';
@Component({
  selector: 'topbar',
  templateUrl: './topbar.template.html'
})
export class TopbarComponent implements OnInit {
  @Input() sidenav;
  @Input() notificPanel;
  @Output() onLangChange = new EventEmitter<any>();
  currentLang = 'en';
  availableLangs = [{
    name: 'English',
    code: 'en',
  }, {
    name: 'Spanish',
    code: 'es',
  }];
  egretThemes;
  userData;

  constructor(private authService: AuthService) {}
  ngOnInit() {
    this.userData = this.authService.userData();
  }
  setLang() {
    this.onLangChange.emit(this.currentLang);
  }
  toggleNotifications() {
    this.notificPanel.toggle();
  }
  toggleSidenav() {
    this.sidenav.toggle();
  }
  toggleCollapse() {
        const appBody = document.body;
        domHelper.toggleClass(appBody, 'collapsed-menu');
        domHelper.removeClass(document.getElementsByClassName('has-submenu'), 'open');
    }

    onSignOutClick() {
      this.authService.logout();
    }
}

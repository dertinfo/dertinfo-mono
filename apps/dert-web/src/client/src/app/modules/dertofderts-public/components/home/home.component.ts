import { Component, OnInit } from '@angular/core';
import { ClientSettingsService } from 'app/core/services/clientsettings.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  public dodOpenToPublic = false;

  constructor(private clientSettings: ClientSettingsService) { }

  ngOnInit() {
    this.dodOpenToPublic = this.clientSettings.dodOpenToPublic;
  }

}

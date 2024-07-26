import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-dod-intro-banner',
  templateUrl: './dod-intro-banner.component.html',
  styleUrls: ['./dod-intro-banner.component.scss']
})
export class DodIntroBannerComponent implements OnInit {

  public registrationLink = '/dertofderts/howtoenter';

  constructor() { }

  ngOnInit() {
  }

}

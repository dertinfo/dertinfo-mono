import { Component, Input, OnInit } from '@angular/core';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';

@Component({
  selector: 'app-facebook-embed',
  templateUrl: './facebook-embed.component.html',
  styleUrls: ['./facebook-embed.component.scss']
})
export class FacebookEmbedComponent implements OnInit {

  @Input() facebookLink: string;
  @Input() autoplay = false;

  public safeURL: SafeUrl;

  constructor(private _sanitizer: DomSanitizer) {

  }

  ngOnInit() {

    const url = `https://www.facebook.com/plugins/video.php?href=${this.facebookLink}&show_text=0&width=560&height=315`;

    this.safeURL = this._sanitizer.bypassSecurityTrustResourceUrl(url);
  }
}

import { Input } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';

@Component({
  selector: 'app-youtube-embed',
  templateUrl: './youtube-embed.component.html',
  styleUrls: ['./youtube-embed.component.scss']
})
export class YouTubeEmbedComponent implements OnInit {

  @Input() youtubeLink: string;
  @Input() autoplay = false;

  public safeURL: SafeUrl;

  constructor(private _sanitizer: DomSanitizer) {

  }

  ngOnInit() {

    this.safeURL = this._sanitizer.bypassSecurityTrustResourceUrl(this.youtubeLink);
  }

}

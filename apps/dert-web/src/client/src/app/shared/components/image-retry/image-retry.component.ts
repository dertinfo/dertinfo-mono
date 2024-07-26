import { ChangeDetectionStrategy, Component, EventEmitter, Input, OnChanges, OnInit, Output } from '@angular/core';
import * as domHelper from '../../../helpers/dom.helper';

@Component({
  selector: 'app-image-retry',
  templateUrl: './image-retry.component.html',
  styleUrls: ['./image-retry.component.css']
})
export class ImageRetryComponent implements OnInit, OnChanges {

  private iteration = 1;
  private maxIterations = 4;
  private loaderImage = './assets/images/image-retry-loading.gif';
  private brokenImage = './assets/images/image-retry-broken.png';

  @Input() src: string;
  @Input() alt: string;

  public imageToDisplay: string;

  constructor() { }

  ngOnInit() {
    this.imageToDisplay = this.src;
  }

  ngOnChanges() {
    this.imageToDisplay = this.src;
  }

  public onError() {
    this.imageToDisplay = this.loaderImage;
    const wait = 1000 * this.iteration * this.iteration;
    // wait - 1s then 4s then 9s;

    if (this.iteration < this.maxIterations) {

      // This is to ensure protection against a broken, broken image;
      if (this.imageToDisplay === this.brokenImage) { return; }

      // Retry to load the image
      this.iteration++;
      setTimeout(() => {
        console.log('Retrying to display.', wait);
        this.imageToDisplay = this.src;
      }, wait);
    } else {

      // Resort to the broken image
      this.imageToDisplay = this.brokenImage;
    }

  }
}

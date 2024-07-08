import { Pipe, PipeTransform } from '@angular/core';
import { ILogger } from '../providers/diagnostics/logger';

@Pipe({ name: 'sizedImage' })
export class ImageSizePipe implements PipeTransform {

  constructor(private _logger: ILogger) {
      this._logger.trace('ImageSizePipe - constructor ', ['ImageSizePipe']);
  }

  transform(value: string, imageSize: string): string {
    this._logger.trace('ImageSizePipe - transform ', ['ImageSizePipe']);

    if (value && value.indexOf('/' + imageSize + '/') === -1) {

      /* find the last index of / and insert size before it*/
      const originalImageUrl: string = value;
      const lastSlashIndex = originalImageUrl.lastIndexOf('/');

      // Check if we have a full url where it ends with /filename.ext
      if (lastSlashIndex > 0) {

        // Construct the sized image url
        const filename = originalImageUrl.slice(lastSlashIndex + 1, originalImageUrl.length);
        const prefixPath = originalImageUrl.slice(0, lastSlashIndex);
        const sizedImageUrl = prefixPath + '/' + imageSize.toLowerCase() + '/' + filename;

        this._logger.trace('ImageSizePipe - transform ', ['ImageSizePipe'], sizedImageUrl);

        return sizedImageUrl + '?firsttry';
      }
    }

    return value;
  }

}

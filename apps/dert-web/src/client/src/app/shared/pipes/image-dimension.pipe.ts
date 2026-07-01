import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'imageDimension' })
export class ImageDimensionPipe implements PipeTransform {

  constructor() { }

  transform(value: string, imageSize: string): string {

    if (value && value !== '' && value.indexOf('/' + imageSize + '/') === -1) {

      /* find the last index of / and insert size before it*/
      const originalImageUrl: string = value;
      const lastSlashIndex = originalImageUrl.lastIndexOf('/');

      // Check if we have a full url where it ends with /filename.ext
      if (lastSlashIndex > 0) {

        // Construct the sized image url
        const filename = originalImageUrl.slice(lastSlashIndex + 1, originalImageUrl.length);
        const prefixPath = originalImageUrl.slice(0, lastSlashIndex);
        const sizedImageUrl = prefixPath + '/' + imageSize.toLowerCase() + '/' + filename;

        return sizedImageUrl;
      }
    }

    if (!value || value === '' || value.indexOf('.') === -1) {
      return ''; // if the image value is not set
    }

    return value;
  }

}

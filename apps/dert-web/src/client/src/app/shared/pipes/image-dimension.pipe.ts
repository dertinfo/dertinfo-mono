import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'imageDimension' })
export class ImageDimensionPipe implements PipeTransform {

  constructor() { }

  private static readonly sizeFolders = ['originals', '100x100', '480x360'];

  transform(value: string, imageSize: string): string {

    if (value && value !== '' && value.indexOf('/' + imageSize + '/') === -1) {

      /* find the last index of / and insert size before it*/
      const originalImageUrl: string = value;
      const lastSlashIndex = originalImageUrl.lastIndexOf('/');

      // Check if we have a full url where it ends with /filename.ext
      if (lastSlashIndex > 0) {

        const filename = originalImageUrl.slice(lastSlashIndex + 1, originalImageUrl.length);
        const prefixPath = originalImageUrl.slice(0, lastSlashIndex);
        const sizedImageUrl = this.withSizeFolder(prefixPath, imageSize.toLowerCase(), filename);

        return sizedImageUrl;
      }
    }

    if (!value || value === '' || value.indexOf('.') === -1) {
      return ''; // if the image value is not set
    }

    return value;
  }

  /**
   * New uploads are {container}/originals/{file}. Swap that folder for the requested size
   * (resize writes {container}/480x360/{file}, not originals/480x360).
   * Older URLs with no size folder still get the size inserted before the file name.
   */
  private withSizeFolder(prefixPath: string, imageSize: string, filename: string): string {
    const folderSlash = prefixPath.lastIndexOf('/');
    const parentFolder = folderSlash >= 0 ? prefixPath.slice(folderSlash + 1).toLowerCase() : '';

    if (ImageDimensionPipe.sizeFolders.indexOf(parentFolder) !== -1) {
      const root = prefixPath.slice(0, folderSlash);
      return root + '/' + imageSize + '/' + filename;
    }

    return prefixPath + '/' + imageSize + '/' + filename;
  }

}

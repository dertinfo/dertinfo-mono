import { DatePipe } from '@angular/common';
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'dateTimeFormat',
    standalone: false
})
export class DateTimeFormatPipe extends DatePipe implements PipeTransform {
  transform(value: any, args?: any): any {
    return super.transform(value, 'dd MMM yyyy (HH:mm)');
  }
}

import { Pipe, PipeTransform } from '@angular/core';

/**
 * FilterBoolPipe class
 */
@Pipe({
  name: 'filterBool'
})
export class FilterBoolPipe implements PipeTransform {

  transform(items: any[], activated: boolean): any[] {

    // Return empty array, if array is falsy
    if (!items) { return []; }

    if (!activated) { return items; }

    // Retrun the filtered array
    return items.filter(item => {
      if (item && item.activated) {
        return item.activated == activated;
      }
      return false;
    });
   }
}
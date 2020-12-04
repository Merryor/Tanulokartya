import { Pipe, PipeTransform } from '@angular/core';

/**
 * FilterPipe class
 */
@Pipe({
  name: 'filter'
})
export class FilterPipe implements PipeTransform {

  transform(items: any[], searchText: string, fieldName: string): any[] {

    // Return empty array if array is falsy
    if (!items) { return []; }

    // Return the original array if search text is empty
    if (!searchText) { return items; }

    // Convert the searchText to lower case
    searchText = searchText.toLowerCase();

    // Retrun the filtered array
    return items.filter(item => {
      if (item && item[fieldName]) {
        return item[fieldName].toLowerCase().includes(searchText);
      }
      return false;
    });
   }
}
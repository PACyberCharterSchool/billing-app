import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'orderBy'
})
export class OrderByPipe implements PipeTransform {

  transform(items: Array<any>, args?: any): any {
    if (items) {
      return items.sort(function(a, b) {
            if (a && b) {
              if (args.property == 'schoolDistrict') {
                if (a[args.property].name.toUpperCase() < b[args.property].name.toUpperCase()) {
                  return -1 * args.direction;
                }
                else if (a[args.property].name.toUpperCase() > b[args.property].name.toUpperCase()) {
                  return 1 * args.direction;
                }
                else {
                  return 0;
                }
              }
              else {
                if (a[args.property] < b[args.property]) {
                  return -1 * args.direction;
                } else if (a[args.property] > b[args.property]) {
                  return 1 * args.direction;
                } else {
                  return 0;
                }
              }
            }
      }
    );
    }
  }
}

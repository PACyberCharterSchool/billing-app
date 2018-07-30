import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'iepEnrolled'
})
export class IepEnrolledPipe implements PipeTransform {

  transform(value: any, args?: any): any {
    return value ? 'Yes' : 'No';
  }

}

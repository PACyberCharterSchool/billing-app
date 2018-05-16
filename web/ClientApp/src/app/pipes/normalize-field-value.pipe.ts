import { Pipe, PipeTransform } from '@angular/core';

import * as moment from 'moment';

@Pipe({
  name: 'normalizeFieldValue'
})
export class NormalizeFieldValuePipe implements PipeTransform {

  transform(value: any, args?: any): any {
    let v = value;
    const formats = [
      'MM/DD/YYYY',
      moment.ISO_8601
    ];

    if (typeof(value) === 'string') {
      if (moment(value, formats, true).isValid()) {
        const d = new Date(value);
        v = d.toLocaleDateString();
      }
    }

    return v;
  }

}

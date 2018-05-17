import { Pipe, PipeTransform } from '@angular/core';

import { SchoolDistrict } from '../models/school-district.model';

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

    switch (typeof(value)) {
      case 'string':
        if (moment(value, formats, true).isValid()) {
          const d = new Date(value);
          v = d.toLocaleDateString();
        }
        break;
      case 'object':
        if (value.name) {
          v = value.name;
        }
        break;
    }

    return v;
  }

}

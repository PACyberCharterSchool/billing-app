import { Pipe, PipeTransform } from '@angular/core';

import { SchoolDistrict } from '../models/school-district.model';

import * as moment from 'moment';

@Pipe({
  name: 'normalizeFieldValue'
})
export class NormalizeFieldValuePipe implements PipeTransform {

  private isDateValue(val: any) {
    const formats = [
      'MM/DD/YYYY',
      'M/D/YYYY',
      'M/DD/YYYY',
      'MM/D/YYYY',
      moment.ISO_8601
    ];

    return (val && moment(val, formats, true).isValid());
  }

  transform(value: any, args?: any): any {
    let v = value;
    switch (typeof(value)) {
      case 'string':
        if (this.isDateValue(value)) {
          const d = new Date(value);
          v = d.toLocaleDateString();
        }
        break;
      case 'object':
        if (this.isDateValue(value)) {
          const d = new Date(value);
          v = d.toLocaleDateString();
        } else if (value && value.name) {
          v = value.name;
        }
        break;
    }

    return v;
  }

}

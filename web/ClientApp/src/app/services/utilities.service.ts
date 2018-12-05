import { Injectable } from '@angular/core';

import * as moment from 'moment';

@Injectable()
export class UtilitiesService {

  constructor() { }

  objectKeys(obj) {
    if (obj) {
      return Object.keys(obj);
    }

    return [];
  }

  objectValues(obj) {
    if (obj) {
      return Object.values(obj);
    }

    return [];
  }

  uniqueItemsInArray(theArray) {
    if (theArray && theArray instanceof Array) {
      const unique = (v, i, s) => {
        return theArray.indexOf(v) === i;
      };

      const uniqVals = theArray.filter(unique);

      return uniqVals;
    }
  }

  pick(obj, keys) {
    return keys.map(k => k in obj ? { [k]: obj[k] } : {})
      .reduce((res, o) => Object.assign(res, o), {});
  }

  isDateValue(val: string): boolean {
    const formats = [
      'MM/DD/YYYY',
      'M/D/YYYY',
      'MM/D/YYYY',
      'M/DD/YYYY',
      moment.ISO_8601
    ];

    return moment(val, formats, true).isValid();
  }

  getSortClass(o: { property: string, isDescending: boolean }, property: string): object {
    return {
      'fa-sort': o.property !== property,
      'fa-sort-desc': o.property === property && o.isDescending,
      'fa-sort-asc': o.property === property && !o.isDescending,
    };
  }
}

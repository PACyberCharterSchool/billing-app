import { Injectable } from '@angular/core';

@Injectable()
export class UtilitiesService {

  constructor() { }

  objectKeys(obj) {
    if (obj) {
      return Object.keys(obj);
    }
  }

  objectValues(obj) {
    if (obj) {
      return Object.values(obj);
    }
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
    return keys.map(k => k in obj ? {[k]: obj[k]} : {})
               .reduce((res, o) => Object.assign(res, o), {});
  }
}
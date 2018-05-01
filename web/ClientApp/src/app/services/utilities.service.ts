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
}

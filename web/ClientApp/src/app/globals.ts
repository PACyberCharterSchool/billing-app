import { Injectable } from '@angular/core';

const SCROLLING_TAKE_LIMIT = 100;
const CURRENT_SCHOOL_YEAR = '2017-2018';

@Injectable()
export class Globals {
  readonly take = SCROLLING_TAKE_LIMIT;
  readonly currentSchoolYear = CURRENT_SCHOOL_YEAR;

  public constructor() { }
}

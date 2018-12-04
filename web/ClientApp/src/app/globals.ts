import { Injectable } from '@angular/core';

const SCROLLING_TAKE_LIMIT = 100;
const CURRENT_SCHOOL_YEAR = '2017-2018';
const SPRITE_2X_FILE = 'assets/images/sprite_2X.png';
const SPRITE_1X_FILE = 'assets/images/sprite_1X.png';
const REPORT_NAME_DATE_FORMAT = 'YYYYMMDDHHmmss';

@Injectable()
export class Globals {
  readonly take = SCROLLING_TAKE_LIMIT;
  readonly currentSchoolYear = CURRENT_SCHOOL_YEAR;
  readonly sprite1X = SPRITE_1X_FILE;
  readonly sprite2X = SPRITE_2X_FILE;
  readonly dateFormat = REPORT_NAME_DATE_FORMAT;

  public constructor() { }
}

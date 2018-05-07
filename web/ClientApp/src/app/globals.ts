import { Injectable } from '@angular/core';

const SCROLLING_TAKE_LIMIT = 100;

@Injectable()
export class Globals {
  readonly take = SCROLLING_TAKE_LIMIT;

  public constructor() { }
}

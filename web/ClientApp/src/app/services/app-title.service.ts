import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';

@Injectable()
export class AppTitleService {

  private appTitleSource = new BehaviorSubject<string>('PACBill');
  currentAppTitle = this.appTitleSource.asObservable();

  constructor() { }

  changeAppTitle(title: string) {
    this.appTitleSource.next(title);
  }
}

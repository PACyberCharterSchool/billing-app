import { Directive } from '@angular/core';

import {
  AbstractControl,
  ValidationErrors,
  NG_VALIDATORS,
  ValidatorFn
} from '@angular/forms';

import { NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';

@Directive({
  selector: '[appFormerCurrentIepDateConflictValidator]',
  providers: [{ provide: NG_VALIDATORS, useExisting: FormerCurrentIepDateConflictValidatorDirective, multi: true }]
})
export class FormerCurrentIepDateConflictValidatorDirective {

  constructor() { }

  validate(control: AbstractControl): ValidationErrors {
    return this.formerCurrentDateConfictValidator(control);
  }

  private dateCompare(o1: NgbDateStruct, o2: NgbDateStruct): number {
    if (!o1 && !o2) { return 0; }
    if (!o1) { return -1; }
    if (!o2) { return 1; }

    const date1: Date = new Date(o1.year, o1.month - 1, o1.day);
    const date2: Date = new Date(o2.year, o2.month - 1, o2.day);

    if (date1 < date2) {
      return -1;
    } else if (date1 === date2) {
      return 0;
    } else {
      return 1;
    }
  }

  private isValidDate(d: NgbDateStruct): boolean {
    return d && !isNaN(d.year) && !isNaN(d.month) && !isNaN(d.day);
  }

  private formerCurrentDateConfictValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
    const formerDate: AbstractControl = control.get('studentInfo.formerIepDate');
    const currentDate: AbstractControl = control.get('studentInfo.currentIepDate');

    if (currentDate && currentDate.value) {
      return formerDate &&
        formerDate.value &&
        this.isValidDate(formerDate.value) &&
        (this.dateCompare(formerDate.value, currentDate.value) === -1
          || this.dateCompare(formerDate.value, currentDate.value) === 0) ? null : { 'appFormerCurrentDateConflictValidator': true };
    }

    return null;
  }
}

import { Directive } from '@angular/core';

import {
  Validator,
  AbstractControl,
  ValidationErrors,
  NG_VALIDATORS,
  ValidatorFn
} from '@angular/forms';

@Directive({
  selector: '[appEnrollmentWithdrawalDateConflictValidator]',
  providers: [{ provide: NG_VALIDATORS, useExisting: EnrollmentWithdrawalDateConflictValidatorDirective, multi: true }]
})
export class EnrollmentWithdrawalDateConflictValidatorDirective {

  constructor() { }

  validate(control: AbstractControl): ValidationErrors {
    return this.enrollmentWithdrawalDateConfictValidator(control);
  }

  private dateCompare(o1: Object, o2: Object): number {
    if (!o1 && !o2) { return 0; }
    if (!o1) { return -1; }
    if (!o2) { return 1; }

    const date1: Date = new Date(o1.year, o1.month - 1, o1.day);
    const date2: Date = new Date(o2.year, o2.month - 1, o2.day);

    if (date1 < date2) {
      return -1;
    } else if (date1 === date2) {
      return 0;
    } else if (date1 > date2) {
      return 1;
    }
  }

  private isValidDate(d: Object): boolean {
    return d && !isNaN(d.year) && !isNaN(d.month) && !isNaN(d.day);
  }

  private enrollmentWithdrawalDateConfictValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
    const withdrawalDate: AbstractControl = control.get('studentInfo.withdrawalDate');
    const enrollmentDate: AbstractControl = control.get('studentInfo.enrollmentDate');

    if (withdrawalDate && withdrawalDate.value) {
      const result = enrollmentDate &&
        enrollmentDate.value &&
        this.isValidDate(enrollmentDate.value) &&
        this.dateCompare(enrollmentDate.value, withdrawalDate.value) !== 1 ?
          null : { 'appEnrollmentWithdrawalDateConflictValidator': true };

      return result;
    }

    return enrollmentDate && enrollmentDate.value ? null : { 'appEnrollmentWithdrawalDateConflictValidator': true };
  }
}

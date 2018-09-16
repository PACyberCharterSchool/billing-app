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

  private enrollmentWithdrawalDateConfictValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
    const enrollmentDate: AbstractControl = control.get('enrollmentDate');
    const withdrawalDate: AbstractControl = control.get('withdrawalDate');

    return enrollmentDate &&
      withdrawalDate &&
      enrollmentDate.value <= withdrawalDate.value ? null : { 'appEnrollmentWithdrawalDateConflictValidator': true };
  }
}

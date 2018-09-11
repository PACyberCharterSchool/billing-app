import { Directive } from '@angular/core';

import {
  Validator,
  AbstractControl,
  ValidationErrors,
  NG_VALIDATORS,
  ValidatorFn
} from '@angular/forms';

@Directive({
  selector: '[appAcademicYearConflictValidator]',
  providers: [{ provide: NG_VALIDATORS, useExisting: AcademicYearConflictValidatorDirective, multi: true }]
})
export class AcademicYearConflictValidatorDirective implements Validator {

  constructor() { }

  validate(control: AbstractControl): ValidationErrors {
    return academicYearConflictValidator(control);
  }
}

const academicYearConflictValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const academicYear: AbstractControl = control.get('academicYear');
  const academicYearSplit: AbstractControl = control.get('academicYearSplit');

  return academicYear && academicYearSplit && academicYear.value === academicYearSplit.value ?
    { 'appAcademicYearConflictValidator': true } : null;
};

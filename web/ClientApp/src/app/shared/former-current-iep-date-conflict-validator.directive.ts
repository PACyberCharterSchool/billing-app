import { Directive } from '@angular/core';

import {
  AbstractControl,
  ValidationErrors,
  NG_VALIDATORS,
  ValidatorFn
} from '@angular/forms';

@Directive({
  selector: '[appFormerCurrentIepDateConflictValidator]',
  providers: [{ provide: NG_VALIDATORS, useExisting: FormerCurrentIepDateConflictValidatorDirective, multi: true }]
})
export class FormerCurrentIepDateConflictValidatorDirective {

  constructor() { }

  validate(control: AbstractControl): ValidationErrors {
    return this.formerCurrentDateConfictValidator(control);
  }

  private formerCurrentDateConfictValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
    const formerDate: AbstractControl = control.get('formerIepDate');
    const currentDate: AbstractControl = control.get('currentIepDate');

    return formerDate &&
      currentDate &&
      formerDate.value <= currentDate.value ? null : { 'appFormerCurrentDateConflictValidator': true };
  }
}

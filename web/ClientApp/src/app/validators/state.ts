import { FormControl, ValidationErrors } from '@angular/forms';
import { States } from '../models/states';

export const InvalidStateError: ValidationErrors = {
  invalidState: 'Please enter a valid state.',
};

export function validateState(control: FormControl): ValidationErrors | null {
  if (!States.some(s => s.abbreviation === control.value)) {
    return InvalidStateError;
  }

  return null;
}

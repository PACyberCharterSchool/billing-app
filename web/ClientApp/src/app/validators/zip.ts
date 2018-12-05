import { FormControl, ValidationErrors } from '@angular/forms';

export const InvalidZipCodeError: ValidationErrors = {
  invalidZipCode: 'Please enter a valid zip code.',
};

export function validateZipCode(control: FormControl): ValidationErrors | null {
  const rgx = /^\d{5}(?:\-\d{4})?$/;
  if (!rgx.test(control.value)) {
    return InvalidZipCodeError;
  }

  return null;
}

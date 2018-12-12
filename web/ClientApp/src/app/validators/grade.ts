import { FormControl, ValidationErrors } from '@angular/forms';

export const GradeValidationError: ValidationErrors = {
  invalidGrade: 'Please enter a valid grade.',
};

const grades = [
  'K5F',
  '1',
  '2',
  '3',
  '4',
  '5',
  '6',
  '7',
  '8',
  '9',
  '10',
  '11',
  '12',
];

export function validateGrade(control: FormControl): ValidationErrors | null {
  if (!grades.includes(control.value)) {
    return GradeValidationError;
  }

  return null;
}

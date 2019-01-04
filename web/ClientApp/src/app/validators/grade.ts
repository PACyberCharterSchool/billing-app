import { FormControl, ValidationErrors } from '@angular/forms';
import { Grades } from '../models/grades';

export const GradeValidationError: ValidationErrors = {
  invalidGrade: 'Please enter a valid grade.',
};

export function validateGrade(control: FormControl): ValidationErrors | null {
  if (!Grades.includes(control.value)) {
    return GradeValidationError;
  }

  return null;
}

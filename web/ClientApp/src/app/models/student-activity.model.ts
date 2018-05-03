enum StudentActivityType {
  NewStudent = 'NewStudent',
  DateOfBirthChange = 'DateOfBirthChange',
  DistrictEnrollment = 'DistrictEnrollment',
  DistrictWithdrawal = 'DistrictWithdraw',
  NameChange = 'NameChange',
  GradeChange = 'GradeChange',
  AddressChange = 'AddressChange',
  SpecialEducationEnrollment = 'SpecialEducationEnrollment',
  SpecialEducationWithdrawal = 'SpecialEducationWithdraw',
  CurrentIepChange = 'CurrentIepChange',
  FormerIepChange = 'FormerIepChange',
  NorepChange = 'NorepChange',
  PASecuredChange = 'PASecuredChange',
}

export class StudentActivity {
  StudentActivity: StudentActivityType;
}

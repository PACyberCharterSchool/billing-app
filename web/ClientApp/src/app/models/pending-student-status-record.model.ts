export class PendingStudentStatusRecord {
  id: number;
  schoolDistrictId: number;
  schoolDistrictName: string;
  studentId: string;
  studentFirstName: string;
  studentMiddleInitial: string;
  studentLastName: string;
  studentGradeLevel: string;
  studentDateOfBirth: Date;
  studentStreet1: string;
  studentStreet2: string;
  studentCity: string;
  studentState: string;
  studentZipCode: string;
  ActivitySchoolYear: string;
  studentEnrollmentDate?: Date;
  studentWithdrawalDate?: Date;
  studentIsSpecialEducation: boolean;
  studentCurrentIep?: Date;
  studentFormerIep?: Date;
  studentNorep?: Date;
  studentPaSecuredId?: number;
  batchTime: string;
  batchFilename: string;
  batchHash: string;
}

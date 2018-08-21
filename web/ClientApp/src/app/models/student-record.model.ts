import { SchoolDistrict } from './school-district.model';

export class StudentRecordsHeader {
  id: number;
  scope: string; // 2018.01 or 2018-2019
  filename: string;
  created: Date;
  locked: boolean;
  records: StudentRecord[];
}

export class StudentRecord {
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
  activitySchoolYear: string;
  studentEnrollmentDate: Date;
  studentWithdrawalDate: Date;
  studentIsSpecialEducation: boolean;
  studentCurrentIep: Date;
  studentFormerIep: Date;
  studentNorep: Date;
  studentPaSecuredId: number;
  lastUpdated: Date;
  lazyLoader?: Object;

  header: StudentRecordsHeader;
}

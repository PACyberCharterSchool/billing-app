export enum AuditRecordActivityType {
  UpdateTemplate = 'UpdateTemplate',
  EditStudentRecord = 'EditStudentRecord',
  UpdateSchoolCalendar = 'UpdateSchoolCalendar',
  SchoolDistricts = 'SchoolDistricts'
}

export class AuditRecord {
  id: number;
  username: string;
  activity: string;
  timestamp: Date;
  field: string;
  identifier: string;
  next: any;
  previous: any;
}

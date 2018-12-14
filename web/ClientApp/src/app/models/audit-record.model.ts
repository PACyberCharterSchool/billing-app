export enum AuditRecordActivityType {
  UpdateTemplate = 'UpdateTemplate',
  EditStudentRecord = 'EditStudentRecord',
  UpdateSchoolCalendar = 'UpdateSchoolCalendar',
  SchoolDistricts = 'SchoolDistricts'
}

export class AuditDetail {
  id: number;
  field: string;
  next?: string;
  previous?: string;
}

export class AuditRecord {
  id: number;
  username: string;
  activity: string;
  timestamp: Date;
  identifier: string;
  details: AuditDetail[];
}

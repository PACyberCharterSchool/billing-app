export enum AuditRecordActivityType {
  UpdateInvoiceTemplate = 'UpdateInvoiceTemplate',
  EditStudentRecord = 'EditStudentRecord',
  SchoolCalendars = 'SchoolCalendars',
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

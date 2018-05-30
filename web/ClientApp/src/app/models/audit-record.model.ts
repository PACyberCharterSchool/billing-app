export enum AuditRecordType {
  InvoiceTemplates,
  StudentRecords,
  SchoolCalendars,
  SchoolDistricts
}

export class AuditRecord {
  id: number;
  username: string;
  activity: string;
  type: AuditRecordType;
  timestamp: Date;
  oldValue: any;
  newValue: any;
}

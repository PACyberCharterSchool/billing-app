export enum ReportType {
  Invoice = 'Invoice',
  StudentInformation = 'StudentInformation',
  BulkInvoice = 'BulkInvoice',
  BulkStudentInformation = 'BulkStudentInformation'
}

class ReportMetadata {
  id: number;
  type: string;
  schoolYear: string;
  name: string;
  approved: boolean;
  created: Date;
}

export class Report extends ReportMetadata {
  data: string;
  xlsx: Blob;
  pdf: Blob;
}

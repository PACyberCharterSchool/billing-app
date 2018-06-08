export enum ReportType {
  Invoice = 'Invoice',
  StudentInformation = 'StudentInformation'
}

export class ReportMetadata {
  id: number;
  type: ReportType;
  schoolYear: string;
  name: string;
  approved: boolean;
  created: Date;
}

export class Report extends ReportMetadata {
  data: string;
  xlsx: Blob;
}

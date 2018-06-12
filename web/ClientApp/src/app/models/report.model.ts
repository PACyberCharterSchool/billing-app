export enum ReportType {
  Invoice = 'Invoice',
  StudentInformation = 'StudentInformation'
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
}

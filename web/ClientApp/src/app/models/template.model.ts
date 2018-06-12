import { ReportType } from './report.model';

class TemplateMetadata
{
  id: number;
  reportType: string;
  schoolYear: string;
  name: string;
  created: Date;
  lastUpdated: Date;
}

export class Template extends TemplateMetadata
{
  content: Blob;
}


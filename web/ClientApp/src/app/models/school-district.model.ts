import { StudentRecord } from './student-record.model';

export class SchoolDistrict {
  id: number;
  rate: string;
  specialEducationRate: string;
  alternateRate: string;
  alternateSpecialEducationRate: string;
  name: string;
  aun: string;
  paymentType: string;
  created: Date;
  lastUpdate: Date;
}

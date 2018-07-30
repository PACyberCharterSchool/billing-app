import { Student } from '../models/student.model';

export class SchoolDistrict {
  id: number;
  rate: number;
  specialEducationRate: number;
  alternateRate: number;
  alternateSpecialEducationRate: number;
  name: string;
  aun: number;
  paymentType: string;
  created: Date;
  lastUpdate: Date;
}

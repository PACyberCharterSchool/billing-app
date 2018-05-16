import { SchoolDistrict } from '../models/school-district.model';

enum PaymentType {
  Check = 'Check',
  UniPay = 'UniPay'
}

export class Payment {
  id: number;
  paymentId: string;
  split: number;
  date: Date;
  created: Date;
  externalId: string;
  type: PaymentType;
  amount: number;
  schoolYear: string;
  lastUpdated: Date;
  schoolDistrict: SchoolDistrict;
}

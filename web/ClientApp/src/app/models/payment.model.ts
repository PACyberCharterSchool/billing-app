import { SchoolDistrict } from './school-district.model';

export enum PaymentType {
  Check = 'Check',
  UniPay = 'UniPay'
}

export class Payment {
  id: number;
  paymentId: string;
  split: number;
  splitAmount: number;
  date: Date;
  created: Date;
  externalId: string;
  type: PaymentType;
  amount: number;
  schoolYear: string;
  schoolYearSplit: string;
  lastUpdated: Date;
  schoolDistrict: SchoolDistrict;
}

import { SchoolDistrict } from './school-district.model';

export enum PaymentType {
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
  splitAmount: number;
  schoolYear: string;
  schoolYearSplit: string;
  lastUpdated: Date;
  schoolDistrict: SchoolDistrict;
  username: string;
}

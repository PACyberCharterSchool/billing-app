import { SchoolDistrict } from './school-district.model';

export class Student {
  id: number;
  city: string;
  firstName: string;
  lastName: string;
  middleInitial: string;
  paCyberId: number;
  paSecuredId: number;
  grade: number;
  dateOfBirth: Date;
  isSpecialEducation: boolean;
  currentIep: Date;
  formerIep: Date;
  norepDate: string;
  state: string;
  street1: string;
  street2: string;
  zipCode: string;
  schoolDistrict: SchoolDistrict;
  startDate: Date;
  endDate: Date;
  created: Date;
  lastUpdated: Date;
}

import { Injectable } from '@angular/core';

const baseYear = '2000';

@Injectable()
export class AcademicYearsService {
  private academicYears: string[];

  constructor() {
    this.initAcademicYearChronology();
  }

  initAcademicYearChronology() {
    const currentDate = new Date();
    let currentYear = +baseYear;
    this.academicYears = [];

    while (currentYear <= currentDate.getFullYear()) {
      this.academicYears.push(`${currentYear.toString()} - ${(currentYear + 1).toString()}`);
      currentYear++;
    }
  }

  getAcademicYears(): string[] {
    return this.academicYears;
  }
}

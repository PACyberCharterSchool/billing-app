import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

import { SchoolDistrict } from '../../../models/school-district.model';

import { StudentsService } from '../../../services/students.service';
import { StudentRecordsService } from '../../../services/student-records.service';

import { StudentRecord } from '../../../models/student-record.model';

@Component({
  selector: 'app-student-advanced-filter',
  templateUrl: './student-advanced-filter.component.html',
  styleUrls: ['./student-advanced-filter.component.scss']
})
export class StudentAdvancedFilterComponent implements OnInit {

  public grades: number[] = [
    0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12
  ];
  public date: Date;
  public startDate: Date;
  public endDate: Date;
  public enrollmentDate: Date;
  public withdrawalDate: Date;
  public studentRecords: StudentRecord[];

  @Input() schoolDistricts: SchoolDistrict[];
  @Input() allStudentRecords: StudentRecord[];
  @Input() scope: string;

  constructor(
    private studentsService: StudentsService,
    private studentRecordsService: StudentRecordsService
  ) { }

  @Output() studentsUpdated: EventEmitter<StudentRecord[]> = new EventEmitter();

  ngOnInit() {
  }

  public dateSelectedDOBDateHandler(date: Date) {
    console.log('StudentAdvancedFilter.dateSelectedDOBDateHandler(): date is ', date);
    this.studentRecordsService.getHeaderByScopeByDob(this.scope, date).subscribe(
      data => {
        this.studentsUpdated.emit(data['header']['records']);
      },
      error => {
      }
    );
  }

  public dateSelectedStartDateHandler(date: Date): void {
    this.startDate = date;
    if (this.endDate) {
      this.studentRecordsService.getHeaderByScopeByStartByEnd(this.scope, this.startDate, this.endDate).subscribe(
        data => {
          console.log('StudentAdvancedFilter.dateSelectedStartDateHandler():  data is ', data['header']['records']);
        },
        error => {
          console.log('StudentAdvancedFilter.dateSelectedStartDateHandler():  error is ', error);
        }
      );
    } else {
      this.studentRecordsService.getHeaderByScopeByStart(this.scope, this.startDate).subscribe(
        data => {
          console.log('StudentAdvancedFilter.dateSelectedStartDateHandler():  data is ', data['header']['records']);
        },
        error => {
          console.log('StudentAdvancedFilter.dateSelectedStartDateHandler():  error is ', error);
        }
      );
    }
    // this.studentRecords = this.allStudentRecords.filter((sr) => {
    //   if (this.endDate) {
    //     if (sr.studentEnrollmentDate >= this.startDate && sr.studentEnrollmentDate <= this.endDate) {
    //       return true;
    //     }
    //   }

    //   if (sr.studentEnrollmentDate <= date) {
    //     return true;
    //   }

    //   return false;
    // });
  }

  public dateSelectedEndDateHandler(date: Date): void {
    this.endDate = date;
    if (this.startDate) {
      this.studentRecordsService.getHeaderByScopeByStartByEnd(this.scope, this.startDate, date).subscribe(
        data => {
          console.log('StudentAdvancedFilterComponent.dateSelectedEndDateHandler():  data is ', data['header']['records']);
          this.studentRecords = data['header']['records'];
        },
        error => {
          console.log('StudentAdvancedFilterComponent.dateSelectedEndDateHandler():  error is ', error);
        }
      );
    } else {
      this.studentRecordsService.getHeaderByScopeByEnd(this.scope, date).subscribe(
        data => {
          console.log('StudentAdvancedFilterComponent.dateSelectedEndDateHandler():  data is ', data['header']['records']);
        },
        error => {
          console.log('StudentAdvancedFilterComponent.dateSelectedEndDateHandler():  error is ', error);
        }
      );
    }
    // this.studentsService.getStudentsFilteredByEndDate(date).subscribe(
    //   data => {
    //     this.studentRecords = data['students'];
    //   },
    //   error => {
    //     console.log('error');
    //   }
    // );
  }

  filterBySchoolDistrict(schoolId: number) {
    this.studentRecordsService.getHeaderByScopeBySchoolDistrict(this.scope, schoolId).subscribe(
      data => {
        this.studentsUpdated.emit(data['header']['records']);
      },
      error => {
        console.log('StudentsAdvancedFilterComponent.filterBySchoolDistrict():  error is ', error);
      }
    );
    // this.studentsService.getStudentsFilteredBySchoolDistrict(schoolId).subscribe(
    //   data => {
    //     this.studentsUpdated.emit(data['students']);
    //     console.log('StudentsListComponent.filterBySchoolDistrict():  students are ', data);
    //   }
    // );
  }

  filterByGrade(grade: number) {
    if (grade) {
      this.studentRecordsService.getHeaderByScopeByGrade(this.scope, grade).subscribe(
        data => {
          console.log('StudentAdvancedFilterComponent.filterByGrade(): data is ', data);
        },
        error => {
          console.log('StudentAdvancedFilterComponent.filterByGrade(): error is ', error);
        }
      );
      // this.studentsService.getStudentsFilteredByGrade(grade).subscribe(
      //   data => {
      //     this.studentsUpdated.emit(data['students']);
      //     console.log('StudentAdvancedFilterComponent.filterByGrade(): students are ', data);
      //   },
      //   error => {
      //     console.log('StudentAdvancedFilterComponent.filterByGrade(): error is ', error);
      //   }
      // );
    }
  }

  filterByDateOfBirth(dob: Date) {
    if (dob) {
      this.studentRecordsService.getHeaderByScopeByDob(this.scope, dob).subscribe(
        data => {
          console.log('StudentAdvancedFilterComponent.filterByDateOfBirth(): students are ', data);
          this.studentRecords = data['header']['records'];
        },
        error => {
          console.log('StudentAdvancedFilterComponent.filterByDateOfBirth():  error is ', error);
        }
      );
      // this.studentsService.getStudentsFilteredByDateOfBirth(dob).subscribe(
      //   data => {
      //     this.studentsUpdated.emit(data['students']);
      //     console.log('StudentAdvancedFilterComponent.filterByDateOfBirth(): students are ', data);
      //   },
      //   error => {
      //     console.log('StudentAdvancedFilterComponent.filterByDateOfBirth():  error is ', error);
      //   }
      // );
    }
  }

  filterByIep(iep: boolean) {
    this.studentRecordsService.getHeaderByScopeByIep(this.scope, iep).subscribe(
      data => {
        this.studentsUpdated.emit(data['header']['records']);
        console.log('StudentAdvancedFilterComponent.filterByIep():  data is ', data);
      },
      error => {
        console.log('StudentAdvancedFilterComponent.filterByIep():  error is ', error);
      }
    );

    // this.studentsService.getStudentsFilteredByIep(iep).subscribe(
    //   data => {
    //     this.studentsUpdated.emit(data['students']);
    //     console.log('StudentAdvancedFilterComponent.filterByIep(): students are ', data);
    //   },
    //   error => {
    //     console.log('StudentAdvancedFilterComponent.filterByIep():  error is ', error);
    //   }
    // );
  }
}

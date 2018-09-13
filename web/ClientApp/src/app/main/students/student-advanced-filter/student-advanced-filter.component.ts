import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';

import { SchoolDistrict } from '../../../models/school-district.model';

import { StudentsService } from '../../../services/students.service';
import { StudentRecordsService } from '../../../services/student-records.service';

import { StudentRecord } from '../../../models/student-record.model';

import { NgxSpinnerService } from 'ngx-spinner';
import { StudentDatepickerComponent } from '../student-datepicker/student-datepicker.component';

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
  public selectedGrade: string;
  public selectedSchoolDistrict: string;
  public selectedIep: string;
  public advancedFilterErrorMsg: string;

  @Input() schoolDistricts: SchoolDistrict[];
  @Input() allStudentRecords: StudentRecord[];
  @Input() scope: string;

  @ViewChild('studentDatepickerDOB')
  studentDatepickerDOBComponent: StudentDatepickerComponent;
  @ViewChild('studentDatepickerEnrollment')
  studentDatepickerEnrollmentComponent: StudentDatepickerComponent;
  @ViewChild('studentDatepickerWithdrawal')
  studentDatepickerWithdrawalComponent: StudentDatepickerComponent;

  constructor(
    private studentsService: StudentsService,
    private studentRecordsService: StudentRecordsService,
    private ngxSpinnerService: NgxSpinnerService
  ) { }

  @Output() studentsUpdated: EventEmitter<StudentRecord[]> = new EventEmitter();

  ngOnInit() {
    this.selectedGrade = 'Select';
    this.selectedSchoolDistrict = 'Select';
    this.selectedIep = 'Select';
    this.advancedFilterErrorMsg = '';
  }

  public initAllFilterControls(): void {
    this.date = null;
    this.startDate = null;
    this.endDate = null;
    this.enrollmentDate = null;
    this.withdrawalDate = null;
    this.studentDatepickerDOBComponent.model = null;
    this.studentDatepickerEnrollmentComponent.model = null;
    this.studentDatepickerWithdrawalComponent.model = null;
    this.selectedGrade = 'Select';
    this.selectedSchoolDistrict = 'Select';
    this.selectedIep = 'Select';
    this.advancedFilterErrorMsg = '';
  }

  public dateSelectedDOBDateHandler(date: Date) {
    console.log('StudentAdvancedFilter.dateSelectedDOBDateHandler(): date is ', date);
    this.ngxSpinnerService.show();
    this.studentRecordsService.getHeaderByScopeByDob(this.scope, date).subscribe(
      data => {
        this.studentsUpdated.emit(data['header']['records']);
        console.log('StudentAdvancedFilterComponent.dateSelectedDOBDateHandler():  data is ', data);
        this.ngxSpinnerService.hide();
      },
      error => {
        console.log('StudentAdvancedFilterComponent.dateSelectedDOBDateHandler():  error is ', error);
        this.ngxSpinnerService.hide();
      }
    );
  }

  public dateSelectedStartDateHandler(date: Date): void {
    this.startDate = date;
    this.ngxSpinnerService.show();
    if (this.endDate) {
      if (this.startDate <= this.endDate) {
        this.studentRecordsService.getHeaderByScopeByStartByEnd(this.scope, this.startDate, this.endDate).subscribe(
          data => {
            this.studentsUpdated.emit(data['header']['records']);
            console.log('StudentAdvancedFilter.dateSelectedStartDateHandler():  data is ', data['header']['records']);
            this.ngxSpinnerService.hide();
          },
          error => {
            console.log('StudentAdvancedFilter.dateSelectedStartDateHandler():  error is ', error);
            this.ngxSpinnerService.hide();
          }
        );
      } else {
        this.ngxSpinnerService.hide();
      }
    } else {
      this.studentRecordsService.getHeaderByScopeByStart(this.scope, this.startDate).subscribe(
        data => {
          this.studentsUpdated.emit(data['header']['records']);
          console.log('StudentAdvancedFilter.dateSelectedStartDateHandler():  data is ', data['header']['records']);
          this.ngxSpinnerService.hide();
        },
        error => {
          console.log('StudentAdvancedFilter.dateSelectedStartDateHandler():  error is ', error);
          this.ngxSpinnerService.hide();
        }
      );
    }
  }

  public dateSelectedEndDateHandler(date: Date): void {
    this.endDate = date;
    this.ngxSpinnerService.show();
    if (this.startDate) {
      if (this.startDate <= this.endDate) {
        this.studentRecordsService.getHeaderByScopeByStartByEnd(this.scope, this.startDate, date).subscribe(
          data => {
            console.log('StudentAdvancedFilterComponent.dateSelectedEndDateHandler():  data is ', data['header']['records']);
            this.studentRecords = data['header']['records'];
            this.ngxSpinnerService.hide();
          },
          error => {
            console.log('StudentAdvancedFilterComponent.dateSelectedEndDateHandler():  error is ', error);
            this.ngxSpinnerService.hide();
          }
        );
      } else {
        this.ngxSpinnerService.hide();
      }
    } else {
      this.studentRecordsService.getHeaderByScopeByEnd(this.scope, date).subscribe(
        data => {
          console.log('StudentAdvancedFilterComponent.dateSelectedEndDateHandler():  data is ', data['header']['records']);
          this.studentRecords = data['header']['records'];
          this.ngxSpinnerService.hide();
        },
        error => {
          console.log('StudentAdvancedFilterComponent.dateSelectedEndDateHandler():  error is ', error);
          this.ngxSpinnerService.hide();
        }
      );
    }
  }

  public filterBySchoolDistrict(schoolId: number) {
    this.selectedSchoolDistrict = this.schoolDistricts.find((s) => +s.aun === schoolId).name;
    this.ngxSpinnerService.show();
    this.studentRecordsService.getHeaderByScopeBySchoolDistrict(this.scope, schoolId).subscribe(
      data => {
        this.studentsUpdated.emit(data['header']['records']);
        this.ngxSpinnerService.hide();
      },
      error => {
        console.log('StudentsAdvancedFilterComponent.filterBySchoolDistrict():  error is ', error);
        this.ngxSpinnerService.hide();
      }
    );
  }

  public filterByGrade(grade: number) {
    if (grade) {
      this.selectedGrade = grade.toString();
      this.ngxSpinnerService.show();
      this.studentRecordsService.getHeaderByScopeByGrade(this.scope, grade).subscribe(
        data => {
          console.log('StudentAdvancedFilterComponent.filterByGrade(): data is ', data);
          this.studentsUpdated.emit(data['header']['records']);
          this.ngxSpinnerService.hide();
        },
        error => {
          console.log('StudentAdvancedFilterComponent.filterByGrade(): error is ', error);
          this.ngxSpinnerService.hide();
        }
      );
    }
  }

  public filterByDateOfBirth(dob: Date) {
    if (dob) {
      this.ngxSpinnerService.show();
      this.studentRecordsService.getHeaderByScopeByDob(this.scope, dob).subscribe(
        data => {
          console.log('StudentAdvancedFilterComponent.filterByDateOfBirth(): students are ', data);
          this.studentRecords = data['header']['records'];
          this.ngxSpinnerService.hide();
        },
        error => {
          console.log('StudentAdvancedFilterComponent.filterByDateOfBirth():  error is ', error);
          this.ngxSpinnerService.hide();
        }
      );
    }
  }

  public filterByIep(iep: boolean) {
    this.selectedIep = iep ? 'Yes' : 'No';
    this.ngxSpinnerService.show();
    this.studentRecordsService.getHeaderByScopeByIep(this.scope, iep).subscribe(
      data => {
        this.studentsUpdated.emit(data['header']['records']);
        this.ngxSpinnerService.hide();
        console.log('StudentAdvancedFilterComponent.filterByIep():  data is ', data);
      },
      error => {
        console.log('StudentAdvancedFilterComponent.filterByIep():  error is ', error);
        this.ngxSpinnerService.hide();
      }
    );
  }

  public isEnrollmentDateValid(): boolean {
    const result: boolean =
      (!this.startDate || (this.startDate && !this.endDate) || (this.startDate && this.endDate && this.startDate <= this.endDate));

    if (!result) {
      this.advancedFilterErrorMsg = 'Specified enrollment date is after withdrawal date.';
    }

    return result;
  }
}

import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

import { SchoolDistrict } from '../../../models/school-district.model';

import { StudentsService } from '../../../services/students.service';

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

  @Input() schoolDistricts: SchoolDistrict[];

  constructor(private studentsService: StudentsService) { }

  @Output() studentsUpdated: EventEmitter<StudentRecord[]> = new EventEmitter();

  ngOnInit() {
  }

  dateSelectedDOBDateHandler(date: Date) {
    console.log('StudentAdvancedFilter.dateSelectedDOBDateHandler(): date is ', date);
    this.studentsService.getStudentsFilteredByDateOfBirth(date).subscribe(
      data => {
        this.studentsUpdated.emit(data['students']);
      }
    );
  }


  filterStudentListBySchoolDistrict(schoolId: number) {
    this.studentsService.getStudentsFilteredBySchoolDistrict(schoolId).subscribe(
      data => {
        this.studentsUpdated.emit(data['students']);
        console.log('StudentsListComponent.filterStudentListBySchoolDistrict():  students are ', data);
      }
    );
  }

  filterStudentListByGrade(grade: number) {
    if (grade) {
      this.studentsService.getStudentsFilteredByGrade(grade).subscribe(
        data => {
          this.studentsUpdated.emit(data['students']);
          console.log('StudentAdvancedFilterComponent.filterStudentListByGrade(): students are ', data);
        },
        error => {
          console.log('StudentAdvancedFilterComponent.filterStudentListByGrade(): error is ', error);
        }
      );
    }
  }

  filterStudentListByDateOfBirth(dob: Date) {
    if (dob) {
      this.studentsService.getStudentsFilteredByDateOfBirth(dob).subscribe(
        data => {
          this.studentsUpdated.emit(data['students']);
          console.log('StudentAdvancedFilterComponent.filterStudentListByDateOfBirth(): students are ', data);
        },
        error => {
          console.log('StudentAdvancedFilterComponent.filterStudentListByDateOfBirth():  error is ', error);
        }
      );
    }
  }

  filterStudentListByIep(iep: boolean) {
    this.studentsService.getStudentsFilteredByIep(iep).subscribe(
      data => {
        this.studentsUpdated.emit(data['students']);
        console.log('StudentAdvancedFilterComponent.filterStudentListByIep(): students are ', data);
      },
      error => {
        console.log('StudentAdvancedFilterComponent.filterStudentListByIep():  error is ', error);
      }
    );
  }
}

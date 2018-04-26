import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

import { SchoolDistrict } from '../../../models/school-district.model';

import { StudentsService } from '../../../services/students.service';

import { Student } from '../../../models/student.model';

@Component({
  selector: 'app-student-advanced-filter',
  templateUrl: './student-advanced-filter.component.html',
  styleUrls: ['./student-advanced-filter.component.scss']
})
export class StudentAdvancedFilterComponent implements OnInit {

  private schoolYears: number[] = [
    2000, 2001, 2001, 2003, 2004, 2005, 2006, 2007, 2008, 2009, 2010, 2011, 2012, 2013, 2014, 2015, 2016, 2017, 2018
  ];
  private grades: number[] = [
    0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12
  ];

  @Input() schoolDistricts: SchoolDistrict[];
  constructor(private studentsService: StudentsService) { }

  @Output() studentsUpdated: EventEmitter<Student[]> = new EventEmitter();

  ngOnInit() {
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
        }
      );
    }
  }
}

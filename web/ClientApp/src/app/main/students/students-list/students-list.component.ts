import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { Observable } from 'rxjs/Observable';

import { Globals } from '../../../globals';

import { Student } from '../../../models/student.model';
import { SchoolDistrict } from '../../../models/school-district.model';

import { StudentsService } from '../../../services/students.service';
import { SchoolDistrictService } from '../../../services/school-district.service';
import { CurrentStudentService } from '../../../services/current-student.service';

@Component({
  selector: 'app-students-list',
  templateUrl: './students-list.component.html',
  styleUrls: ['./students-list.component.scss']
})
export class StudentsListComponent implements OnInit {

  public students: Student[];
  private schoolDistricts: SchoolDistrict[];
  public advancedSearchEnabled: boolean;
  public searchText: string;
  public isDescending: boolean;
  public property: string;
  public direction: number;
  public startDate: Date;
  public endDate: Date;
  private selectedStudent: Student;
  private skip: number;
  private retrievingStudents: boolean;
  public column: any;

  constructor(
    private globals: Globals,
    private studentsService: StudentsService,
    private schoolDistrictService: SchoolDistrictService,
    private currentStudentService: CurrentStudentService,
    private router: Router) {
      this.advancedSearchEnabled = false;
      this.isDescending = false;
      this.property = 'paCyberId';
      this.direction = 1;
      this.skip = 0;
      this.students = [];
    }

  ngOnInit() {
    this.retrievingStudents = true;
    this.studentsService.getStudents(this.skip).subscribe(
      data => {
        this.updateScrollingSkip();
        this.students = this.students.concat(data['students']);
        this.retrievingStudents = false;
        console.log('StudentsListComponent.ngOnInit():  students are ', this.students);
      },
      error => {
        this.retrievingStudents = false;
        console.log('StudentsListComponent.ngOnInit():  error is ', error);
      }
    );

    this.schoolDistrictService.getSchoolDistricts().subscribe(
      data => {
        this.schoolDistricts = data['schoolDistricts'];
        console.log('StudentsListComponent.ngOnInit():  school districts are', this.schoolDistricts);
      },
      error => {
        console.log('StudentsListComponent.ngOnInit():  error is ', error);
      }
    );

    this.currentStudentService.currentStudent.subscribe((student) => this.selectedStudent = student, (error) => error);
  }

  dateSelectedStartDateHandler(date: Date) {
    this.studentsService.getStudentsFilteredByStartDate(date).subscribe(
      data => {
        this.students = data['students'];
      },
      error => {
        console.log('error');
      }
    );
  }

  dateSelectedEndDateHandler(date: Date) {
    this.studentsService.getStudentsFilteredByEndDate(date).subscribe(
      data => {
        this.students = data['students'];
      },
      error => {
        console.log('error');
      }
    );
  }

  studentsUpdatedHandler(students: Student[]) {
    this.students = students;
    console.log('StudentsListComponent.studentsUpdatedHandler():  students are ', this.students);
  }

  getStudents($event) {
    this.studentsService.getStudents(this.skip).subscribe(
      data => {
        this.students = this.students.concat(data['students']);
        console.log('StudentsListComponent.getStudents():  students are ', this.students);
      },
      error => {
        console.log('error');
      }
    );
  }

  sort(property) {
    this.isDescending = !this.isDescending; // change the direction
    this.property = property;
    this.direction = this.isDescending ? 1 : -1;
  }

  showStudentDetails(studentId: number) {
    console.log('StudentsListComponent.showStudentDetails():  studentId is ', studentId);
    const s: Student = this.students.find((student) => student.id === studentId);
    this.currentStudentService.changeStudent(s);
    this.router.navigate(['/students', { id: studentId, outlets: {'action': [`${studentId}`]} }]);
  }

  toggleAdvancedSearchTools() {
    this.advancedSearchEnabled = !this.advancedSearchEnabled;
  }

  resetStudentList() {
    this.studentsService.getStudents(this.skip).subscribe(
      data => {
        this.students = data['students'];
      },
      error => {
        console.log('StudentsListComponent.resetStudentList(): error is ', error);
      }
    );
    this.searchText = '';
  }

  filterStudentListByNameOrId() {
    if (this.searchText) {
      this.studentsService.getStudentsFilteredByNameOrId(this.searchText).subscribe(
        data => {
          this.students = data['students'];
          console.log('StudentsListComponent.filterStudentList():  students are ', data);
        },
        error => {
          console.log('StudentsListComponent.filterStudentList():  error is ', error);
        }
      );
    }
  }

  onScroll($event) {
    if (!this.retrievingStudents) {
      this.getStudents($event);
    }
  }

  private updateScrollingSkip() {
    this.skip += this.globals.take;
  }
}

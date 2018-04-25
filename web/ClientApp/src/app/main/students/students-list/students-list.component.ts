import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { Observable } from 'rxjs/Observable';

import { Student } from '../../../models/student.model';
import { SchoolDistrict } from '../../../models/school-district.model';

import { StudentsService } from '../../../services/students.service';
import { SchoolDistrictService } from '../../../services/school-district.service';

@Component({
  selector: 'app-students-list',
  templateUrl: './students-list.component.html',
  styleUrls: ['./students-list.component.scss']
})
export class StudentsListComponent implements OnInit {

  private students: Student[];
  private schoolDistricts: SchoolDistrict[];
  private advancedSearchEnabled: boolean;
  private searchText: string;

  constructor(
    private studentsService: StudentsService,
    private schoolDistrictService: SchoolDistrictService,
    private router: Router) { this.advancedSearchEnabled = false; }

  ngOnInit() {
    this.studentsService.getStudents().subscribe(
      data => {
        this.students = data['students'];
        console.log('StudentsListComponent.ngOnInit():  students are ', this.students);
      }
    );

    this.schoolDistrictService.getSchoolDistricts().subscribe(
      data => {
        this.schoolDistricts = data['schoolDistricts'];
        console.log('StudentsListComponent.ngOnInit():  school districts are', this.schoolDistricts);
      }
    );
  }

  getStudents($event) {
   this.studentsService.getStudents().subscribe(
     data => {
       this.students = data['students'];
       console.log('StudentsListComponent.getStudents():  students are ', this.students);
     }
   );
  }

  onSorted($event) {
    this.getStudents($event);
  }

  showStudentDetails(studentId: number) {
    console.log('StudentsListComponent.showStudentDetails():  studentId is ', studentId);
    this.router.navigate(['/students', { id: studentId, outlets: {'action': [`${studentId}`]} }]);
  }

  toggleAdvancedSearchTools() {
    this.advancedSearchEnabled = !this.advancedSearchEnabled;
  }

  filterStudentList() {
    if (this.searchText) {
      this.studentsService.getFilteredStudents(this.searchText).subscribe(
        data => {
          this.students = data['students'];
          console.log('StudentsListComponent.filterStudentList():  students are ', data);
        }
      );
    }
  }
}

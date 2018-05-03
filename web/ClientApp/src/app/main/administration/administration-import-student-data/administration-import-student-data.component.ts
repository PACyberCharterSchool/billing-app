import { Component, OnInit } from '@angular/core';

import { Router } from '@angular/router';

import { Student } from '../../../models/student.model';

import { StudentsService } from '../../../services/students.service';

@Component({
  selector: 'app-administration-import-student-data',
  templateUrl: './administration-import-student-data.component.html',
  styleUrls: ['./administration-import-student-data.component.scss']
})
export class AdministrationImportStudentDataComponent implements OnInit {

  studentList: Student[];

  constructor(private studentsService: StudentsService, private router: Router) { }

  ngOnInit() {
    this.studentsService.getStudents().subscribe(
      data => {
        this.studentList = data['students'];
      }
    );
  }

  handleCancelClick() {
    this.router.navigate(['/administration', { outlets: { 'action': ['home'] } }]);
  }

  objectKeys(obj) {
    console.log('AdministrationImportStudentDataComponent.objectKeys(): obj is ', obj);
    if (obj) {
      return Object.keys(obj);
    }
  }

  objectValues(obj) {
    if (obj) {
      return Object.values(obj);
    }
  }
}

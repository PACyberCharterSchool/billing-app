import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { Observable } from 'rxjs/Observable';

import { Student } from '../../../models/student.model';

import { StudentsService } from '../../../services/students.service';

@Component({
  selector: 'app-students-list',
  templateUrl: './students-list.component.html',
  styleUrls: ['./students-list.component.scss']
})
export class StudentsListComponent implements OnInit {

  private students: Student[];

  constructor(private studentsService: StudentsService, private router: Router) { }

  ngOnInit() {
    this.studentsService.getStudents().subscribe(
      data => {
        this.students = data['students'];
        console.log('StudentsListComponent.ngOnInit():  students are ', this.students);
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
}

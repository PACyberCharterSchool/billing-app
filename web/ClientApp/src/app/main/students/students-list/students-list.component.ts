import { Component, OnInit } from '@angular/core';

import { Observable } from 'rxjs/Observable';

import { Student } from '../../../models/student.model';

import { StudentsService } from '../../../services/students.service';

@Component({
  selector: 'app-students-list',
  templateUrl: './students-list.component.html',
  styleUrls: ['./students-list.component.sass']
})
export class StudentsListComponent implements OnInit {

  private students: Student[];

  constructor(private studentsService: StudentsService) { }

  ngOnInit() {
    this.studentsService.getStudents().subscribe(
      data => {
        this.students = data;
      }
    );
  }

}

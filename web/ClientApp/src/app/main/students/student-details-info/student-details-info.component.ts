import { Component, Input, OnInit } from '@angular/core';

import { Student } from '../../../models/student.model';

import { CurrentStudentService } from '../../../services/current-student.service';

@Component({
  selector: 'app-student-details-info',
  templateUrl: './student-details-info.component.html',
  styleUrls: ['./student-details-info.component.scss']
})
export class StudentDetailsInfoComponent implements OnInit {

  public student: Student;

  constructor(private currentStudentService: CurrentStudentService) { }

  ngOnInit() {
    this.currentStudentService.currentStudent.subscribe((s) => this.student = s);
    console.log(`StudentDetailsInfoComponent.ngOnInit(): student is ${this.student}.`);
  }
}

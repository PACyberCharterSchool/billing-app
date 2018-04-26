import { Component, Input, OnInit } from '@angular/core';

import { Student } from '../../../models/student.model';

@Component({
  selector: 'app-student-details-info',
  templateUrl: './student-details-info.component.html',
  styleUrls: ['./student-details-info.component.scss']
})
export class StudentDetailsInfoComponent implements OnInit {

  @Input() studentInfo: Student;

  constructor() { }

  ngOnInit() {
  }

}

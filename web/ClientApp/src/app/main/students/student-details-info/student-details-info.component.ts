import { Component, Input, OnInit } from '@angular/core';

import { FormGroup, FormControl } from '@angular/forms';

import { StudentRecord } from '../../../models/student-record.model';

import { CurrentStudentService } from '../../../services/current-student.service';

@Component({
  selector: 'app-student-details-info',
  templateUrl: './student-details-info.component.html',
  styleUrls: ['./student-details-info.component.scss']
})
export class StudentDetailsInfoComponent implements OnInit {
  public student: StudentRecord;
  studentDetailForm = new FormGroup({
    personalInfo: new FormGroup({
      paSecuredId: new FormControl(''),
      firstName: new FormControl(''),
      lastName: new FormControl(''),
      middleInitial: new FormControl(''),
      dateOfBirth: new FormControl('')
    }),
    addressInfo: new FormGroup({
      street1: new FormControl(''),
      street2: new FormControl(''),
      city: new FormControl(''),
      state: new FormControl(''),
      zip: new FormControl('')
    }),
    studentInfo: new FormGroup({
      gradeLevel: new FormControl(''),
      enrollmentDate: new FormControl(''),
      withdrawalDate: new FormControl(''),
      spedStatus: new FormControl(''),
      currentIep: new FormControl(''),
      formerIep: new FormControl('')
    })
  });

  constructor(private currentStudentService: CurrentStudentService) { }

  ngOnInit() {
    this.currentStudentService.currentStudent.subscribe((s) => this.student = s);
    console.log('StudentDetailsInfoComponent.ngOnInit(): student is ', this.student);
  }
}

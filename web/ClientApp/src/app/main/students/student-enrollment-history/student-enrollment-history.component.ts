import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-student-enrollment-history',
  templateUrl: './student-enrollment-history.component.html',
  styleUrls: ['./student-enrollment-history.component.scss']
})
export class StudentEnrollmentHistoryComponent implements OnInit {
  private enrollments = [
    { 'School District': 'Mars', 'AUN #': '1234567', 'Date Enrolled': '05/10/2006', 'Date Withdrawn': '02/02/2008' },
    { 'School District': 'Penn Trafford', 'AUN #': '8675422', 'Date Enrolled': '05/13/2008', 'Date Withdrawn': '11/12/2013' },
    { 'School District': 'Seneca Valley', 'AUN #': '9999999', 'Date Enrolled': '11/13/2013', 'Date Withdrawn': '' }
  ];

  constructor() { }

  ngOnInit() {
  }

  objectKeys(obj) {
    return Object.keys(obj);
  }

  objectValues(obj) {
    return Object.values(obj);
  }
}

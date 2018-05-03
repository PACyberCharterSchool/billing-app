import { Component, Input, OnInit } from '@angular/core';

import { UtilitiesService } from '../../../services/utilities.service';
import { StudentsService } from '../../../services/students.service';
import { CurrentStudentService } from '../../../services/current-student.service';

import { Student } from '../../../models/student.model';
import { StudentActivityRecord } from '../../../models/student-activity-record.model';

const ACTIVITY_TYPES = [
  'Address Change',
  'Enrollment',
  'Withdrawal'
];

const activityList = [
  {
    'type': 'Address Change',
    'oldValue': '308 Negra Arroyo Lane, Wexford, PA 15090',
    'newValue': '369 Grubbs Road, Wexford, PA 15090',
    'date': '04/13/2017'
  },
  {
    'type': 'Withdrawal',
    'oldValue': 'Pine-Richland School District',
    'newValue': 'n/a',
    'date': '04/18/2017'
  },
  {
    'type': 'Enrollment',
    'oldValue': 'Pittston School District',
    'newValue': 'n/a',
    'date': '09/04/2017'
  },
  {
    'type': 'Enrollment',
    'oldValue': 'Wyomissing School District',
    'newValue': '',
    'date': '04/13/2018'
  },
  {
    'type': 'Address Change',
    'oldValue': '920 Pentland Ave, Pittsburgh, PA 15237',
    'newValue': '142 Georgetown Ave, Pittsburgh, PA 15235',
    'date': '04/13/2018'
  },
  {
    'type': 'Withdrawal',
    'oldValue': 'Wyomissing School District',
    'newValue': 'n/a',
    'date': '04/13/2018'
  },
];

@Component({
  selector: 'app-student-activity-history',
  templateUrl: './student-activity-history.component.html',
  styleUrls: ['./student-activity-history.component.scss']
})
export class StudentActivityHistoryComponent implements OnInit {
  private activityTypes = ACTIVITY_TYPES;
  private activities;
  private property: string;
  private direction: number;
  private isDescending: boolean;
  private student: Student;

  constructor(
    private utilities: UtilitiesService,
    private studentsService: StudentsService,
    private currentStudentService: CurrentStudentService) {
    this.property = 'date';
    this.direction = 1;
    this.activities = [];
  }

  ngOnInit() {
    this.currentStudentService.currentStudent.subscribe((s) => this.student = s);
    this.studentsService.getStudentActivityRecordsByStudentId(this.student.id).subscribe(
      data => {
        console.log(`StudentActivityHistoryComponent.ngOnInit(): data is ${data['studentActivityRecords']}.`);
        this.activities = data['studentActivityRecords'];
      }
    );
  }

  filterStudentHistoryByType(type: string) {
    this.activities = activityList.filter((e) => e['type'] === type);
  }

  resetStudentHistoryFilter() {
    this.activities = activityList;
  }

  sort(property) {
    this.isDescending = !this.isDescending; // change the direction
    this.property = property;
    this.direction = this.isDescending ? 1 : -1;
  }
}

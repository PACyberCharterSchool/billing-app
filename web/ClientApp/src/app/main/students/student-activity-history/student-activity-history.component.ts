import { Component, Input, OnInit } from '@angular/core';

import { UtilitiesService } from '../../../services/utilities.service';
import { StudentsService } from '../../../services/students.service';
import { CurrentStudentService } from '../../../services/current-student.service';

import { StudentRecord } from '../../../models/student-record.model';
import { StudentActivityRecord } from '../../../models/student-activity-record.model';

@Component({
  selector: 'app-student-activity-history',
  templateUrl: './student-activity-history.component.html',
  styleUrls: ['./student-activity-history.component.scss']
})
export class StudentActivityHistoryComponent implements OnInit {
  public activityTypes;
  public activities: StudentActivityRecord[];
  private allActivities: StudentActivityRecord[];
  public property: string;
  public direction: number;
  private isDescending: boolean;
  private student: StudentRecord;

  constructor(
    private utilitiesService: UtilitiesService,
    private studentsService: StudentsService,
    private currentStudentService: CurrentStudentService) {
    this.property = 'date';
    this.direction = 1;
    this.activities = this.allActivities = [];
  }

  ngOnInit() {
    this.currentStudentService.currentStudent.subscribe((s) => this.student = s, (e) => console.log('error'));
    this.studentsService.getStudentActivityRecordsByStudentId(+this.student.studentId).subscribe(
      data => {
        this.activities = this.allActivities = data['studentActivityRecords'];
        this.initActivityTypes();
      },
      error => {
        console.log('getStudentActivityRecordsByStudentId(): ', error);
      }
    );
  }

  initActivityTypes() {
    const atypes = this.allActivities.map((a) => a.activity);
    this.activityTypes = this.utilitiesService.uniqueItemsInArray(atypes);
  }

  listDisplayableFields() {
    const fields = this.utilitiesService.objectKeys(this.allActivities[0]);
    const rejected = ['batchHash', 'sequence', 'paCyberId'];

    if (fields) {
      return fields.filter((i) => !rejected.includes(i));
    }
  }

  listDisplayableValues(activity) {
    const vkeys = this.listDisplayableFields();
    const selected = this.utilitiesService.pick(activity, vkeys);

    return this.utilitiesService.objectValues(selected);
  }

  filterStudentHistoryByType(type: string) {
    this.activities = this.allActivities.filter(
      (a) => {
        const atype = a['activity'];
        return atype === type;
      }
    );
  }

  resetStudentHistoryFilter() {
    this.activities = this.allActivities;
  }

  sort(property) {
    this.isDescending = !this.isDescending; // change the direction
    this.property = property;
    this.direction = this.isDescending ? 1 : -1;
  }
}

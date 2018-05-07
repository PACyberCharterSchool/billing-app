import { Component, Input, OnInit } from '@angular/core';

import { UtilitiesService } from '../../../services/utilities.service';
import { StudentsService } from '../../../services/students.service';
import { CurrentStudentService } from '../../../services/current-student.service';

import { Student } from '../../../models/student.model';
import { StudentActivityRecord } from '../../../models/student-activity-record.model';

@Component({
  selector: 'app-student-activity-history',
  templateUrl: './student-activity-history.component.html',
  styleUrls: ['./student-activity-history.component.scss']
})
export class StudentActivityHistoryComponent implements OnInit {
  private activityTypes;
  private activities: StudentActivityRecord[];
  private allActivities: StudentActivityRecord[];
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
    this.activities = this.allActivities = [];
  }

  ngOnInit() {
    this.currentStudentService.currentStudent.subscribe((s) => this.student = s);
    this.studentsService.getStudentActivityRecordsByStudentId(this.student.paCyberId).subscribe(
      data => {
        this.activities = this.allActivities = data['studentActivityRecords'];
        console.log('StudentActivityHistoryComponent.ngOnInit(): data is ', data['studentActivityRecords']);
        this.initActivityTypes();
      }
    );
  }

  initActivityTypes() {
    const atypes = this.allActivities.map((a) => a.activity);
    this.activityTypes = this.utilities.uniqueItemsInArray(atypes);
    console.log(`StudentActivityHistoryComponent.initActivityTypes():  activityTypes are ${this.activityTypes}.`);
  }

  listDisplayableFields() {
    const fields = this.utilities.objectKeys(this.allActivities[0]);
    if (fields) {
      return fields.filter((i) => i !== 'batchHash' && i !== 'sequence');
    }
  }

  listDisplayableValues(activity) {
    const values = this.utilities.objectKeys(activity).map((i) => { if (i !== 'batchHash' && i !== 'sequence') { return activity[i]; } });
    return values;
  }

  filterStudentHistoryByType(type: string) {
    this.activities = this.allActivities.filter((e) => e['type'] === type);
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

import { Component, OnInit } from '@angular/core';

import { UtilitiesService } from '../../../services/utilities.service';

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
  private activities = activityList;
  private property: string;
  private direction: number;
  private isDescending: boolean;

  constructor(private utilities: UtilitiesService) {
    this.property = 'date';
    this.direction = 1;
  }

  ngOnInit() {
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
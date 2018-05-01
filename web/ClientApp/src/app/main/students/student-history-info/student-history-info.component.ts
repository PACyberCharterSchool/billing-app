import { Component, OnInit } from '@angular/core';

import { UtilitiesService } from '../../../services/utilities.service';

const ACTIVITY_TYPES = [
  'Address Change',
  'Enrollment',
  'Withdrawal'
];

@Component({
  selector: 'app-student-history-info',
  templateUrl: './student-history-info.component.html',
  styleUrls: ['./student-history-info.component.scss']
})
export class StudentHistoryInfoComponent implements OnInit {
  private activityTypes = ACTIVITY_TYPES;

  constructor(private utilities: UtilitiesService) { }

  ngOnInit() {
  }

  filterStudentHistoryByType(type: string) {
  }
}

import { Component, Input, OnInit } from '@angular/core';

import { UtilitiesService } from '../../../services/utilities.service';

import { StudentRecord } from '../../../models/student-record.model';

@Component({
  selector: 'app-student-history-info',
  templateUrl: './student-history-info.component.html',
  styleUrls: ['./student-history-info.component.scss']
})
export class StudentHistoryInfoComponent implements OnInit {
  constructor(private utilities: UtilitiesService) { }

  ngOnInit() {
  }

  filterStudentHistoryByType(type: string) {
  }
}

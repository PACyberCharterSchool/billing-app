import { Component, OnInit } from '@angular/core';

import { Router } from '@angular/router';

import { Student } from '../../../models/student.model';
import { PendingStudentStatusRecord } from '../../../models/pending-student-status-record.model';

import { UtilitiesService } from '../../../services/utilities.service';
import { StudentStatusRecordsImportService } from '../../../services/student-status-records-import.service';

import { Globals } from '../../../globals';

@Component({
  selector: 'app-administration-import-student-data',
  templateUrl: './administration-import-student-data.component.html',
  styleUrls: ['./administration-import-student-data.component.scss'],
})
export class AdministrationImportStudentDataComponent implements OnInit {

  studentStatusRecords: PendingStudentStatusRecord[] = [];
  private skip;

  constructor(
    private globals: Globals,
    private utilitiesService: UtilitiesService,
    private ssrImportService: StudentStatusRecordsImportService,
    private router: Router,
  ) { this.skip = 0; }

  ngOnInit() {
    this.ssrImportService.getPending(this.skip).subscribe(
      data => {
        this.updateScrollingSkip();
        this.studentStatusRecords = data['studentStatusRecords'];
        console.log('AdministrationImportStudentDataComponent.ngOnInit():  sample status record ', this.studentStatusRecords[0]);
      }
    );
  }

  handleCancelClick() {
    this.router.navigate(['/administration', { outlets: { 'action': ['home'] } }]);
  }

  handleDataImportCommitClick() {
  }

  getPending($event) {
    this.ssrImportService.getPending(this.skip).subscribe(
      data => {
        this.updateScrollingSkip();
        this.studentStatusRecords.concat(data['studentStatusRecords']);
        console.log('AdministrationImportStudentDataComponent.ngOnInit():  sample status record ', this.studentStatusRecords[0]);
      }
    );
  }

  onScroll($event) {
    this.getPending($event);
  }

  private updateScrollingSkip() {
    this.skip += this.globals.take;
  }
}

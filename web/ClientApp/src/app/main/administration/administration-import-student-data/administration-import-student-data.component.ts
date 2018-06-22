import { Component, OnInit } from '@angular/core';

import { Router } from '@angular/router';

import { Student } from '../../../models/student.model';
import { PendingStudentStatusRecord } from '../../../models/pending-student-status-record.model';

import { UtilitiesService } from '../../../services/utilities.service';
import { StudentStatusRecordsImportService } from '../../../services/student-status-records-import.service';

import { Globals } from '../../../globals';

import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-administration-import-student-data',
  templateUrl: './administration-import-student-data.component.html',
  styleUrls: ['./administration-import-student-data.component.scss'],
})
export class AdministrationImportStudentDataComponent implements OnInit {

  pendingStudentStatusRecords: PendingStudentStatusRecord[] = [];
  private skip;
  lastUpdatedDate: string;

  constructor(
    private globals: Globals,
    private utilitiesService: UtilitiesService,
    private ssrImportService: StudentStatusRecordsImportService,
    private router: Router,
    private spinnerService: NgxSpinnerService
  ) {
    this.skip = 0;
  }

  ngOnInit() {
    this.ssrImportService.getPending(this.skip).subscribe(
      data => {
        this.updateScrollingSkip();
        this.pendingStudentStatusRecords = data['studentStatusRecords'];
        console.log('AdministrationImportStudentDataComponent.ngOnInit():  sample status record ', this.pendingStudentStatusRecords[0]);
        if (this.pendingStudentStatusRecords.length > 0) {
          this.lastUpdatedDate = this.pendingStudentStatusRecords[0].batchTime;
        } else {
          this.lastUpdatedDate = '?';
        }
      },
      error => {
      }
    );
    this.spinnerService.hide();
  }

  handleCancelClick() {
    this.router.navigate(['/administration', { outlets: { 'action': ['home'] } }]);
  }

  handleDataImportCommitClick() {
    this.spinnerService.show();
    this.ssrImportService.postStudentData().subscribe(
      response => {
        this.spinnerService.hide();
        this.lastUpdatedDate = this.pendingStudentStatusRecords[0].batchTime;
        this.pendingStudentStatusRecords = [];
      },
      error => {
        this.spinnerService.hide();
      }
    );
  }

  getPending($event) {
    this.ssrImportService.getPending(this.skip).subscribe(
      data => {
        this.updateScrollingSkip();
        this.pendingStudentStatusRecords = this.pendingStudentStatusRecords.concat(data['studentStatusRecords']);
      },
      error => {
      }
    );
  }

  onScroll($event) {
    this.getPending($event);
  }

  private updateScrollingSkip() {
    this.skip += this.globals.take;
  }

  listDisplayableFields() {
    const fields = this.utilitiesService.objectKeys(this.pendingStudentStatusRecords[0]);
    const rejected = [
      'batchHash',
      'batchTime',
      'batchFilename',
      'id',
      'lazyLoader',
      'studentMiddleInitial'
    ];

    if (fields) {
      const filtered = fields.filter((i) => !rejected.includes(i));

      return filtered;
    }
  }

  listDisplayableValues(activity) {
    const vkeys = this.listDisplayableFields();
    const selected = this.utilitiesService.pick(activity, vkeys);

    return this.utilitiesService.objectValues(selected);
  }
}

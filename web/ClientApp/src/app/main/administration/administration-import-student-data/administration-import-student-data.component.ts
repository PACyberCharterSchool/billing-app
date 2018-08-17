import { Component, OnInit } from '@angular/core';

import { Router } from '@angular/router';

import { StudentRecord, StudentRecordsHeader } from '../../../models/student-record.model';

import { UtilitiesService } from '../../../services/utilities.service';
import { StudentRecordsService } from '../../../services/student-records.service';

import { Globals } from '../../../globals';

import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-administration-import-student-data',
  templateUrl: './administration-import-student-data.component.html',
  styleUrls: ['./administration-import-student-data.component.scss'],
})
export class AdministrationImportStudentDataComponent implements OnInit {

  public studentRecords: StudentRecord[] = [];
  public scopes: string[];
  public currentScope: string;
  private skip;
  public canCommit: boolean;

  constructor(
    private globals: Globals,
    private utilitiesService: UtilitiesService,
    private ssrImportService: StudentRecordsService,
    private router: Router,
    private spinnerService: NgxSpinnerService
  ) {
    this.skip = 0;
    this.canCommit = true;
  }

  ngOnInit() {
    this.currentScope = 'Select billing period...';
    this.ssrImportService.getStudentRecordsHeaders().subscribe(
      data => {
        this.scopes = data['scopes'];
      },
      error => {
        console.log('AdministrationImportStudentDataComponent.ngOnInit():  error is ', error);
      }
    );

    this.spinnerService.hide();
  }

  public handleCancelClick(): void {
    this.router.navigate(['/administration', { outlets: { 'action': ['home'] } }]);
  }

  public handleCommitClick(): void {
    this.spinnerService.show();
    this.ssrImportService.postLockStudentData(this.currentScope).subscribe(
      response => {
        this.spinnerService.hide();
      },
      error => {
        this.spinnerService.hide();
      }
    );
  }

  public filterByStudentRecordScope(scope: string): void {
    this.currentScope = scope;
    this.spinnerService.show();
    this.ssrImportService.getStudentRecordsHeaderByScope(this.currentScope, this.skip).subscribe(
      data => {
        this.studentRecords = data['header']['records'];
        console.log('AdministrationImportStudentDataComponent.ngOnInit():  data is ', data);
        this.spinnerService.hide();
        this.canCommit = !data['header']['locked'];
      },
      error => {
        console.log('AdministrationImportStudentDataComponent.ngOnInit():  error is ', error);
        this.spinnerService.hide();
      }
    );
  }

  getStudentRecords($event) {
    this.ssrImportService.getStudentRecordsHeaderByScope(this.currentScope, this.skip).subscribe(
      data => {
        this.updateScrollingSkip();
        this.studentRecords = this.studentRecords.concat(data['header']['records']);
      },
      error => {
      }
    );
  }

  onScroll($event) {
    this.getStudentRecords($event);
  }

  private updateScrollingSkip() {
    this.skip += this.globals.take;
  }

  listDisplayableFields() {
    const fields = this.utilitiesService.objectKeys(this.studentRecords[0]);
    const rejected = [
      'id',
      'studentMiddleInitial',
      'studentState',
      'studentNorep',
      'lastUpdated',
      'lazyLoader'
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

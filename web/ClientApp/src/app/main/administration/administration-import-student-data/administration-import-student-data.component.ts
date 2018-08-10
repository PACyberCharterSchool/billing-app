import { Component, OnInit } from '@angular/core';

import { Router } from '@angular/router';

import { StudentRecord, StudentRecordsHeader } from '../../../models/student-record.model';

import { UtilitiesService } from '../../../services/utilities.service';
import { StudentRecordsImportService } from '../../../services/student-records-import.service';

import { Globals } from '../../../globals';

import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-administration-import-student-data',
  templateUrl: './administration-import-student-data.component.html',
  styleUrls: ['./administration-import-student-data.component.scss'],
})
export class AdministrationImportStudentDataComponent implements OnInit {

  studentRecords: StudentRecord[] = [];
  scopes: string[];
  currentScope: string;
  private skip;

  constructor(
    private globals: Globals,
    private utilitiesService: UtilitiesService,
    private ssrImportService: StudentRecordsImportService,
    private router: Router,
    private spinnerService: NgxSpinnerService
  ) {
    this.skip = 0;
  }

  ngOnInit() {
    this.currentScope = 'Select scope...';
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
    this.ssrImportService.postStudentData().subscribe(
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
    this.ssrImportService.getStudentRecordsHeaderByScope(this.currentScope).subscribe(
      data => {
        this.studentRecords = data['header']['records'];
        console.log('AdministrationImportStudentDataComponent.ngOnInit():  data is ', data);
        this.spinnerService.hide();
      },
      error => {
        console.log('AdministrationImportStudentDataComponent.ngOnInit():  error is ', error);
        this.spinnerService.hide();
      }
    );
  }

  getStudentRecords($event) {
    this.ssrImportService.getStudentRecordsHeaderByScope(this.currentScope).subscribe(
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
      'lastUpdated'
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

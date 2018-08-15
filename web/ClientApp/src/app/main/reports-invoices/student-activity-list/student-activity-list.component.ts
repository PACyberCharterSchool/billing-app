import { Component, OnInit } from '@angular/core';

import { Report } from '../../../models/report.model';

import { UtilitiesService } from '../../../services/utilities.service';
import { ReportsService } from '../../../services/reports.service';

import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-student-activity-list',
  templateUrl: './student-activity-list.component.html',
  styleUrls: ['./student-activity-list.component.scss']
})
export class StudentActivityListComponent implements OnInit {
  private reports: Report[];
  private allReports: Report[];
  public bulkReports: Report[];
  private allBulkReports: Report[];
  private skip: number;
  public property: string;
  public direction: number;
  private isDescending: boolean;
  public searchText: string;
  public selectedScope: string;
  public selectedCreateSchoolYear: string;
  public spinnerMsg: string;

  constructor(
    private utilitiesService: UtilitiesService,
    private ngbModal: NgbModal,
    private reportsService: ReportsService,
    private ngxSpinnerService: NgxSpinnerService
  ) { }

  ngOnInit() {
    this.spinnerMsg = 'Loading bulk student activity reports.  Please wait...';
  }

  sort(property) {
    this.isDescending = !this.isDescending; // change the direction
    this.property = property;
    this.direction = this.isDescending ? 1 : -1;
  }

  listDisplayableFields() {
    if (this.allBulkReports) {
      const fields = this.utilitiesService.objectKeys(this.allBulkReports[0]);
      const rejected = ['data', 'xlsx', 'type', 'id'];
      return fields.filter((i) => !rejected.includes(i));
    }
  }

  listDisplayableValues(report: Report) {
    const vkeys = this.listDisplayableFields();
    const selected = this.utilitiesService.pick(report, vkeys);

    return this.utilitiesService.objectValues(selected);
  }

  refreshActivityReports(): void {
  }

  create(): void {
    // this.reportsService.createBulkActivity(
    //   {
    //     'schoolYear': this.selectedCreateSchoolYear,
    //     'name': this.generateBulkActivityName(this.selectedCreateSchoolYear, this.selectedScope),
    //     'bulkInvoice': {
    //       'asOf': this.selectedAsOfBillingDate,
    //       'toSchoolDistrict': this.selectedAsOfBillingDate,
    //       'toPDE': this.selectedAsOfBillingDate
    //     }
    //   }
    // ).subscribe(
    //   data => {
    //     console.log('InvoicesMonthlyCombinedListComponent.create(): data is ', data['reports']);
    //     this.ngxSpinnerService.hide();
    //     this.refreshActivityReports();
    //   },
    //   error => {
    //     console.log('InvoicesMonthlyCombinedListComponent.create(): error is ', error);
    //     this.ngxSpinnerService.hide();
    //   }
    // );
  }

  displayCreateBulkActivityDialog(bulkCreateContent): void {
    const modal = this.ngbModal.open(bulkCreateContent, { centered: true, size: 'sm' });
    modal.result.then(
      (result) => {
        console.log('InvoicesListComponent.createBulkInvoice(): result is ', result);
        this.ngxSpinnerService.show();
      },
      (reason) => {
        console.log('InvoicesListComponent.createBulkInvoice(): reason is ', reason);
      }
    );
  }
}

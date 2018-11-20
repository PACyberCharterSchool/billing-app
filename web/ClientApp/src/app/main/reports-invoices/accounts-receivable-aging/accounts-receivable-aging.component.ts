import { Component, OnInit } from '@angular/core';

import { Report } from '../../../models/report.model';

import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NgxSpinnerService } from 'ngx-spinner';

import { UtilitiesService } from '../../../services/utilities.service';
import { ReportsService } from '../../../services/reports.service';
import { FileSaverService } from '../../../services/file-saver.service';
import { AcademicYearsService } from '../../../services/academic-years.service';
import * as moment from 'moment';
import { Globals } from '../../../globals';

@Component({
  selector: 'app-accounts-receivable-aging',
  templateUrl: './accounts-receivable-aging.component.html',
  styleUrls: ['./accounts-receivable-aging.component.scss']
})
export class AccountsReceivableAgingComponent implements OnInit {
  public reports: Report[];
  public allReports: Report[];
  private skip: number;
  public property: string;
  public direction: number;
  private isDescending: boolean;
  public searchText: string;
  public fromDate;
  public asOfDate;
  public selectedDownloadFormat: string;
  public downloadFormats: string[] = [
    'Microsoft Excel',
    'PDF'
  ];
  public spinnerMsg: string;

  constructor(
    private globals: Globals,
    private ngbModalService: NgbModal,
    private ngxSpinnerService: NgxSpinnerService,
    private utilitiesService: UtilitiesService,
    private reportsService: ReportsService,
    private fileSaverService: FileSaverService,
    private academicYearsService: AcademicYearsService
  ) { }

  ngOnInit() {
    this.skip = 0;
    this.spinnerMsg = '';
    this.refreshAccountsReceivableAgingList();
  }

  public refreshAccountsReceivableAgingList(): void {
    this.reportsService.getAccountsReceivableAging().subscribe(
      data => {
        this.allReports = this.reports = data['reports'];
        console.log('AccountsReceivableAgingComponent.ngOnInit():  reports are ', data['reports']);
      },
      error => {
        console.log('AccountsReceivableAgingomponent.ngOnInit():  error is ', error);
      }
    );
  }

  sort(property) {
    this.isDescending = !this.isDescending; // change the direction
    this.property = property;
    this.direction = this.isDescending ? 1 : -1;
  }

  public filterAccountsReceivableAgingBySearch(): void {
    if (this.searchText) {
      this.reports = this.allReports.filter(
        (i) => {
          const re = new RegExp(this.searchText, 'gi');
          if (
            i.name.search(re) !== -1
          ) {
            return true;
          }
          return false;
        }
      );

    }
  }

  listDisplayableFields() {
    if (this.allReports) {
      const fields = this.utilitiesService.objectKeys(this.allReports[0]);
      const rejected = ['schoolYear', 'data', 'xlsx', 'type', 'id', 'approved', 'scope'];
      return fields.filter((i) => !rejected.includes(i));
    }
  }

  listDisplayableValues(report: Report) {
    const vkeys = this.listDisplayableFields();
    const selected = this.utilitiesService.pick(report, vkeys);

    return this.utilitiesService.objectValues(selected);
  }

  public displayCreateAccountsReceivableAgingDialog(createContent): void {
    const modal = this.ngbModalService.open(createContent, { centered: true });
    modal.result.then(
      (result) => {
      },
      (reason) => {
      }
    );
  }

  public displayDownloadFormatTypeDialog(downloadContent): void {
    const modal = this.ngbModalService.open(downloadContent, { centered: true });
    modal.result.then(
      (result) => {
      },
      (reason) => {
      }
    );
  }

  private generateAccountsReceivableAgingReportName(): string {
    const parts = new Array<string>();
    parts.push('Aging');

    if (this.fromDate) {
      parts.push(`${this.fromDate.year}${this.fromDate.month}${this.fromDate.day}`);
    }

    if (this.asOfDate) {
      parts.push(`${this.asOfDate.year}${this.asOfDate.month}${this.asOfDate.day}`);
    }

    parts.push(`${moment().format(this.globals.dateFormat)}`);

    return parts.join('_');
    // let name = 'Aging';

    // if (this.fromDate) {
    //   name += `_${this.fromDate.year}${this.fromDate.month}${this.fromDate.day}`;
    // }

    // if (this.asOfDate) {
    //   name += `_${this.asOfDate.year}${this.asOfDate.month}${this.asOfDate.day}`;
    // }

    // name += `_${moment().format(this.globals.dateFormat)}`;

    // return name;
  }

  public onCreateSubmit(): void {
    let from: Date;
    if (this.fromDate) {
      from = new Date(this.fromDate.year, this.fromDate.month - 1, this.fromDate.day);
    }

    let asOf: Date;
    if (this.asOfDate) {
      asOf = new Date(this.asOfDate.year, this.asOfDate.month - 1, this.asOfDate.day);
    }

    this.ngxSpinnerService.show();
    this.spinnerMsg = 'Generating accounts receivable as of report.  Please wait...';
    this.reportsService.createAccountsReceivableAging(
      this.generateAccountsReceivableAgingReportName(),
      from,
      asOf).subscribe(
        data => {
          this.ngxSpinnerService.hide();
          this.refreshAccountsReceivableAgingList();
        },
        error => {
          this.ngxSpinnerService.hide();
          this.refreshAccountsReceivableAgingList();
        }
      );
  }

  public downloadReportByFormat(report: Report, format: string): void {
    this.ngxSpinnerService.show();
    this.reportsService.getReportDataByFormat(report, format.includes('Microsoft Excel') ? 'excel' : 'pdf').subscribe(
      data => {
        console.log('AccountsReceivableAgingComponent.downloadReportByFormat():  data is ', data);
        this.ngxSpinnerService.hide();
        if (format.toLowerCase().includes('excel')) {
          this.fileSaverService.saveInvoiceAsExcelFile(data, report);
        } else {
          this.fileSaverService.saveInvoiceAsPDFFile(data, report);
        }
      },
      error => {
        this.ngxSpinnerService.hide();
        console.log('AccountsReceivableAgingComponent.downloadReportByFormat():  error is ', error);
      }
    );
  }

  public displayDownloadAccountsReceivableAgingFormatDialog(downloadReportTypeContent, report: Report): void {
    const modal = this.ngbModalService.open(downloadReportTypeContent, { centered: true });
    modal.result.then(
      (result) => {
        console.log('AccountsReceivableAgingComponent.displayDownloadAccountsReceivableAgingFormatDialog():  download');
        this.downloadReportByFormat(report, this.selectedDownloadFormat);
      },
      (reason) => {
      }
    );
  }

  public onDateChanged() {
  }

  public setSelectedDownloadFormat(format: string): void {
    this.selectedDownloadFormat = format;
  }


}

import { Component, OnInit } from '@angular/core';

import { Report } from '../../../models/report.model';

import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NgxSpinnerService } from 'ngx-spinner';

import { UtilitiesService } from '../../../services/utilities.service';
import { ReportsService } from '../../../services/reports.service';
import { FileSaverService } from '../../../services/file-saver.service';
import { AcademicYearsService } from '../../../services/academic-years.service';
import { PassThrough } from 'stream';
import * as moment from 'moment';
import { Globals } from '../../../globals';

@Component({
  selector: 'app-unipay-invoice-summary',
  templateUrl: './unipay-invoice-summary.component.html',
  styleUrls: ['./unipay-invoice-summary.component.scss']
})
export class UnipayInvoiceSummaryComponent implements OnInit {
  public reports: Report[];
  public allReports: Report[];
  private skip: number;
  public property: string;
  public direction: number;
  private isDescending: boolean;
  public searchText: string;
  public asOfDate;
  public selectedAcademicYear: string;
  public schoolYears: string[];
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
    this.refreshUniPayInvoiceSummaryList();
    this.schoolYears = this.academicYearsService.getAcademicYears();
  }

  public refreshUniPayInvoiceSummaryList(): void {
    this.reportsService.getUniPayInvoiceSummary().subscribe(
      data => {
        this.allReports = this.reports = data['reports'];
        console.log('UnipayInvoiceSummaryComponent.ngOnInit():  reports are ', data['reports']);
      },
      error => {
        console.log('UnipayInvoiceSummaryComponent.ngOnInit():  error is ', error);
      }
    );
  }

  sort(property) {
    this.isDescending = !this.isDescending; // change the direction
    this.property = property;
    this.direction = this.isDescending ? 1 : -1;
  }

  public filterUniPayInvoiceSummaryBySearch(): void {
    if (this.searchText) {
      this.reports = this.allReports.filter(
        (i) => {
          const re = new RegExp(this.searchText, 'gi');
          if (
            i.name.search(re) !== -1 ||
            i.schoolYear.search(re) !== -1
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
      const rejected = ['data', 'xlsx', 'type', 'id', 'approved', 'scope'];
      return fields.filter((i) => !rejected.includes(i));
    }
  }

  listDisplayableValues(report: Report) {
    const vkeys = this.listDisplayableFields();
    const selected = this.utilitiesService.pick(report, vkeys);

    return this.utilitiesService.objectValues(selected);
  }

  public displayCreateUniPayInvoiceSummaryDialog(createContent): void {
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

  private generateUniPayInvoiceSummaryReportName(): string {
    const parts = new Array<string>();
    parts.push('UniPayInvoiceSummary');

    parts.push(this.selectedAcademicYear);
    parts.push(`${this.asOfDate.year}${this.asOfDate.month}${this.asOfDate.day}`);
    parts.push(moment().format(this.globals.dateFormat));

    return parts.join('_');
  }

  public onCreateSubmit(): void {
    this.ngxSpinnerService.show();
    this.spinnerMsg = 'Generating UniPay invoice summary report.  Please wait...';
    this.reportsService.createUniPayInvoiceSummary(
      this.generateUniPayInvoiceSummaryReportName(),
      this.selectedAcademicYear,
      new Date(this.asOfDate.year, this.asOfDate.month - 1, this.asOfDate.day)).subscribe(
        data => {
          this.ngxSpinnerService.hide();
          this.refreshUniPayInvoiceSummaryList();
        },
        error => {
          this.ngxSpinnerService.hide();
          this.refreshUniPayInvoiceSummaryList();
        }
      );
  }

  public downloadReportByFormat(report: Report, format: string): void {
    this.ngxSpinnerService.show();
    this.spinnerMsg = 'Generating UniPay invoice report format.  Please wait...';
    this.reportsService.getReportDataByFormat(report, format.includes('Microsoft Excel') ? 'excel' : 'pdf').subscribe(
      data => {
        console.log('UnipayInvoiceSummaryComponent.downloadReportByFormat():  data is ', data);
        this.ngxSpinnerService.hide();
        if (format.toLowerCase().includes('excel')) {
          this.fileSaverService.saveInvoiceAsExcelFile(data, report);
        } else {
          this.fileSaverService.saveInvoiceAsPDFFile(data, report);
        }
      },
      error => {
        this.ngxSpinnerService.hide();
        console.log('UnipayInvoiceSummaryComponent.downloadReportByFormat():  error is ', error);
      }
    );
  }

  public setSelectedAcademicYear(year: string) {
    this.selectedAcademicYear = year;
  }

  public displayDownloadUniPayInvoiceSummaryFormatDialog(downloadReportTypeContent, report: Report): void {
    const modal = this.ngbModalService.open(downloadReportTypeContent, { centered: true });
    modal.result.then(
      (result) => {
        console.log('UnipayInvoiceSummaryComponent.displayDownloadAccountsReceivableAsOfFormatDialog():  download');
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

import { Component, OnInit } from '@angular/core';

import { Report } from '../../../models/report.model';

import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NgxSpinnerService } from 'ngx-spinner';

import { UtilitiesService } from '../../../services/utilities.service';
import { ReportsService } from '../../../services/reports.service';
import { FileSaverService } from '../../../services/file-saver.service';
import { AcademicYearsService } from '../../../services/academic-years.service';

@Component({
  selector: 'app-accounts-receivable-as-of',
  templateUrl: './accounts-receivable-as-of.component.html',
  styleUrls: ['./accounts-receivable-as-of.component.scss']
})
export class AccountsReceivableAsOfComponent implements OnInit {
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
    this.refreshAccountsReceivableAsOfList();
    this.schoolYears = this.academicYearsService.getAcademicYears();
  }

  public refreshAccountsReceivableAsOfList(): void {
    this.reportsService.getAccountsReceivableAsOf().subscribe(
      data => {
        this.allReports = this.reports = data['reports'];
        console.log('AccountsReceivableAsOfComponent.ngOnInit():  reports are ', data['reports']);
      },
      error => {
        console.log('AccountsReceivableAsOfComponent.ngOnInit():  error is ', error);
      }
    );
  }

  sort(property) {
    this.isDescending = !this.isDescending; // change the direction
    this.property = property;
    this.direction = this.isDescending ? 1 : -1;
  }

  public filterAccountsReceivableAsOfBySearch(): void {
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

  public displayCreateAccountsReceivableAsOfDialog(createContent): void {
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

  private generateAccountsReceivableAsOfReportName(): string {
    return 'AccountsReceivableAsOf_' +
      this.selectedAcademicYear.replace(/\s+/g, '') + '_' + `${this.asOfDate.month}-${this.asOfDate.day}-${this.asOfDate.year}`;
  }

  public onCreateSubmit(): void {
    this.ngxSpinnerService.show();
    this.spinnerMsg = 'Generating accounts receivable as of report.  Please wait...';
    this.reportsService.createAccountsReceivableAsOf(
      this.generateAccountsReceivableAsOfReportName(),
      this.selectedAcademicYear,
      new Date(this.asOfDate.year, this.asOfDate.month - 1, this.asOfDate.day)).subscribe(
        data => {
          this.ngxSpinnerService.hide();
          this.refreshAccountsReceivableAsOfList();
        },
        error => {
          this.ngxSpinnerService.hide();
          this.refreshAccountsReceivableAsOfList();
        }
      );
  }

  public downloadReportByFormat(report: Report, format: string): void {
    this.ngxSpinnerService.show();
    this.reportsService.getReportDataByFormat(report, format.includes('Microsoft Excel') ? 'excel' : 'pdf').subscribe(
      data => {
        console.log('AccountsReceivableAsOfComponent.downloadReportByFormat():  data is ', data);
        this.ngxSpinnerService.hide();
        if (format.toLowerCase().includes('excel')) {
          this.fileSaverService.saveInvoiceAsExcelFile(data, report);
        } else {
          this.fileSaverService.saveInvoiceAsPDFFile(data, report);
        }
      },
      error => {
        this.ngxSpinnerService.hide();
        console.log('AccountsReceivableAsOfComponent.downloadReportByFormat():  error is ', error);
      }
    );
  }

  public setSelectedAcademicYear(year: string) {
    this.selectedAcademicYear = year;
  }

  public displayDownloadAccountsReceivableAsOfFormatDialog(downloadReportTypeContent, report: Report): void {
    const modal = this.ngbModalService.open(downloadReportTypeContent, { centered: true });
    modal.result.then(
      (result) => {
        console.log('AccountsReceivableAsOfComponent.displayDownloadAccountsReceivableAsOfFormatDialog():  download');
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

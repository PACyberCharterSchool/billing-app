import { Component, OnInit } from '@angular/core';

import { Report, ReportType } from '../../../models/report.model';
import { Template } from '../../../models/template.model';

import { Globals } from '../../../globals';
import { ReportsService } from '../../../services/reports.service';
import { UtilitiesService } from '../../../services/utilities.service';
import { FileSaverService } from '../../../services/file-saver.service';
import { TemplatesService } from '../../../services/templates.service';
import { AcademicYearsService } from '../../../services/academic-years.service';
import { StudentRecordsService } from '../../../services/student-records.service';

import { InvoiceCreateFormComponent } from '../invoice-create-form/invoice-create-form.component';

import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-invoices-monthly-combined-list',
  templateUrl: './invoices-monthly-combined-list.component.html',
  styleUrls: ['./invoices-monthly-combined-list.component.scss']
})
export class InvoicesMonthlyCombinedListComponent implements OnInit {
  private reports: Report[];
  private allReports: Report[];
  public bulkReports: Report[];
  private allBulkReports: Report[];
  private skip: number;
  public property: string;
  public direction: number;
  private isDescending: boolean;
  public searchText: string;
  public statuses: string[] = [
    'Approved',
    'Disapproved'
  ];
  public downloadFormats: string[] = [
    'Microsoft Excel',
    'PDF'
  ];
  private selectedCreateSchoolYear: string;
  private selectedDownloadSchoolYear: string;
  private selectedDownloadStatus: string;
  private selectedTemplate: Template;
  private selectedTemplateName: string;
  public selectedFilterSchoolYear: string;
  public selectedFilterStatus: string;
  private downloadType: string;
  private spinnerMsg: string;
  private templates: Template[];
  public selectedScope: string;
  public scopes: string[];
  public selectedAsOfBillingDate: string;
  public selectedDownloadFormat: string;

  constructor(
    private globals: Globals,
    private reportsService: ReportsService,
    private utilitiesService: UtilitiesService,
    private fileSaverService: FileSaverService,
    private templatesService: TemplatesService,
    private ngxSpinnerService: NgxSpinnerService,
    private academicYearsService: AcademicYearsService,
    private studentRecordsService: StudentRecordsService,
    private ngbModal: NgbModal,
    private ngbActiveModal: NgbActiveModal
  ) { }

  ngOnInit() {
    this.selectedTemplateName = 'Select Bulk Invoice Template';
    this.selectedCreateSchoolYear = 'Select Academic Year';
    this.selectedScope = 'Select billing period';
    this.spinnerMsg = 'Loading bulk invoices.  Please wait...';
    this.ngxSpinnerService.show();
    this.reportsService.getBulkInvoices(null).subscribe(
      data => {
        console.log('InvoicesListComponent.ngOnInit(): invoices are ', data['reports']);
        this.bulkReports = this.allBulkReports = data['reports'];
        this.ngxSpinnerService.hide();
      },
      error => {
        console.log('InvoicesListComponent.ngOnInit(): error is ', error);
        this.ngxSpinnerService.hide();
      }
    );

    this.reportsService.getInvoices(null, null, null, null).subscribe(
      data => {
        console.log('InvoicesListComponent.ngOnInit(): invoices are ', data['reports']);
        this.reports = this.allReports = data['reports'];
      },
      error => {
        console.log('InvoicesListComponent.ngOnInit(): error is ', error);
      }
    );

    this.templatesService.getTemplates(this.skip).subscribe(
      data => {
        console.log(`InvoicesListComponent.ngOnInit(): data is ${data}.`);
        this.templates = data['templates'];
      },
      error => {
        console.log(`InvoicesListComponent.ngOnInit(): error is ${error}.`);
      }
    );

    this.studentRecordsService.getStudentRecordsHeaders().subscribe(
      data => {
        console.log(`InvoicesListComponent.ngOnInit(): data is ${data}.`);
        this.scopes = data['scopes'];
      },
      error => {
        console.log(`InvoicesListComponent.ngOnInit(): error is ${error}.`);
      }
    );
    this.selectedFilterSchoolYear = 'School Year';
    this.selectedFilterStatus = 'Status';
  }

  sort(property) {
    this.isDescending = !this.isDescending; // change the direction
    this.property = property;
    this.direction = this.isDescending ? 1 : -1;
  }

  filterInvoices() {
    this.bulkReports = this.allBulkReports.filter(
      (i) => {
        const re = new RegExp(this.searchText, 'gi');
        if (
          i.name.toString().search(re) !== -1 ||
          i.schoolYear.search(re) !== -1
        ) {
          return true;
        }
        return false;
      }
    );
  }

  resetInvoices() {
    this.bulkReports = this.allBulkReports;
    this.searchText = '';
  }

  refreshInvoices(): void {
    this.spinnerMsg = 'Loading bulk invoices.  Please wait...';
    this.ngxSpinnerService.show();
    this.reportsService.getReportsByInfo({
      'Type': ReportType.Invoice,
      'Name': '', 'Approved': null,
      'SchoolYear': null}).subscribe(
        data => {
          console.log(`InvoicesListComponent.refreshInvoices(): data is ${data}.`);
          this.ngxSpinnerService.hide();
          this.bulkReports = this.allBulkReports = data['reports'];
        },
        error => {
          this.ngxSpinnerService.hide();
          console.log(`InvoicesListComponent.refreshInvoices(): error is ${error}.`);
        }
      );
  }

  setSelectedDownloadFormat(format: string): void {
    this.selectedDownloadFormat = format;
  }

  listDisplayableFields() {
    if (this.allBulkReports) {
      const fields = this.utilitiesService.objectKeys(this.allBulkReports[0]);
      const rejected = ['data', 'xlsx', 'type', 'id', 'pdf'];
      return fields.filter((i) => !rejected.includes(i));
    }
  }

  listDisplayableValues(report: Report) {
    const vkeys = this.listDisplayableFields();
    const selected = this.utilitiesService.pick(report, vkeys);

    return this.utilitiesService.objectValues(selected);
  }

  filterBySchoolYear(year: string) {
    this.selectedFilterSchoolYear = year;
    this.bulkReports = this.allBulkReports.filter((r) => (r.type === ReportType.Invoice && r.schoolYear === year));
  }

  filterByApprovedStatus(status: string) {
    this.selectedFilterStatus = status;
    this.bulkReports = this.allBulkReports.filter((r) => {
      if (r.type === ReportType.Invoice &&
        r.approved === (status === 'Approved')) {
          return true;
        }
        return false;
    });
  }

  selectScope(scope: string): void {
    this.selectedScope = scope;
  }

  getInvoiceBillingMonths(): string[] {
    if (this.allReports) {
      const years = this.allReports.filter((obj, pos, arr) => {
        return arr.map(mo => mo['created']).indexOf(obj['created']) === pos;
      });
      return years.map(y => y.created.toString());
    }
  }

  getSchoolYears(): string[] {
    // if (this.allReports) {
    //   const years = this.allReports.filter((obj, pos, arr) => {
    //     return arr.map(mo => mo['schoolYear']).indexOf(obj['schoolYear']) === pos;
    //   });
    //   return years.map(y => y.schoolYear);
    // }
    return this.academicYearsService.getAcademicYears();
  }

  create(): void {
    this.spinnerMsg = 'Creating bulk invoice.  Please wait...';
    this.ngxSpinnerService.show();
    this.reportsService.createBulkInvoice(
      {
        'schoolYear': this.selectedCreateSchoolYear.replace(/\s+/g, ''),
        'name': this.generateBulkInvoiceName(this.selectedCreateSchoolYear, this.selectedScope),
        'templateId': this.selectedTemplate.id,
        'bulkInvoice': {
          'asOf': this.selectedAsOfBillingDate,
          'toSchoolDistrict': this.selectedAsOfBillingDate,
          'toPDE': this.selectedAsOfBillingDate,
          'scope': this.selectedScope
        }
      }
    ).subscribe(
      data => {
        console.log('InvoicesMonthlyCombinedListComponent.create(): data is ', data['reports']);
        this.ngxSpinnerService.hide();
        this.refreshInvoices();
      },
      error => {
        console.log('InvoicesMonthlyCombinedListComponent.create(): error is ', error);
        this.ngxSpinnerService.hide();
      }
    );
  }

  displayDownloadFormatDialog(downloadFormatContent, report: Report): void {
    const modal = this.ngbModal.open(downloadFormatContent, { centered: true, size: 'sm' });
    modal.result.then(
      (result) => {
        console.log('InvoicesListComponent.displayDownloadFormatDialog(): result is ', result);
        this.downloadInvoiceByFormat(report, this.selectedDownloadFormat);
      },
      (reason) => {
        console.log('InvoicesListComponent.displayDownloadFormatDialog(): reason is ', reason);
      }
    );
  }

  public downloadActivityByFormat(report: Report, format: string) {
    this.reportsService.getReportStudentActivityDataByFormat(report, format.includes('Microsoft Excel') ? 'excel' : 'pdf').subscribe(
      data => {
        console.log('InvoiceListComponent.downloadStudentActivityByFormat():  data is ', data);
        if (format.toLowerCase().includes('excel')) {
          this.fileSaverService.saveStudentActivityAsExcelFile(data, report);
        } else {
          this.fileSaverService.saveStudentActivityAsPDFFile(data, report);
        }
      },
      error => {
        console.log('InvoiceListComponent.downloadStudentActivityByFormat():  error is ', error);
      }
    );
  }

  public downloadInvoiceByFormat(report: Report, format: string) {
    this.reportsService.getReportInvoiceByDataFormat(report, format.includes('Microsoft Excel') ? 'excel' : 'pdf').subscribe(
      data => {
        console.log('InvoiceListComponent.downloadInvoiceByFormat():  data is ', data);
        if (format.toLowerCase().includes('excel')) {
          this.fileSaverService.saveInvoiceAsExcelFile(data, report);
        } else {
          this.fileSaverService.saveInvoiceAsPDFFile(data, report);
        }
      },
      error => {
        console.log('InvoiceListComponent.downloadInvoiceByFormat():  error is ', error);
      }
    );
  }

  displayCreateBulkInvoiceDialog(bulkCreateContent): void {
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

  downloadInvoice(invoice: Report) {
    this.reportsService.getInvoiceByName(invoice.name).subscribe(
      data => {
        console.log('InvoicesListComponent.downloadInvoice(): data is', data);
        invoice.xlsx = data;
        this.fileSaverService.saveInvoiceAsExcelFile(data, invoice);
      },
      error => {
        console.log('InvoicesListComponent.downloadInvoice(): error is', error);
        invoice.data = error.error.text;
      }
    );
  }

  downloadStudentActivity(bulkDownloadContent) {
    const modal = this.ngbModal.open(bulkDownloadContent, { centered: true, size: 'sm' });
    this.downloadType = 'students';
    this.selectedDownloadSchoolYear = 'Select School Year';
    this.selectedDownloadStatus = 'Approval Status';

    modal.result.then(
      (result) => {
        this.spinnerMsg = 'Generating student activity data.  Please wait...';
        this.ngxSpinnerService.show();

        this.reportsService.getInvoiceStudentActivityDataBulk(
          this.selectedDownloadSchoolYear,
          this.selectedDownloadStatus === 'Approved').subscribe(
          data => {
            console.log('InvoiceListComponent.downloadStudentActivity():  data is ', data);
            this.ngxSpinnerService.hide();
          },
          error => {
            console.log('InvoiceListComponent.downloadStudentActivity():  error is ', error);
            this.ngxSpinnerService.hide();
          }
        );
      },
      (reason) => {
        console.log('InvoiceListComponent.downloadStudentActivity(): reason is ', reason);
      }
    );
  }

  doDownload() {
    if (this.downloadType === 'invoices') {
      this.reportsService.getBulkInvoices(
        this.selectedDownloadSchoolYear).subscribe(
          data => {
            console.log('InvoicesListComponent.doDownload(): data is ', data);
            this.ngxSpinnerService.hide();
            this.fileSaverService.saveDataAsExcelFile(data, 'BulkInvoices');
          },
          error => {
            console.log('InvoicesListComponent.doDownload(): error is ', error);
            this.ngxSpinnerService.hide();
          }
      );
    } else {
      this.reportsService.getInvoiceStudentActivityDataBulk(
        this.selectedDownloadSchoolYear,
        this.selectedDownloadStatus === 'Approved' ? true : false).subscribe(
          data => {
            console.log('InvoicesListComponent.doDownload(): data is ', data);
            this.ngxSpinnerService.hide();
            this.fileSaverService.saveDataAsExcelFile(data, 'BulkStudentActivity');
          },
          error => {
            console.log('InvoicesListComponent.doDownload(): error is ', error);
            this.ngxSpinnerService.hide();
          }
      );
    }
  }

  private generateBulkInvoiceName(schoolYear: string, asOfDate: string): string {
    const billingDate: Date = new Date(asOfDate);
    const months: string[] = [
      'January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'
    ];

    return 'BulkInvoice_' + months[billingDate.getMonth()] + '-' + billingDate.getFullYear() + '_' + schoolYear;
  }

  private selectSchoolYear(year: string): void {
    this.selectedCreateSchoolYear = year;
  }

  private selectDownloadStatus(status: string): void {
    this.selectedDownloadStatus = status;
  }

  private selectTemplate(template: Template): void {
    this.selectedTemplate = template;
    this.selectedTemplateName = template.name;
  }

  private selectBillingMonth(billingDate: string): void {
    this.selectedAsOfBillingDate = billingDate;
  }

  private getUnapprovedInvoices(): Report[] {
    return this.allBulkReports.filter((r) => (r.type === ReportType.Invoice && !r.approved));
  }

  private getApprovedInvoices(): Report[] {
    return this.allBulkReports.filter((r) => (r.type === ReportType.Invoice && r.approved));
  }

  private getInvoicesForSchoolYear(year: string): Report[] {
    return this.allBulkReports.filter((r) => (r.type === ReportType.Invoice && r.schoolYear === year));
  }

  private getInvoicesBySchoolYearAndStatus(year: string, status: boolean): Report[] {
    return this.allBulkReports.filter((r) => (r.type === ReportType.Invoice && r.schoolYear === year && r.approved === status));
  }

  private filterBulkTemplates(): Template[] {
    return this.templates.filter(r => r.reportType === ReportType.BulkInvoice);
  }
}

import { Component, OnInit } from '@angular/core';

import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

import { Report, ReportType } from '../../../models/report.model';
import { Template } from '../../../models/template.model';

import { ReportsService } from '../../../services/reports.service';
import { UtilitiesService } from '../../../services/utilities.service';
import { FileSaverService } from '../../../services/file-saver.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { TemplatesService } from '../../../services/templates.service';
import { StudentRecordsService } from '../../../services/student-records.service';

import { Globals } from '../../../globals';

import { InvoiceCreateFormComponent } from '../invoice-create-form/invoice-create-form.component';
import { InvoicePreviewFormComponent } from '../invoice-preview-form/invoice-preview-form.component';

import * as FileSaver from 'file-saver';

@Component({
  selector: 'app-invoices-list',
  templateUrl: './invoices-list.component.html',
  styleUrls: ['./invoices-list.component.scss']
})
export class InvoicesListComponent implements OnInit {
  public reports: Report[];
  private allReports: Report[];
  private skip: number;
  public property = 'name';
  private isDescending = false;
  public direction = -1;
  public searchText: string;
  public statuses: string[] = [
    'Approved',
    'Disapproved'
  ];
  private selectedDownloadSchoolYear: string;
  private selectedDownloadStatus: string;
  private selectedTemplate: Template;
  private selectedTemplateName: string;
  public selectedFilterSchoolYear: string;
  public selectedFilterStatus: string;
  private downloadType: string;
  public spinnerMsg: string;
  private templates: Template[];
  public downloadFormats: string[] = [
    'Microsoft Excel',
    'PDF'
  ];
  public selectedDownloadFormat: string;
  public scopes: string[];
  public selectedScope: string;

  constructor(
    private globals: Globals,
    private reportsService: ReportsService,
    private utilitiesService: UtilitiesService,
    private fileSaverService: FileSaverService,
    private templatesService: TemplatesService,
    private ngxSpinnerService: NgxSpinnerService,
    private studentRecordsService: StudentRecordsService,
    private ngbModal: NgbModal,
    private ngbActiveModal: NgbActiveModal
  ) { }

  ngOnInit() {
    this.spinnerMsg = 'Loading invoices.  Please wait...';
    this.ngxSpinnerService.show();

    this.studentRecordsService.getHeaders(true).subscribe(
      data => {
        this.scopes = data['scopes'];
      },
      error => {
        console.log('InvoicesListComponent.ngInit():  error is ', error);
      }
    );

    this.reportsService.getInvoices(null, null, null, null).subscribe(
      data => {
        this.reports = this.allReports = data['reports'];
        this.ngxSpinnerService.hide();
      },
      error => {
        console.log('InvoicesListComponent.ngOnInit(): error is ', error);
        this.ngxSpinnerService.hide();
      }
    );

    this.templatesService.getTemplates(this.skip).subscribe(
      data => {
        this.templates = data['templates'];
      },
      error => {
        console.log(`InvoiceCreateFormComponent.ngOnInit(): error is ${error}.`);
      }
    );

    this.selectedFilterSchoolYear = 'School Year';
    this.selectedFilterStatus = 'Status';
    this.selectedDownloadFormat = 'Download as...';
    this.selectedScope = 'Select billing period...';
  }

  sort(property: string): void {
    this.isDescending = !this.isDescending; // change the direction
    this.property = property;
    this.direction = this.isDescending ? 1 : -1;
  }

  getSortClass(property: string): object {
    return this.utilitiesService.getSortClass({ property: this.property, isDescending: this.isDescending }, property);
  }

  filterInvoices() {
    this.reports = this.allReports.filter(
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
    this.reports = this.allReports;
    this.searchText = '';
  }

  refreshInvoices(): void {
    this.spinnerMsg = 'Loading invoices.  Please wait...';
    this.ngxSpinnerService.show();
    this.reportsService.getReportsByMeta({ 'Type': ReportType.Invoice, 'Name': '', 'SchoolYear': null }).subscribe(
      data => {
        console.log(`InvoicesListComponent.refreshInvoices(): data is ${data}.`);
        this.reports = this.allReports = data['reports'];
        this.ngxSpinnerService.hide();
      },
      error => {
        console.log(`InvoicesListComponent.refreshInvoices(): error is ${error}.`);
        this.ngxSpinnerService.hide();
      }
    );
  }

  listDisplayableFields() {
    if (this.allReports) {
      const fields = this.utilitiesService.objectKeys(this.allReports[0]);
      const rejected = ['data', 'xlsx', 'type', 'id', 'pdf', 'approved'];
      return fields.filter((i) => !rejected.includes(i));
    }
  }

  listDisplayableValues(report: Report) {
    const vkeys = this.listDisplayableFields();
    const selected = this.utilitiesService.pick(report, vkeys);

    return this.utilitiesService.objectValues(selected);
  }

  filterByScope(scope: string) {
    this.selectedScope = scope;
    this.spinnerMsg = `Retrieving invoices for the ${scope} billing period.  Please wait...`;
    this.ngxSpinnerService.show();
    this.reportsService.getInvoices(null, null, scope, null).subscribe(
      data => {
        console.log('InvoicesListComponent.ngOnInit(): invoices are ', data['reports']);
        this.reports = this.allReports = data['reports'];
        this.ngxSpinnerService.hide();
      },
      error => {
        console.log('InvoicesListComponent.ngOnInit(): error is ', error);
        this.ngxSpinnerService.hide();
      }
    );

  }

  filterByApprovedStatus(status: string) {
    this.selectedFilterStatus = status;
    this.reports = this.allReports.filter((r) => (r.type === ReportType.Invoice && r.approved === (status === 'Approved' ? true : false)));
  }

  getSchoolYears(): string[] {
    if (this.allReports) {
      const years = this.allReports.filter((obj, pos, arr) => {
        return arr.map(mo => mo['schoolYear']).indexOf(obj['schoolYear']) === pos;
      });
      return years.map(y => y.schoolYear);
    }
  }

  createInvoice(): void {
    const modal = this.ngbModal.open(InvoiceCreateFormComponent, { centered: true });
    modal.componentInstance.op = 'single';
    modal.result.then(
      (result) => {
        console.log('InvoicesListComponent.createInvoice(): result is ', result);
        this.refreshInvoices();
      },
      (reason) => {
        console.log('InvoicesListComponent.createInvoice(): reason is ', reason);
      }
    );
  }

  createInvoices(): void {
    const modal = this.ngbModal.open(InvoiceCreateFormComponent, { centered: true });
    modal.componentInstance.op = 'many';
    modal.result.then(
      (result) => {
        console.log('InvoicesListComponent.createInvoices(): result is ', result);
        this.refreshInvoices();
      },
      (reason) => {
        console.log('InvoicesListComponent.createInvoices(): reason is ', reason);
      }
    );
  }

  downloadInvoiceStudentActivity(invoice: Report) {
    this.spinnerMsg = 'Downloading student activity.  Please wait...';
    this.ngxSpinnerService.show();
    this.reportsService.getInvoiceStudentActivityDataByName(invoice.name).subscribe(
      data => {
        console.log('InvoiceListComponent().downloadInvoiceStudentActivity():  data is ', data);
        this.ngxSpinnerService.hide();
        this.fileSaverService.saveStudentActivityAsExcelFile(data, invoice);
      },
      error => {
        this.ngxSpinnerService.hide();
        console.log('InvoiceListComponent().downloadInvoiceStudentActivity():  error is ', error);
      }
    );
  }

  setSelectedDownloadFormat(format: string): void {
    this.selectedDownloadFormat = format;
  }

  downloadInvoice(invoice: Report) {
    this.spinnerMsg = 'Downloading invoice.  Please wait...';
    this.ngxSpinnerService.show();
    this.reportsService.getInvoiceByName(invoice.name).subscribe(
      data => {
        console.log('InvoicesListComponent.downloadInvoice(): data is', data);
        invoice.xlsx = data;
        this.ngxSpinnerService.hide();
        this.fileSaverService.saveInvoiceAsExcelFile(data, invoice);
      },
      error => {
        console.log('InvoicesListComponent.downloadInvoice(): error is', error);
        this.ngxSpinnerService.hide();
        invoice.data = error.error.text;
      }
    );
  }

  downloadInvoices(bulkDownloadContent) {
    const modal = this.ngbModal.open(bulkDownloadContent, { centered: true, size: 'sm' });
    this.downloadType = 'invoices';
    this.selectedDownloadSchoolYear = 'Select School Year';
    this.selectedTemplateName = 'Select Template';

    modal.result.then(
      (result) => {
        this.spinnerMsg = 'Generating invoices in bulk.  Please wait...';
        this.ngxSpinnerService.show();

        this.reportsService.getInvoicesBySchoolYearAndStatus(this.selectedDownloadSchoolYear, this.selectedDownloadStatus).subscribe(
          data => {
            console.log(`AdministrationInvoiceListComponent.downloadInvoices(): data is ${data}.`);
            this.ngbActiveModal.close('download successful');
          },
          error => {
            console.log(`AdministrationInvoiceListComponent.downloadInvoices(): error is ${error}.`);
            this.ngbActiveModal.dismiss('download error');
          }
        );
      },
      (reason) => {
        this.ngbActiveModal.dismiss(reason.toString());
      }
    );
  }

  downloadStudentActivity(bulkDownloadContent) {
    const modal = this.ngbModal.open(bulkDownloadContent, { centered: true, size: 'sm' });
    this.downloadType = 'students';
    this.selectedDownloadSchoolYear = 'Select School Year';

    modal.result.then(
      (result) => {
        this.spinnerMsg = 'Generating student activity data.  Please wait...';
        this.ngxSpinnerService.show();

        // this.reportsService.getReportDataByFormat().subscribe(
        //   data => {
        //     console.log('InvoiceListComponent.downloadStudentActivity():  data is ', data);
        //     this.ngxSpinnerService.hide();
        //   },
        //   error => {
        //     console.log('InvoiceListComponent.downloadStudentActivity():  error is ', error);
        //     this.ngxSpinnerService.hide();
        //   }
        // );
      },
      (reason) => {
        console.log('InvoiceListComponent.downloadStudentActivity(): reason is ', reason);
      }
    );
  }

  public downloadActivityByFormat(report: Report, format: string) {
    this.spinnerMsg = 'Downloading student activity.  Please wait...';
    this.ngxSpinnerService.show();
    this.reportsService.getReportDataByFormat(report, format.includes('Microsoft Excel') ? 'excel' : 'pdf').subscribe(
      data => {
        console.log('InvoiceListComponent.downloadStudentActivityByFormat():  data is ', data);
        this.ngxSpinnerService.hide();
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
    this.spinnerMsg = 'Downloading invoice.  Please wait...';
    this.ngxSpinnerService.show();
    this.reportsService.getReportDataByFormat(report, format.includes('Microsoft Excel') ? 'excel' : 'pdf').subscribe(
      data => {
        console.log('InvoiceListComponent.downloadInvoiceByFormat():  data is ', data);
        this.ngxSpinnerService.hide();
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

  displayDownloadFormatDialog(downloadFormatContent, type: string, report: Report): void {
    if (type !== 'activity') {
      const modal = this.ngbModal.open(downloadFormatContent, { centered: true, size: 'sm' });
      modal.result.then(
        (result) => {
          this.downloadInvoiceByFormat(report, this.selectedDownloadFormat);
        },
        (reason) => {
          console.log('InvoicesListComponent.displayDownloadFormatDialog(): reason is ', reason);
        }
      );
    } else {
      this.downloadActivityByFormat(report, 'Microsoft Excel');
    }
  }

  private selectSchoolYear(year: string) {
    this.selectedDownloadSchoolYear = year;
  }

  private selectDownloadStatus(status: string) {
    this.selectedDownloadStatus = status;
  }

  private selectTemplate(template: Template) {
    this.selectedTemplate = template;
    this.selectedTemplateName = template.name;
  }

  private getUnapprovedInvoices(): Report[] {
    return this.allReports.filter((r) => (r.type === ReportType.Invoice && !r.approved));
  }

  private getApprovedInvoices(): Report[] {
    return this.allReports.filter((r) => (r.type === ReportType.Invoice && r.approved));
  }

  private getInvoicesForSchoolYear(year: string): Report[] {
    return this.allReports.filter((r) => (r.type === ReportType.Invoice && r.schoolYear === year));
  }

  private getInvoicesBySchoolYearAndStatus(year: string, status: boolean): Report[] {
    return this.allReports.filter((r) => (r.type === ReportType.Invoice && r.schoolYear === year && r.approved === status));
  }

  private filterNonBulkTemplates(): Template[] {
    return this.templates.filter(r => r.reportType === ReportType.BulkInvoice);
  }
}

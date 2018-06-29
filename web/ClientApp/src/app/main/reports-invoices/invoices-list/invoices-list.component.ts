import { Component, OnInit } from '@angular/core';

import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

import { Report, ReportType } from '../../../models/report.model';

import { ReportsService } from '../../../services/reports.service';
import { UtilitiesService } from '../../../services/utilities.service';
import { ExcelService } from '../../../services/excel.service';

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
  private reports: Report[];
  private allReports: Report[];
  private skip: number;
  private property: string;
  private direction: number;
  private isDescending: boolean;
  private searchText: string;
  private statuses: string[] = [
    'Approved',
    'Disapproved'
  ];
  private selectedDownloadSchoolYear: string;
  private selectedDownloadStatus: string;
  private downloadType: string;

  constructor(
    private globals: Globals,
    private reportsService: ReportsService,
    private utilitiesService: UtilitiesService,
    private excelService: ExcelService,
    private ngbModal: NgbModal,
    private ngbActiveModal: NgbActiveModal
  ) { }

  ngOnInit() {
    this.reportsService.getInvoices(null, null, null).subscribe(
      data => {
        console.log('InvoicesListComponent.ngOnInit(): invoices are ', data['reports']);
        this.reports = this.allReports = data['reports'];
      },
      error => {
        console.log('InvoicesListComponent.ngOnInit(): error is ', error);
      }
    )
  }

  sort(property) {
    this.isDescending = !this.isDescending; // change the direction
    this.property = property;
    this.direction = this.isDescending ? 1 : -1;
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
    this.reportsService.getReportsByInfo({'Type': ReportType.Invoice, 'Name': '', 'Approved': null, 'SchoolYear': null}).subscribe(
      data => {
        console.log(`InvoicesListComponent.refreshInvoices(): data is ${data}.`);
        this.reports = this.allReports = data['reports'];
      },
      error => {
        console.log(`InvoicesListComponent.refreshInvoices(): error is ${error}.`);
      }
    );
  }

  listDisplayableFields() {
    if (this.allReports) {
      const fields = this.utilitiesService.objectKeys(this.allReports[0]);
      const rejected = ['data', 'xlsx', 'type', 'id'];
      return fields.filter((i) => !rejected.includes(i));
    }
  }

  listDisplayableValues(report: Report) {
    const vkeys = this.listDisplayableFields();
    const selected = this.utilitiesService.pick(report, vkeys);

    return this.utilitiesService.objectValues(selected);
  }

  filterBySchoolYear(year: string) {
    this.reports = this.allReports.filter((r) => (r.type === ReportType.Invoice && r.schoolYear === year));
  }

  filterByApprovedStatus(status: string) {
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
    modal.componentInstance.op == 'single';
    modal.result.then(
      (result) => {
        console.log('InvoicesListComponent.createInvoices(): result is ', result);
        this.refreshInvoices();
      },
      (reason) => {
        console.log('InvoicesListComponent.createInvoices(): reason is ', reason);
      }
    )
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
    )
  }

  approveInvoices() {
    const modal = this.ngbModal.open(InvoicePreviewFormComponent, { centered: true, size: 'lg' });
    modal.componentInstance.invoices = this.getUnapprovedInvoices();
    modal.result.then(
      (result) => {
        console.log('InvoicesListComponent.previewInvoices(): result is ', result);
      },
      (reason) => {
        console.log('InvoicesListComponent.previewInvoices(): reason is ', reason);
      }
    );
  }

  previewInvoice(invoice: Report) {
    const modal = this.ngbModal.open(InvoicePreviewFormComponent, { centered: true, size: 'lg' });
    modal.componentInstance.invoices = [invoice];
    modal.result.then(
      (result) => {
      },
      (reason) => {
      }
    )
  }

  downloadInvoice(invoice: Report) {
    this.reportsService.getInvoiceByName(invoice.name).subscribe(
      data => {
        console.log('InvoicesListComponent.downloadInvoice(): data is', data);
        invoice.xlsx = data;
        this.excelService.saveInvoiceAsExcelFile(invoice);
      },
      error => {
        console.log('InvoicesListComponent.downloadInvoice(): error is', error);
        invoice.data = error.error.text;
      }
    );
  }

  downloadInvoices(bulkDownloadContent) {
    const modal = this.ngbModal.open(bulkDownloadContent, { centered: true, size: 'sm' });
    this.downloadType = 'invoices';
    this.selectedDownloadSchoolYear = 'Select School Year';
    this.selectedDownloadStatus = 'Approval Status';

    modal.result.then(
      (result) => {
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
    )
  }

  downloadStudentActivity(bulkDownloadContent) {
    const modal = this.ngbModal.open(bulkDownloadContent, { centered: true, size: 'sm' });
    this.downloadType = 'students';
    this.selectedDownloadSchoolYear = 'Select School Year';
    this.selectedDownloadStatus = 'Approval Status';

    modal.result.then(
      (result) => {
        this.reportsService.getInvoiceStudentActivityDataBulk(
          this.selectedDownloadSchoolYear,
          this.selectedDownloadStatus === 'Approved').subscribe(
          data => {
            console.log('InvoiceListComponent.downloadStudentActivity():  data is ', data);
          },
          error => {
            console.log('InvoiceListComponent.downloadStudentActivity():  error is ', error);
          }
        )
      },
      (reason) => {
        console.log('InvoiceListComponent.downloadStudentActivity(): reason is ', reason);
      }
    )
  }

  doDownload() {
    if (this.downloadType === 'invoices') {
      this.reportsService.getInvoicesBulk(
        this.selectedDownloadSchoolYear,
        this.selectedDownloadStatus === 'Approved' ? true : false).subscribe(
        data => {
          console.log('InvoicesListComponent.doDownload(): data is ', data);
          this.excelService.saveDataAsExcelFile(data, 'bulk_invoices');
          this.ngbActiveModal.close('download successful');
        },
        error => {
          console.log('InvoicesListComponent.doDownload(): error is ', error);
          this.ngbActiveModal.dismiss('download failed');
        }
      );
    }
    else {
      this.reportsService.getInvoiceStudentActivityDataBulk(
        this.selectedDownloadSchoolYear,
        this.selectedDownloadStatus === 'Approved' ? true : false).subscribe(
        data => {
          console.log('InvoicesListComponent.doDownload(): data is ', data);
          this.excelService.saveDataAsExcelFile(data, 'bulk_invoices');
          this.ngbActiveModal.close('download successful');
        },
        error => {
          console.log('InvoicesListComponent.doDownload(): error is ', error);
          this.ngbActiveModal.dismiss('download failed');
        }
      );
    }
  }

  private selectSchoolYear(year: string) {
    this.selectedDownloadSchoolYear = year;
  }

  private selectDownloadStatus(status: string) {
    this.selectedDownloadStatus = status;
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
    return this.allReports.filter((r) => (r.type === ReportType.Invoice && r.schoolYear === year && r.approved == status));
  }
}

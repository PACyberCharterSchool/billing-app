import { Component, OnInit } from '@angular/core';

import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

import { Report, ReportType } from '../../../models/report.model';

import { ReportsService } from '../../../services/reports.service';
import { UtilitiesService } from '../../../services/utilities.service';

import { Globals } from '../../../globals';

import { InvoiceCreateFormComponent } from '../invoice-create-form/invoice-create-form.component';
import { InvoicePreviewFormComponent } from '../invoice-preview-form/invoice-preview-form.component';

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
  private downloadSchoolYear: string;
  private downloadStatus: string;

  constructor(
    private globals: Globals,
    private reportsService: ReportsService,
    private utilitiesService: UtilitiesService,
    private ngbModal: NgbModal
  ) { }

  ngOnInit() {
    this.reportsService.getReportsByType(ReportType.Invoice, this.skip).subscribe(
      data => {
        console.log(`InvoicesListComponent.ngOnInit(): data is ${data}.`);
        this.reports = this.allReports = data;
      },
      error => {
        console.log(`InvoicesListComponent.ngOnInit(): error is ${error}.`);
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
    const years = this.allReports.filter((v, i, s) => s.indexOf(v) === i).map((i) => i.schoolYear);
    return years;
  }

  createInvoices() {
    const modal = this.ngbModal.open(InvoiceCreateFormComponent, { centered: true });
    modal.result.then(
      (result) => {
        console.log('InvoicesListComponent.createInvoices(): result is ', result);
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

  downloadInvoices(bulkDownloadContent) {
    this.ngbModal.open(bulkDownloadContent, { centered: true, size: 'sm' }).result.then(
      (result) => {
      },
      (reason) => {
      }
    )
  }

  doDownload() {
  }

  previewInvoice(invoice: Report) {
  }

  downloadInvoice(invoice: Report) {
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

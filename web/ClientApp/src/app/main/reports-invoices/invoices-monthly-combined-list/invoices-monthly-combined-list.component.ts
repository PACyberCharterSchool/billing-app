import { Component, OnInit } from '@angular/core';

import { Report, ReportType } from '../../../models/report.model';
import { Template } from '../../../models/template.model';
import { SchoolDistrict } from '../../../models/school-district.model';
import { PaymentType } from '../../../models/payment.model';

import { Globals } from '../../../globals';

import { ReportsService } from '../../../services/reports.service';
import { UtilitiesService } from '../../../services/utilities.service';
import { FileSaverService } from '../../../services/file-saver.service';
import { TemplatesService } from '../../../services/templates.service';
import { AcademicYearsService } from '../../../services/academic-years.service';
import { StudentRecordsService } from '../../../services/student-records.service';
import { SchoolDistrictService } from '../../../services/school-district.service';

import { InvoiceCreateFormComponent } from '../invoice-create-form/invoice-create-form.component';

import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

import { NgxSpinnerService } from 'ngx-spinner';
import * as moment from 'moment';

@Component({
  selector: 'app-invoices-monthly-combined-list',
  templateUrl: './invoices-monthly-combined-list.component.html',
  styleUrls: ['./invoices-monthly-combined-list.component.scss']
})
export class InvoicesMonthlyCombinedListComponent implements OnInit {
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
  private selectedDownloadStatus: string;
  public selectedFilterSchoolYear: string;
  public selectedFilterStatus: string;
  private downloadType: string;
  private spinnerMsg: string;
  private templates: Template[];
  public selectedCreateTemplate: Template;
  public selectedCreateTemplateName: string;
  public selectedCurrentScope: string;
  public selectedCreateScope: string;
  public scopes: string[];
  public selectedDownloadFormat: string;
  public invoiceRecipient;
  private schoolDistricts: SchoolDistrict[];
  public toSchoolDistrictDate;
  public toPDEDate;
  public paymentType: string;

  constructor(
    private globals: Globals,
    private reportsService: ReportsService,
    private utilitiesService: UtilitiesService,
    private fileSaverService: FileSaverService,
    private templatesService: TemplatesService,
    private ngxSpinnerService: NgxSpinnerService,
    private academicYearsService: AcademicYearsService,
    private studentRecordsService: StudentRecordsService,
    private schoolDistrictsService: SchoolDistrictService,
    private ngbModal: NgbModal,
    private ngbActiveModal: NgbActiveModal
  ) { }

  ngOnInit() {
    this.selectedCreateTemplateName = 'Select Bulk Invoice Template';
    this.selectedCurrentScope = 'Select billing period';
    this.allBulkReports = [];
    this.bulkReports = [];

    this.refreshInvoices();
    this.templatesService.getTemplates(this.skip).subscribe(
      data => {
        this.templates = data['templates'];
      },
      error => {
        console.log(`InvoicesMonthlyCombinedListComponent.ngOnInit(): error is ${error}.`);
      }
    );

    this.studentRecordsService.getHeaders(true).subscribe(
      data => {
        this.scopes = data['scopes'];
      },
      error => {
        console.log(`InvoicesMonthlyCombinedListComponent.ngOnInit(): error is ${error}.`);
      }
    );

    this.schoolDistrictsService.getSchoolDistricts().subscribe(
      data => {
        this.schoolDistricts = data['schoolDistricts'];
      },
      error => {
        console.log('InvoicesMonthlyCombinedListComponent.ngOnInit():  error is ', error);
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

  getSortClass(property: string): object {
    return {
      'fa-sort': this.property !== property,
      'fa-sort-desc': this.property === property && this.isDescending,
      'fa-sort-asc': this.property === property && !this.isDescending,
    };
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
    this.bulkReports = this.allBulkReports = [];
    this.ngxSpinnerService.show();
    this.reportsService.getReportsByMeta({
      'Type': ReportType.BulkInvoice,
    }).subscribe(
      data => {
        this.ngxSpinnerService.hide();
        this.allBulkReports = this.allBulkReports.concat(data['reports']);
        this.bulkReports = this.allBulkReports;
      },
      error => {
        this.ngxSpinnerService.hide();
        console.log(`InvoicesMonthlyCombinedListComponent.refreshInvoices(): error is ${error}.`);
      }
    );

    this.spinnerMsg = 'Loading totals only invoices.  Please wait...';
    this.ngxSpinnerService.show();
    this.reportsService.getReportsByMeta({
      'Type': ReportType.TotalsOnly,
    }).subscribe(
      data => {
        this.ngxSpinnerService.hide();
        this.allBulkReports = this.allBulkReports.concat(data['reports']);
        this.bulkReports = this.allBulkReports;
      },
      error => {
        this.ngxSpinnerService.hide();
        console.log(`InvoicesMonthlyCombinedListComponent.refreshInvoices(): error is ${error}.`);
      }
    );
  }

  setSelectedDownloadFormat(format: string): void {
    this.selectedDownloadFormat = format;
  }

  filterByScope(scope: string): void {
    this.selectedCurrentScope = scope;
    this.ngxSpinnerService.show();
    this.bulkReports = this.allBulkReports = [];

    this.reportsService.getBulkInvoices(null, scope).subscribe(
      data => {
        this.allBulkReports = this.allBulkReports.concat(data['reports']);
        this.bulkReports = this.allBulkReports;
        this.ngxSpinnerService.hide();
      },
      error => {
        console.log('InvoicesMonthlyCombinedListComponent.filterByScope():  error is ', error);
        this.ngxSpinnerService.hide();
      }
    );

    this.reportsService.getTotalsOnly().subscribe(
      data => {
        this.allBulkReports = this.allBulkReports.concat(data['reports']);
        this.bulkReports = this.allBulkReports;
      },
      error => {
        console.log('InvoicesMonthlyCombinedListComponent.filterByScope():  error is ', error);
      }
    );
  }

  filterBySchoolYear(year: string) {
    this.selectedFilterSchoolYear = year;
    this.bulkReports = this.allBulkReports.filter((r) => (r.type === ReportType.Invoice && r.schoolYear === year));
  }

  selectCreatedScope(scope: string): void {
    this.selectedCreateScope = scope;
  }

  selectCreatedTemplate(template: Template): void {
    this.selectedCreateTemplate = template;
    this.selectedCreateTemplateName = template.name;
  }

  getSchoolYears(): string[] {
    return this.academicYearsService.getAcademicYears();
  }

  private isTotalsOnly(): boolean {
    return this.invoiceRecipient === 'Totals';
  }

  private doCreateBulkInvoice(): void {
    this.spinnerMsg = 'Creating bulk invoice.  Please wait...';
    this.ngxSpinnerService.show();

    let paymentType: string;
    if (this.invoiceRecipient === 'SD') {
      paymentType = 'Check';
    } else if (this.invoiceRecipient === 'PDE') {
      paymentType = 'UniPay';
    }

    this.reportsService.createBulkInvoice(
      {
        reportType: 'BulkInvoice',
        schoolYear: this.selectedCreateSchoolYear.replace(/\s+/g, ''),
        name: this.generateBulkInvoiceName(this.selectedCreateScope),
        bulkInvoice: {
          toSchoolDistrict: new Date(
            this.toSchoolDistrictDate.year,
            this.toSchoolDistrictDate.month - 1,
            this.toSchoolDistrictDate.day).toLocaleDateString('en-US'),
          toPDE: new Date(
            this.toPDEDate.year,
            this.toPDEDate.month - 1,
            this.toPDEDate.day).toLocaleDateString('en-US'),
          scope: this.selectedCreateScope,
          paymentType: paymentType,
        }
      }
    ).subscribe(
      data => {
        this.ngxSpinnerService.hide();
        this.refreshInvoices();
      },
      error => {
        console.log('InvoicesMonthlyCombinedListComponent.create(): error is ', error);
        this.ngxSpinnerService.hide();
      }
    );
  }

  private doCreateTotalsOnlyInvoice(): void {
    this.spinnerMsg = 'Creating totals only invoice.  Please wait...';
    this.ngxSpinnerService.show();

    this.reportsService.createTotalsOnlyInvoice(
      this.generateTotalsOnlyInvoiceName(this.selectedCreateScope, this.paymentType),
      this.selectedCreateScope,
      this.selectedCreateSchoolYear,
      this.paymentType === 'All' ? undefined : this.paymentType).subscribe(
        data => {
          this.ngxSpinnerService.hide();
          this.refreshInvoices();
        },
        error => {
          this.ngxSpinnerService.hide();
        }
      );
  }

  create(): void {
    if (this.isTotalsOnly()) {
      this.doCreateTotalsOnlyInvoice();
    } else {
      this.doCreateBulkInvoice();
    }
  }

  displayDownloadFormatDialog(downloadFormatContent, report: Report): void {
    const modal = this.ngbModal.open(downloadFormatContent, { centered: true, size: 'sm' });
    modal.result.then(
      (result) => {
        this.downloadInvoiceByFormat(report, this.selectedDownloadFormat);
      },
      (reason) => {
      }
    );
  }

  public downloadInvoiceByFormat(report: Report, format: string) {
    this.ngxSpinnerService.show();
    this.reportsService.getReportDataByFormat(report, format.includes('Microsoft Excel') ? 'excel' : 'pdf').subscribe(
      data => {
        this.ngxSpinnerService.hide();
        if (format.toLowerCase().includes('excel')) {
          this.fileSaverService.saveInvoiceAsExcelFile(data, report);
        } else {
          this.fileSaverService.saveInvoiceAsPDFFile(data, report);
        }
      },
      error => {
        this.ngxSpinnerService.hide();
      }
    );
  }

  displayCreateBulkInvoiceDialog(bulkCreateContent): void {
    const modal = this.ngbModal.open(bulkCreateContent, { centered: true, size: 'lg' });
    this.selectedCreateTemplateName = 'Select Template';

    modal.result.then(
      (result) => {
        this.ngxSpinnerService.show();
      },
      (reason) => {
      }
    );
  }

  onIssuedSchoolDistrictDateChanged() {
  }

  onIssuedPDEDateChanged() {
  }

  private generateBulkInvoiceName(scope: string): string {
    let name = `${scope}_Combined`;
    if (this.invoiceRecipient !== '' && this.invoiceRecipient !== 'All') {
      name += `_${this.invoiceRecipient}`;
    }
    name += `_${moment().format(this.globals.dateFormat)}`;

    return name;
  }

  private generateTotalsOnlyInvoiceName(scope: string, paymentType: string): string {
    let name = `Totals_${scope}`;
    if (paymentType !== undefined && paymentType !== 'All') {
      name += `_${paymentType}`;
    }
    name += `_${moment().format(this.globals.dateFormat)}`;

    return name;
  }

  private selectSchoolYear(year: string): void {
    this.selectedCreateSchoolYear = year;
  }

  private selectDownloadStatus(status: string): void {
    this.selectedDownloadStatus = status;
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

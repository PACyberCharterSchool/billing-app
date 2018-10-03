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
    this.spinnerMsg = 'Loading bulk invoices.  Please wait...';

    this.ngxSpinnerService.show();
    this.reportsService.getBulkInvoices(null, null).subscribe(
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

    this.templatesService.getTemplates(this.skip).subscribe(
      data => {
        console.log(`InvoicesListComponent.ngOnInit(): data is ${data}.`);
        this.templates = data['templates'];
      },
      error => {
        console.log(`InvoicesListComponent.ngOnInit(): error is ${error}.`);
      }
    );

    this.studentRecordsService.getHeaders(true).subscribe(
      data => {
        console.log(`InvoicesListComponent.ngOnInit(): data is ${data}.`);
        this.scopes = data['scopes'];
      },
      error => {
        console.log(`InvoicesListComponent.ngOnInit(): error is ${error}.`);
      }
    );

    this.schoolDistrictsService.getSchoolDistricts().subscribe(
      data => {
        console.log('InvoicesListComponent.ngOnInit():  data is ', data);
        this.schoolDistricts = data['schoolDistricts'];
      },
      error => {
        console.log('InvoicesListComponent.ngOnInit():  error is ', error);
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
    this.reportsService.getReportsByMeta({
      'Type': ReportType.BulkInvoice,
      'Name': '',
      'Approved': null,
      'SchoolYear': null
    }).subscribe(
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

  filterByScope(scope: string): void {
    this.selectedCurrentScope = scope;
    this.ngxSpinnerService.show();
    this.reportsService.getBulkInvoices(null, scope).subscribe(
      data => {
        this.bulkReports = this.allBulkReports = data['reports'];
        this.ngxSpinnerService.hide();
      },
      error => {
        console.log('StudentActivityListComponent.filterByScope():  error is ', error);
        this.ngxSpinnerService.hide();
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

  private getInvoiceCreateParams(): number[] {
    let auns: number[];

    switch (this.invoiceRecipient) {
      case 'SD':
        auns = this.schoolDistricts.filter((sd) => sd.paymentType === PaymentType.Check).map((sd) => +sd.aun);
        break;
      case 'PDE':
        auns = this.schoolDistricts.filter((sd) => sd.paymentType === 'ACH').map((sd) => +sd.aun);
        break;
      case 'All':
        auns = null;
        break;
    }

    return auns;
  }

  private isTotalsOnly(): boolean {
    return this.invoiceRecipient === 'Totals';
  }

  create(): void {
    this.spinnerMsg = 'Creating bulk invoice.  Please wait...';
    this.ngxSpinnerService.show();
    const auns: number[] = this.getInvoiceCreateParams();
    const totalsOnly = this.isTotalsOnly();

    this.reportsService.createBulkInvoice(
      {
        'reportType': 'BulkInvoice',
        'schoolYear': this.selectedCreateSchoolYear.replace(/\s+/g, ''),
        'name': this.generateBulkInvoiceName(this.selectedCreateSchoolYear, this.selectedCreateScope),
        'bulkInvoice': {
          'toSchoolDistrict': new Date(
            this.toSchoolDistrictDate.year,
            this.toSchoolDistrictDate.month - 1,
            this.toSchoolDistrictDate.day).toLocaleDateString('en-US'),
          'toPDE': new Date(
            this.toPDEDate.year,
            this.toPDEDate.month - 1,
            this.toPDEDate.day).toLocaleDateString('en-US'),
          'scope': this.selectedCreateScope,
          'auns': auns,
          'totalsOnly': totalsOnly,
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

  public downloadInvoiceByFormat(report: Report, format: string) {
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
        this.ngxSpinnerService.hide();
        console.log('InvoiceListComponent.downloadInvoiceByFormat():  error is ', error);
      }
    );
  }

  displayCreateBulkInvoiceDialog(bulkCreateContent): void {
    const modal = this.ngbModal.open(bulkCreateContent, { centered: true, size: 'lg' });
    this.selectedCreateTemplateName = 'Select Template';

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

  onIssuedSchoolDistrictDateChanged() {
  }

  onIssuedPDEDateChanged() {
  }

  private generateInvoiceRecipientFileNameTag(): string {
    switch (this.invoiceRecipient) {
      case 'SD':
        return 'SD_Only';
      case 'PDE':
        return 'PDE_Only';
      case 'Totals':
        return 'Totals_Only';
      default:
        return '';
    }
  }

  private generateBulkInvoiceName(schoolYear: string, scope: string): string {
    return 'BulkInvoice_' + scope + '_' + schoolYear + this.generateInvoiceRecipientFileNameTag();
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

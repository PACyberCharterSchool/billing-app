import { Component, OnInit, Input } from '@angular/core';

import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

import { Report, ReportType } from '../../../models/report.model';
import { Template } from '../../../models/template.model';

import { ReportsService } from '../../../services/reports.service';
import { UtilitiesService } from '../../../services/utilities.service';
import { AcademicYearsService } from '../../../services/academic-years.service';
import { TemplatesService } from '../../../services/templates.service';
import { SchoolDistrictService } from '../../../services/school-district.service';
import { StudentRecordsService } from '../../../services/student-records.service';

import { SchoolDistrict } from '../../../models/school-district.model';

import { Globals } from '../../../globals';

import { Observable } from 'rxjs/Observable';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';

import { NgxSpinnerService } from 'ngx-spinner';
import * as moment from 'moment';

@Component({
  selector: 'app-invoice-create-form',
  templateUrl: './invoice-create-form.component.html',
  styleUrls: ['./invoice-create-form.component.scss']
})
export class InvoiceCreateFormComponent implements OnInit {
  private invoiceTemplate: string;
  // public asOfDate;
  public toSchoolDistrictDate;
  public toPDEDate;
  private studentTemplate: string;
  public selectedScope: string;
  public selectedSchoolYear: string;
  private templates: Template[];
  public selectedInvoiceTemplate: Template;
  private selectedSchoolTemplate: Template;
  private skip: number;
  private selectedSchoolDistrict: SchoolDistrict;
  private schoolDistricts: SchoolDistrict[];
  public scopes: string[];

  @Input() op: string;

  constructor(
    private globals: Globals,
    private reportsService: ReportsService,
    private utilitiesService: UtilitiesService,
    private academicYearsService: AcademicYearsService,
    private templatesService: TemplatesService,
    private schoolDistrictService: SchoolDistrictService,
    private ngxSpinnerService: NgxSpinnerService,
    private studentRecordsService: StudentRecordsService,
    public ngbActiveModal: NgbActiveModal
  ) { }

  ngOnInit() {
    this.templatesService.getTemplates(this.skip).subscribe(
      data => {
        console.log('InvoiceCreateFormComponent.ngOnInit(): data is ', data);
        this.templates = data['templates'];
      },
      error => {
        console.log('InvoiceCreateFormComponent.ngOnInit(): error is ', error);
      }
    );

    this.schoolDistrictService.getSchoolDistricts().subscribe(
      data => {
        console.log('InvoiceCreateFormComponent.ngOnInit(): data is ', data);
        this.schoolDistricts = data['schoolDistricts'];
      },
      error => {
        console.log('InvoiceCreateFormComponent.ngOnInit(): error is ', error);
      }
    );

    this.studentRecordsService.getHeaders(true).subscribe(
      data => {
        console.log('StudentsListComponent.ngOnInit(): data is ', data['scopes']);
        this.scopes = data['scopes'];
      },
      error => {
        console.log('StudentsListComponent.ngOnInit():  error is ', error);
      }
    );

  }

  getSchoolYears(): string[] {
    return this.academicYearsService.getAcademicYears();
  }

  setSelectedSchoolYear(year: string): void {
    this.selectedSchoolYear = year;
  }

  create(): void {
    if (this.isMany()) {
      this.createInvoices();
      this.ngxSpinnerService.show();
    } else {
      this.createInvoice();
      this.ngxSpinnerService.show();
    }
  }

  createInvoice(): void {
    console.log(`InvoiceCreateFormComponent.createInvoice(): whatever.`);
    this.reportsService.createInvoice(this.buildInvoiceCreationInfo()).subscribe(
      data => {
        console.log('InvoiceCreateFormComponent.createInvoice(): data is ', data);
        this.ngxSpinnerService.hide();
        this.ngbActiveModal.close('Invoices created');
      },
      error => {
        console.log('InvoiceCreateFormComponent.createInvoice(): error is ', error);
        this.ngxSpinnerService.hide();
        this.ngbActiveModal.dismiss('Invoices creation failed');
      }
    );
  }

  createInvoices(): void {
    console.log(`InvoiceCreateFormComponent.createInvoices(): whatever.`);
    this.reportsService.createInvoices(this.buildInvoicesCreationInfo()).subscribe(
      data => {
        console.log('InvoiceCreateFormComponent.createInvoices(): data is ', data);
        this.ngxSpinnerService.hide();
        this.ngbActiveModal.close('Invoices created');
      },
      error => {
        console.log('InvoiceCreateFormComponent.createInvoices(): error is ', error);
        this.ngxSpinnerService.hide();
        this.ngbActiveModal.dismiss('Invoices creation failed');
      }
    );
  }

  getInvoiceTemplates() {
    if (this.templates) {
      return this.templates.filter((t) => t.reportType === ReportType.Invoice);
    }
  }

  setSelectedScope(scope: string): void {
    this.selectedScope = scope;
  }

  setSelectedInvoiceTemplate(template: Template): void {
    this.selectedInvoiceTemplate = template;
  }

  onAsOfDateChanged() {
  }

  onIssuedSchoolDistrictDateChanged() {
  }

  onIssuedPDEDateChanged() {
  }

  isMany(): boolean {
    return this.op === 'many';
  }

  setSelectedSchoolDistrict($event) {
    this.selectedSchoolDistrict = this.schoolDistricts.find((sd) => sd.name === $event.item);
  }

  search = (text$: Observable<string>) => {
    const results = text$.pipe(
      debounceTime(200),
      distinctUntilChanged(),
      map((term) => {
        return term.length < 2 ? [] : this.schoolDistricts.filter(
          (sd) => {
            if (sd.name.toLowerCase().indexOf(term.toLowerCase()) > -1) {
              return true;
            } else {
              return false;
            }
          }).map((sd) => sd.name);
      })
    );

    return results;
  }

  private buildInvoiceCreationInfo(): Object {
    return {
      reportType: ReportType.Invoice,
      name: `Invoice_${this.selectedSchoolDistrict.name}_${this.selectedScope}_${moment().format(this.globals.dateFormat)}`,
      schoolYear: this.selectedSchoolYear.replace(/\s+/g, ''),
      invoice: {
        // asOf: new Date(Date.now()).toLocaleDateString('en-US'),
        toSchoolDistrict: new Date(`${this.toSchoolDistrictDate.month}/${this.toSchoolDistrictDate.day}/${this.toSchoolDistrictDate.year}`),
        toPDE: new Date(`${this.toPDEDate.month}/${this.toPDEDate.day}/${this.toPDEDate.year}`),
        schoolDistrictAun: +this.selectedSchoolDistrict.aun,
        studentsTemplateId: null,
        scope: this.selectedScope.replace(/\s+/g, '')
      }
    };
  }

  private buildInvoicesCreationInfo(): Object {
    return {
      reportType: ReportType.Invoice,
      schoolYear: this.selectedSchoolYear.replace(/\s+/g, ''),
      invoice: {
        // asOf: new Date(Date.now()).toLocaleDateString('en-US'),
        toSchoolDistrict: new Date(`${this.toSchoolDistrictDate.month}/${this.toSchoolDistrictDate.day}/${this.toSchoolDistrictDate.year}`),
        toPDE: new Date(`${this.toPDEDate.month}/${this.toPDEDate.day}/${this.toPDEDate.year}`),
        scope: this.selectedScope.replace(/\s+/g, '')
      }
    };
  }
}

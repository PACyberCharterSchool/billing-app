import { Component, OnInit, Input } from '@angular/core';

import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

import { Report, ReportType } from '../../../models/report.model';
import { Template } from '../../../models/template.model';

import { ReportsService } from '../../../services/reports.service';
import { UtilitiesService } from '../../../services/utilities.service';
import { AcademicYearsService } from '../../../services/academic-years.service';
import { TemplatesService } from '../../../services/templates.service';
import { SchoolDistrictService } from '../../../services/school-district.service';

import { SchoolDistrict } from '../../../models/school-district.model';

import { Globals } from '../../../globals';

import { Observable } from 'rxjs/Observable';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';

import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-invoice-create-form',
  templateUrl: './invoice-create-form.component.html',
  styleUrls: ['./invoice-create-form.component.scss']
})
export class InvoiceCreateFormComponent implements OnInit {
  private schoolYear: string;
  private invoiceTemplate: string;
  private asOfDate;
  private toSchoolDistrictDate;
  private toPDEDate;
  private studentTemplate: string;
  private selectedSchoolYear: string;
  private schoolYears: string[];
  private templates: Template[];
  private selectedInvoiceTemplate: Template;
  private selectedSchoolTemplate: Template;
  private skip: number;
  private selectedSchoolDistrictName: string;
  private selectedSchoolDistrict: SchoolDistrict;
  private schoolDistricts: SchoolDistrict[];

  @Input() op: string;

  constructor(
    private globals: Globals,
    private reportsService: ReportsService,
    private utilitiesService: UtilitiesService,
    private academicYearsService: AcademicYearsService,
    private templatesService: TemplatesService,
    private schoolDistrictService: SchoolDistrictService,
    private ngxSpinnerService: NgxSpinnerService,
    private ngbActiveModal: NgbActiveModal
  ) { }

  ngOnInit() {
    this.schoolYears = this.academicYearsService.getAcademicYears();
    this.templatesService.getTemplates(this.skip).subscribe(
      data => {
        console.log(`InvoiceCreateFormComponent.ngOnInit(): data is ${data}.`);
        this.templates = data['templates'];
      },
      error => {
        console.log(`InvoiceCreateFormComponent.ngOnInit(): error is ${error}.`);
      }
    );

    this.schoolDistrictService.getSchoolDistricts().subscribe(
      data => {
        console.log(`InvoiceCreateFormComponent.ngOnInit(): data is ${data}.`);
        this.schoolDistricts = data['schoolDistricts'];
      },
      error => {
        console.log(`InvoiceCreateFormComponent.ngOnInit(): error is ${error}.`);
      }
    );
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
        console.log(`InvoiceCreateFormComponent.createInvoice(): data is ${data}.`);
        this.ngxSpinnerService.hide();
        this.ngbActiveModal.close('Invoices created');
      },
      error => {
        console.log(`InvoiceCreateFormComponent.createInvoice(): error is ${error}.`);
        this.ngxSpinnerService.hide();
        this.ngbActiveModal.dismiss('Invoices creation failed');
      }
    );
  }

  createInvoices(): void {
    console.log(`InvoiceCreateFormComponent.createInvoices(): whatever.`);
    this.reportsService.createInvoices(this.buildInvoicesCreationInfo()).subscribe(
      data => {
        console.log(`InvoiceCreateFormComponent.createInvoices(): data is ${data}.`);
        this.ngxSpinnerService.hide();
        this.ngbActiveModal.close('Invoices created');
      },
      error => {
        console.log(`InvoiceCreateFormComponent.createInvoices(): error is ${error}.`);
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

  setSelectedSchoolYear(year: string): void {
    this.selectedSchoolYear = year;
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
      name: this.selectedSchoolYear + '_INVOICE_' + this.selectedSchoolDistrict.name,
      schoolYear: this.selectedSchoolYear.replace(/\s+/g, ''),
      templateId: this.selectedInvoiceTemplate.id,
      invoice: {
        asOf: new Date(`${this.asOfDate.month}/${this.asOfDate.day}/${this.asOfDate.year}`),
        toSchoolDistrict: new Date(`${this.toSchoolDistrictDate.month}/${this.toSchoolDistrictDate.day}/${this.toSchoolDistrictDate.year}`),
        toPDE: new Date(`${this.toPDEDate.month}/${this.toPDEDate.day}/${this.toPDEDate.year}`),
        schoolDistrictAun: +this.selectedSchoolDistrict.aun,
        studentsTemplateId: null
      }
    };
  }

  private buildInvoicesCreationInfo(): Object {
    return {
      reportType: ReportType.Invoice,
      schoolYear: this.selectedSchoolYear.replace(/\s+/g, ''),
      templateId: this.selectedInvoiceTemplate.id,
      invoice: {
        asOf: new Date(`${this.asOfDate.month}/${this.asOfDate.day}/${this.asOfDate.year}`),
        toSchoolDistrict: new Date(`${this.toSchoolDistrictDate.month}/${this.toSchoolDistrictDate.day}/${this.toSchoolDistrictDate.year}`),
        toPDE: new Date(`${this.toPDEDate.month}/${this.toPDEDate.day}/${this.toPDEDate.year}`)
      }
    };
  }
}

import { Component, OnInit } from '@angular/core';

import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

import { Report, ReportType } from '../../../models/report.model';
import { Template } from '../../../models/template.model';

import { ReportsService } from '../../../services/reports.service';
import { UtilitiesService } from '../../../services/utilities.service';
import { AcademicYearsService } from '../../../services/academic-years.service';
import { TemplatesService } from '../../../services/templates.service';

import { Globals } from '../../../globals';

@Component({
  selector: 'app-invoice-create-form',
  templateUrl: './invoice-create-form.component.html',
  styleUrls: ['./invoice-create-form.component.scss']
})
export class InvoiceCreateFormComponent implements OnInit {
  private schoolYear: string;
  private invoiceTemplate: string;
  private asOfDate: Date;
  private toSchoolDistrictDate: Date;
  private toPDEDate: Date;
  private studentTemplate: string;
  private selectedSchoolYear: string;
  private schoolYears: string[];
  private templates: Template[];
  private selectedTemplate: string;
  private skip: number;

  constructor(
    private globals: Globals,
    private reportsService: ReportsService,
    private utilitiesService: UtilitiesService,
    private academicYearsService: AcademicYearsService,
    private templatesService: TemplatesService,
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
  }

  createInvoices() {
    console.log(`InvoiceCreateFormComponent.createReport(): whatever.`);
  }

  getStudentTemplates() {
    if (this.templates) {
      return this.templates.filter((t) => t.reportType === ReportType.StudentInformation);
    }
  }

  getInvoiceTemplates() {
    if (this.templates) {
      return this.templates.filter((t) => t.reportType === ReportType.Invoice);
    }
  }

  setSelectedSchoolYear(year: string) {
    this.selectedSchoolYear = year;
  }

  onAsOfDateChanged() {
  }

  onIssuedSchoolDistrictDateChanged() {
  }

  onIssuedPDEDateChanged() {
  }
}

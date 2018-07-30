import { Component, Input, OnInit } from '@angular/core';

import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

import { Report, ReportType } from '../../../models/report.model';

import { ReportsService } from '../../../services/reports.service';
import { ExcelService } from '../../../services/excel.service';

@Component({
  selector: 'app-invoice-preview-form',
  templateUrl: './invoice-preview-form.component.html',
  styleUrls: ['./invoice-preview-form.component.scss']
})
export class InvoicePreviewFormComponent implements OnInit {
  private xlsxData;
  private jsonData;
  private currentInvoice: Report;
  private invoiceIdx;

  @Input() invoices;

  constructor(
    private ngbActiveModal: NgbActiveModal,
    private reportsService: ReportsService,
    private excelService: ExcelService
  ) { }

  ngOnInit() {
    if (this.invoices) {
      this.invoiceIdx = 0;
      this.currentInvoice = this.invoices[this.invoiceIdx];
    }
  }

  downloadStudentActivityData(): void {
    this.reportsService.getInvoiceStudentActivityDataByName(this.currentInvoice.name).subscribe(
      data => {
        console.log('InvoicePreviewFormComponent.getCurrentInvoiceData(): data is ', data);
        this.currentInvoice.xlsx = data;
        this.excelService.saveStudentActivityAsExcelFile(data, this.currentInvoice);
        this.ngbActiveModal.close("Successful download");
      },
      error => {
        console.log('InvoicePreviewFormComponent.getCurrentInvoiceData(): error is ', error);
      }
    );
  }

  approveInvoice(): void {
    this.invoices[this.invoiceIdx++].approved = true;
  }

  rejectInvoice(): void {
    this.invoiceIdx++;
  }
}

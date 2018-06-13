import { Component, Input, OnInit } from '@angular/core';

import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-invoice-preview-form',
  templateUrl: './invoice-preview-form.component.html',
  styleUrls: ['./invoice-preview-form.component.scss']
})
export class InvoicePreviewFormComponent implements OnInit {
  private xlsxData;
  private currentInvoice;
  private invoiceIdx;

  @Input() invoices;

  constructor(
    private ngbActiveModal: NgbActiveModal
  ) { }

  ngOnInit() {
    if (this.invoices) {
      this.xlsxData = this.invoices[0].xlsx;
      this.invoiceIdx = 0;
    }
  }

  approveInvoice() {
    this.invoices[this.invoiceIdx++].approved = true;
  }
}

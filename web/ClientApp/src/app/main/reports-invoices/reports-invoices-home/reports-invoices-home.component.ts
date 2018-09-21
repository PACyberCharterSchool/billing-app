import { Component, OnInit } from '@angular/core';

import { Router } from '@angular/router';

enum BUTTON_IDS {
  InvoicesBtn = 'invoices-btn',
  DigitalSignaturesBtn = 'digital-signatures-btn',
  AgingReportsBtn = 'aging-reports-btn',
  AsOfDateReportsBtn = 'accounts-receivable-aod-report-btn',
  CSIUExportBtn = 'csiu-btn',
  ItemizedStudentActivityBtn = 'itemized-student-activity-btn',
  MonthlyCombinedInvoiceBtn = 'monthly-combined-invoice-btn'
}

@Component({
  selector: 'app-reports-invoices-home',
  templateUrl: './reports-invoices-home.component.html',
  styleUrls: ['./reports-invoices-home.component.scss']
})
export class ReportsInvoicesHomeComponent implements OnInit {

  constructor(private router: Router) { }

  ngOnInit() {
  }

  handleClick(event) {
    const target = event.currentTarget;
    const idAttr = target.attributes.id;
    const id = idAttr.value;

    if (id) {
      this.handleInvoicesReportsHomeBtnSelection(id);
    }
  }

  handleInvoicesReportsHomeBtnSelection(id: string) {
    switch (id) {
      case BUTTON_IDS.InvoicesBtn:
        this.router.navigate(['/reports-invoices', { outlets: { 'action': ['invoices'] } }]);
        break;
      case BUTTON_IDS.DigitalSignaturesBtn:
        this.router.navigate(['/reports-invoices', { outlets: { 'action': ['digital-signatures'] } }]);
        break;
      case BUTTON_IDS.AgingReportsBtn:
        this.router.navigate(['/reports-invoices', { outlets: { 'action': ['aging-reports'] } }]);
        break;
      case BUTTON_IDS.AsOfDateReportsBtn:
        this.router.navigate(['/reports-invoices', { outlets: { 'action': ['as-of-date-reports'] } }]);
        break;
      case BUTTON_IDS.CSIUExportBtn:
        this.router.navigate(['/reports-invoices', { outlets: { 'action': ['csiu-reports'] } }]);
        break;
      case BUTTON_IDS.ItemizedStudentActivityBtn:
        this.router.navigate(['/reports-invoices', { outlets: { 'action': ['itemized-student-activity'] } }]);
        break;
      case BUTTON_IDS.MonthlyCombinedInvoiceBtn:
        this.router.navigate(['/reports-invoices', { outlets: { 'action': ['invoices-monthly-combined'] } }]);
        break;
      default:
        this.router.navigate(['/reports-invoices', { outlets: { 'action': ['home'] } }]);
        break;
    }
  }

}

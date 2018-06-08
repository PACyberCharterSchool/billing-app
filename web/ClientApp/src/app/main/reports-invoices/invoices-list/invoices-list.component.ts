import { Component, OnInit } from '@angular/core';

import { Invoice } from '../../../models/invoice.model';

@Component({
  selector: 'app-invoices-list',
  templateUrl: './invoices-list.component.html',
  styleUrls: ['./invoices-list.component.scss']
})
export class InvoicesListComponent implements OnInit {
  private invoices: Invoice[];
  private allInvoices: Invoice[];

  constructor() { }

  ngOnInit() {
  }

  listDisplayableFields() {

  }

  listDisplayableValues() {

  }
}

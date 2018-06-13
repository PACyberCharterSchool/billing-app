import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-invoice-excel-preview',
  templateUrl: './invoice-excel-preview.component.html',
  styleUrls: ['./invoice-excel-preview.component.scss']
})
export class InvoiceExcelPreviewComponent implements OnInit {

  @Input() xlsxData;

  constructor() { }

  ngOnInit() {
  }

}

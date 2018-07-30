import { Component, Input, OnInit } from '@angular/core';

import * as XLSX from 'xlsx';

@Component({
  selector: 'app-invoice-excel-preview',
  templateUrl: './invoice-excel-preview.component.html',
  styleUrls: ['./invoice-excel-preview.component.scss']
})
export class InvoiceExcelPreviewComponent implements OnInit {
  private worksheet;
  private html;

  @Input() xlsxData;
  @Input() jsonData;

  constructor() { }

  ngOnInit() {
    if (this.jsonData) {
      const json = JSON.parse(this.jsonData);
      this.worksheet = XLSX.utils.json_to_sheet(json);
      this.html = XLSX.utils.sheet_to_html(this.worksheet);
    }

    if (this.xlsxData) {
      this.worksheet = XLSX.utils.book_new();
    }
  }

}

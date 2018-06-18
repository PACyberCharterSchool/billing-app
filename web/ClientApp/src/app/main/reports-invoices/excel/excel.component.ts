import { Component, Input, OnInit, AfterViewInit } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';

import * as Handsontable from 'handsontable';
import * as XLSX from 'xlsx';

import { HotTableRegisterer } from '@handsontable/angular';

const CONTENT_TYPE_WORKSHEET = 'worksheet';
const CONTENT_TYPE_HTML = 'html';

@Component({
  selector: 'app-excel',
  templateUrl: './excel.component.html',
  styleUrls: ['./excel.component.scss']
})
export class ExcelComponent implements OnInit {
  private table;
  private data;
  private contentType: string;
  private settings: Handsontable.GridSettings;

  @Input() html;
  @Input() worksheet;

  constructor(
    private sanitizer: DomSanitizer,
    private hotRegisterer: HotTableRegisterer
  ) { }

  ngOnInit() {
    this.setContentType();
  }

  ngAfterViewInit() {
    this.table = document.getElementById('spreadsheetContainer');
    if (this.isHTMLContent()) {
      this.generateHTMLContent();
    }
    else {
      this.generateWorksheetContent();
    }
  }

  private generateHTMLContent(): void {
    this.data = this.sanitizer.bypassSecurityTrustHtml(this.html);
  }

  private generateWorksheetContent(): void {
    console.log('ExcelComponent.generateWorksheetContent():  worksheet is .', this.worksheet);
    console.log('ExcelComponent.generateWorksheetContent():  data is .', this.data);
    this.data = Handsontable.helper.createSpreadsheetData(10, 10);
    this.settings = {
      data: this.data,
      rowHeaders: false,
      colHeaders: this.data[0],
      stretchH: 'all'
    };
    let hot = new Handsontable(this.table, { data: this.data, rowHeaders: true, colHeaders: false });
    hot.render();
  }

  private setContentType(): void {
    if (this.worksheet) {
      this.contentType = CONTENT_TYPE_WORKSHEET;
    }
    else {
      this.contentType = CONTENT_TYPE_HTML;
    }
  }

  public isWorksheetContent(): boolean {
    return this.contentType === CONTENT_TYPE_WORKSHEET;
  }

  public isHTMLContent(): boolean {
    return this.contentType === CONTENT_TYPE_HTML;
  }
}

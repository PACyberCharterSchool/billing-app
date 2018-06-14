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
    // this.data = XLSX.utils.sheet_to_json(this.worksheet);
    // this.data = this.worksheet;
    // this.data = [
    //   {a: 1, b: 2, c: 3, d: 4},
    //   {e: 5, f: 6, g: 7, h: 8}
    // ];
    // this.data = Object.values(this.worksheet);
    // this.data = [
    //   ['one', 'two', 'three', 'four'],
    //   [ 1, 2, 3, 4 ],
    //   [ 5, 6, 7, 8 ]
    // ];
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

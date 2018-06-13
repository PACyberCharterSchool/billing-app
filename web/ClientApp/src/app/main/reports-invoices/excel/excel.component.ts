import { Component, Input, OnInit } from '@angular/core';

import * as XLSX from 'xlsx';

@Component({
  selector: 'app-excel',
  templateUrl: './excel.component.html',
  styleUrls: ['./excel.component.scss']
})
export class ExcelComponent implements OnInit {
  @Input() xlsxData;

  constructor() { }

  ngOnInit() {
  }

}

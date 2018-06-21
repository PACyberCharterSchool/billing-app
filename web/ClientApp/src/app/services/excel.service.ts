import { Injectable } from '@angular/core';
import * as FileSaver from 'file-saver';

import { Report } from '../models/report.model';

const EXCEL_TYPE = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
const CSV_TYPE = 'application/text';
const EXCEL_EXTENSION = '.xlsx';
const CSV_EXTENSION = '.csv';
const INVOICE = '_INVOICE';
const STUDENT_ACTIVITY = '_STUDENT_ACTIVITY';

@Injectable()
export class ExcelService {

  constructor(
  ) { }

  public saveInvoiceAsExcelFile(invoice: Report): void {
    const data = invoice.xlsx;
    this.saveAsExcelFile(data, this.generateInvoiceExcelFileName(invoice));
  }

  public saveStudentActivityAsExcelFile(report: Report): void {
    const data = report.xlsx;
    this.saveAsExcelFile(data, this.generateStudentActivityExcelFileName(report));
  }

  private saveAsExcelFile(buffer: any, fileName: string): void {
    const data: Blob = new Blob([buffer], {
      type: EXCEL_TYPE
    });

    FileSaver.saveAs(data, fileName);
  }

  private generateInvoiceExcelFileName(invoice: Report): string {
    return invoice.name + invoice.schoolYear + INVOICE + EXCEL_EXTENSION;
  }

  private generateStudentActivityExcelFileName(invoice: Report): string {
    return invoice.name + invoice.schoolYear + STUDENT_ACTIVITY + EXCEL_EXTENSION;
  }

  /* generate a download */
  private s2ab(s: any) {
    var buf = new ArrayBuffer(s.length);
    var view = new Uint8Array(buf);
    for (var i=0; i!=s.length; ++i) view[i] = s.charCodeAt(i) & 0xFF;
    return buf;
  }
}

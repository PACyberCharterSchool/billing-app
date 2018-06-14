import { Injectable } from '@angular/core';
import * as XLSX from 'xlsx';
import * as FileSaver from 'file-saver';

import { Report } from '../models/report.model';

const EXCEL_TYPE = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
const CSV_TYPE = 'application/text';
const EXCEL_EXTENSION = '.xlsx';
const CSV_EXTENSION = '.csv';
const INVOICE = '_INVOICE';

@Injectable()
export class ExcelService {

  constructor(
  ) { }

  public exportAsExcelFile(json: any[], excelFileName: string): void {
    const worksheet: XLSX.WorkSheet = XLSX.utils.json_to_sheet(json);
    const workbook: XLSX.WorkBook = { Sheets: { 'data': worksheet }, SheetNames: ['data'] };
    // const excelBuffer: any = XLSX.write(workbook, { bookType: 'xlsx', type: 'buffer' });
    const excelBuffer: any = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });
    this.saveAsExcelFile(excelBuffer, excelFileName);
  }

  public exportAsCSVFile(json: any[], csvFileName: string): void {
    const worksheet: XLSX.WorkSheet = XLSX.utils.json_to_sheet(json);
    const csvBuffer = XLSX.utils.sheet_to_csv(worksheet);
    // const workbook: XLSX.WorkBook = { Sheets: { 'data': worksheet }, SheetNames: ['data'] };
    // const csvBuffer: any = XLSX.write(workbook, { bookType: 'xlsx', type: 'buffer' });
    this.saveAsCSVFile(csvBuffer, csvFileName);
  }

  public saveInvoiceAsExcelFile(invoice: Report): void {
    const data = JSON.parse(invoice.data);
    this.exportAsExcelFile(data, this.generateInvoiceExcelFileName(invoice));
  }

  public saveInvoiceAsCSVFile(invoice: Report): void {
    const data = JSON.parse(invoice.data);
    this.exportAsCSVFile(data, this.generateInvoiceCSVFileName(invoice));
  }

  private saveAsExcelFile(buffer: any, fileName: string): void {
    const data: Blob = new Blob([buffer], {
      type: EXCEL_TYPE
    });

    FileSaver.saveAs(data, fileName);
  }

  private saveAsCSVFile(buffer: any, fileName: string): void {
    const data: Blob = new Blob([this.s2ab(buffer)], {
      type: CSV_TYPE
    });

    FileSaver.saveAs(data, fileName);
  }

  private generateInvoiceExcelFileName(invoice: Report): string {
    return invoice.name + invoice.schoolYear + INVOICE + EXCEL_EXTENSION;
  }

  private generateInvoiceCSVFileName(invoice: Report): string {
    return invoice.name + invoice.schoolYear + INVOICE + CSV_EXTENSION;
  }

  /* generate a download */
  private s2ab(s: any) {
    var buf = new ArrayBuffer(s.length);
    var view = new Uint8Array(buf);
    for (var i=0; i!=s.length; ++i) view[i] = s.charCodeAt(i) & 0xFF;
    return buf;
  }
}

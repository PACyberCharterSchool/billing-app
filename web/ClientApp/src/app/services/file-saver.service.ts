import { Injectable } from '@angular/core';
import * as FileSaver from 'file-saver';

import { Report } from '../models/report.model';

const EXCEL_TYPE = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
const PDF_TYPE = 'application/pdf';
const CSV_TYPE = 'application/text';
const EXCEL_EXTENSION = '.xlsx';
const PDF_EXTENSION = '.pdf';
const CSV_EXTENSION = '.csv';
const INVOICE = '_INVOICE';
const STUDENT_ACTIVITY = '_STUDENT_ACTIVITY';

@Injectable()
export class FileSaverService {

  constructor(
  ) { }

  public saveInvoiceAsExcelFile(buffer: any, invoice: Report): void {
    this.saveAsExcelFile(buffer, this.generateInvoiceExcelFileName(invoice));
  }

  public saveStudentActivityAsExcelFile(buffer: any, invoice: Report): void {
    this.saveAsExcelFile(buffer, this.generateStudentActivityExcelFileName(invoice));
  }

  public saveStudentActivityAsPDFFile(buffer: any, invoice: Report): void {
    this.saveAsPDFFile(buffer, this.generateStudentActivityPDFFileName(invoice));
  }

  public saveInvoiceAsPDFFile(buffer: any, invoice: Report): void {
    this.saveAsPDFFile(buffer, this.generateInvoicePDFFileName(invoice));
  }

  public saveDataAsExcelFile(buffer: any, fileName: string): void {
    this.saveAsExcelFile(buffer, fileName);
  }

  private saveAsExcelFile(buffer: any, fileName: string): void {
    const data: Blob = new Blob([buffer], {
      type: EXCEL_TYPE
    });

    FileSaver.saveAs(data, fileName);
  }

  private generateInvoiceExcelFileName(invoice: Report): string {
    return invoice.name + EXCEL_EXTENSION;
  }

  private generateStudentActivityExcelFileName(invoice: Report): string {
    return invoice.name + EXCEL_EXTENSION;
  }

  private generateInvoicePDFFileName(invoice: Report): string {
    return invoice.name + PDF_EXTENSION;
  }

  private generateStudentActivityPDFFileName(invoice: Report): string {
    return invoice.name + PDF_EXTENSION;
  }

  private saveAsPDFFile(buffer: any, fileName: string): void {
    const data: Blob = new Blob([buffer], {
      type: PDF_TYPE
    });

    FileSaver.saveAs(data, fileName);
  }
}

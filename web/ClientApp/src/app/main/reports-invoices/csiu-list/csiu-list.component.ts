import { Component, OnInit } from '@angular/core';

import { Report } from '../../../models/report.model';
import { SchoolDistrict } from '../../../models/school-district.model';

import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NgxSpinnerService } from 'ngx-spinner';

import { UtilitiesService } from '../../../services/utilities.service';
import { ReportsService } from '../../../services/reports.service';
import { FileSaverService } from '../../../services/file-saver.service';
import { AcademicYearsService } from '../../../services/academic-years.service';
import { SchoolDistrictService } from '../../../services/school-district.service';

@Component({
  selector: 'app-csiu-list',
  templateUrl: './csiu-list.component.html',
  styleUrls: ['./csiu-list.component.scss']
})
export class CsiuListComponent implements OnInit {
  public reports: Report[];
  public allReports: Report[];
  private skip: number;
  public property: string;
  public direction: number;
  private isDescending: boolean;
  public searchText: string;
  public asOfDate;
  public selectedAcademicYear: string;
  public schoolYears: string[];
  public selectedDownloadFormat: string;
  public downloadFormats: string[] = [
    'Microsoft Excel',
    'PDF'
  ];
  public spinnerMsg: string;
  public auns: number[];
  private schoolDistricts: SchoolDistrict[];

  constructor(
    private ngbModalService: NgbModal,
    private ngxSpinnerService: NgxSpinnerService,
    private utilitiesService: UtilitiesService,
    private reportsService: ReportsService,
    private fileSaverService: FileSaverService,
    private academicYearsService: AcademicYearsService,
    private schoolDistrictService: SchoolDistrictService
  ) { }

  ngOnInit() {
    this.skip = 0;
    this.spinnerMsg = '';
    this.schoolYears = this.academicYearsService.getAcademicYears();

    this.schoolDistrictService.getSchoolDistricts().subscribe(
      data => {
        console.log('CsiuListComponent.ngOnInit():  data is ', data);
        this.schoolDistricts = data['schoolDistricts'];
      },
      error => {
        console.log('CsiuListComponent.ngOnInit():  error is ', error);
      }
    );

    this.refreshCSIUList();
  }

  public filterCSIUList(): void {
  }

  public refreshCSIUList(): void {
    this.reportsService.getCSIU().subscribe(
      data => {
        this.allReports = this.reports = data['reports'];
        console.log('CsiuListComponent.ngOnInit():  reports are ', data['reports']);
      },
      error => {
        console.log('CsiuListComponent.ngOnInit():  error is ', error);
      }
    );
  }

  sort(property) {
    this.isDescending = !this.isDescending; // change the direction
    this.property = property;
    this.direction = this.isDescending ? 1 : -1;
  }

  listDisplayableFields() {
    if (this.allReports) {
      const fields = this.utilitiesService.objectKeys(this.allReports[0]);
      const rejected = ['data', 'xlsx', 'type', 'id', 'approved', 'scope'];
      return fields.filter((i) => !rejected.includes(i));
    }
  }

  listDisplayableValues(report: Report) {
    const vkeys = this.listDisplayableFields();
    const selected = this.utilitiesService.pick(report, vkeys);

    return this.utilitiesService.objectValues(selected);
  }

  public displayCreateCSIUDialog(createContent): void {
    const modal = this.ngbModalService.open(createContent, { centered: true });
    modal.result.then(
      (result) => {
      },
      (reason) => {
      }
    );
  }

  public displayDownloadCSIUFormatDialog(downloadContent, report: Report): void {
    const modal = this.ngbModalService.open(downloadContent, { centered: true });
    modal.result.then(
      (result) => {
        this.downloadReportByFormat(report, this.selectedDownloadFormat);
      },
      (reason) => {
        console.log('CsiuListComponent.displayDownloadCSIUFormatDialog():  reason is ', reason);
      }
    );
  }

  public onDateChanged($event): void {
  }

  private generateCSIUReportName(): string {
    const d: Date = new Date(this.asOfDate.year, this.asOfDate.month + 1, this.asOfDate.day);
    return `CSIU_${d.getMonth() - 1}${d.getFullYear()}`;
  }

  public onCreateSubmit(): void {
    this.spinnerMsg = `Generating CSIU report for ${this.asOfDate.month + 1}/${this.asOfDate.year}.  Please wait...`;
    this.ngxSpinnerService.show();
    this.reportsService.createCSIU(
      this.generateCSIUReportName(),
      // this.selectedAcademicYear.replace(/\s+/g, ''),
      new Date(this.asOfDate.year, this.asOfDate.month - 1, this.asOfDate.day),
      this.schoolDistricts.map((sd) => +sd.aun)).subscribe(
        data => {
          console.log('CsiuListComponent.onCreateSubmit():  data is ', data);
          this.reports = this.allReports = data['reports'];
          this.ngxSpinnerService.hide();
          this.refreshCSIUList();
        },
        error => {
          console.log('CsiuListComponent.onCreateSubmit():  error is ', error);
          this.ngxSpinnerService.hide();
        }
      );
  }

  public downloadReportByFormat(report: Report, format: string): void {
    this.ngxSpinnerService.show();
    this.reportsService.getReportDataByFormat(report, format.includes('Microsoft Excel') ? 'excel' : 'pdf').subscribe(
      data => {
        console.log('AccountsReceivableAsOfComponent.downloadReportByFormat():  data is ', data);
        this.ngxSpinnerService.hide();
        if (format.toLowerCase().includes('excel')) {
          this.fileSaverService.saveInvoiceAsExcelFile(data, report);
        } else {
          this.fileSaverService.saveInvoiceAsPDFFile(data, report);
        }
      },
      error => {
        this.ngxSpinnerService.hide();
        console.log('AccountsReceivableAsOfComponent.downloadReportByFormat():  error is ', error);
      }
    );
  }

  public setSelectedAcademicYear(year: string): void {
    this.selectedAcademicYear = year;
  }

  public setSelectedDownloadFormat(format: string): void {
    this.selectedDownloadFormat = format;
  }
}

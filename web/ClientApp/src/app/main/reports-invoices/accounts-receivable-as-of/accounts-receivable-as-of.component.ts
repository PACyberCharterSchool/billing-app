import { Component, OnInit } from '@angular/core';

import { Report } from '../../../models/report.model';

import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NgxSpinnerService } from 'ngx-spinner';

import { UtilitiesService } from '../../../services/utilities.service';
import { ReportsService } from '../../../services/reports.service';
import { FileSaverService } from '../../../services/file-saver.service';

@Component({
  selector: 'app-accounts-receivable-as-of',
  templateUrl: './accounts-receivable-as-of.component.html',
  styleUrls: ['./accounts-receivable-as-of.component.scss']
})
export class AccountsReceivableAsOfComponent implements OnInit {
  public reports: Report[];
  public allReports: Report[];
  private skip: number;
  public property: string;
  public direction: number;
  private isDescending: boolean;
  public searchText: string;
  public asOfDate;

  constructor(
    private ngbModalService: NgbModal,
    private ngxSpinnerService: NgxSpinnerService,
    private utilitiesService: UtilitiesService,
    private reportsService: ReportsService,
    private fileSaverService: FileSaverService
  ) { }

  ngOnInit() {
    this.skip = 0;
    this.reportsService.getAccountsReceivableAsOf().subscribe(

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
      const rejected = ['data', 'xlsx', 'type', 'id'];
      return fields.filter((i) => !rejected.includes(i));
    }
  }

  listDisplayableValues(report: Report) {
    const vkeys = this.listDisplayableFields();
    const selected = this.utilitiesService.pick(report, vkeys);

    return this.utilitiesService.objectValues(selected);
  }

  public displayCreateAccountsReceivableAsOfDialog(createDialogContent): void {
    const modal = this.ngbModalService.open(createDialogContent, { centered: true });
    modal.result.then(
      (result) => {
      },
      (reason) => {
      }
    );
  }

  public onSubmit(): void {
  }
}

import { Component, OnInit } from '@angular/core';

import { UtilitiesService } from '../../../services/utilities.service';
import { PaymentsService } from '../../../services/payments.service';
import { SchoolDistrictService } from '../../../services/school-district.service';

import { Payment } from '../../../models/payment.model';
import { SchoolDistrict } from '../../../models/school-district.model';

import { PaymentUpsertFormComponent } from '../payment-upsert-form/payment-upsert-form.component';

import { Globals } from '../../../globals';

import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-payments-list',
  templateUrl: './payments-list.component.html',
  styleUrls: ['./payments-list.component.scss']
})
export class PaymentsListComponent implements OnInit {
  public searchText: string;
  private direction: number;
  private property: string;
  private isDescending: boolean;
  private allPayments: Payment[];
  public payments: Payment[];
  private schoolDistricts: SchoolDistrict[];
  private skip: number;
  private selectedBulkImportFile;
  public spinnerMsg: string;

  constructor(
    private globals: Globals,
    private utilitiesService: UtilitiesService,
    private paymentsService: PaymentsService,
    private schoolDistrictsService: SchoolDistrictService,
    private ngxSpinnerService: NgxSpinnerService,
    private ngbModalService: NgbModal
  ) {
  }

  ngOnInit() {
    this.property = 'schoolDistrictName';
    this.direction = 1;
    this.skip = 0;

    this.refreshPaymentList();

    this.schoolDistrictsService.getSchoolDistricts().subscribe(
      data => {
        this.schoolDistricts = data['schoolDistricts'];
        console.log('PaymentsListComponent.ngOnInit():  school district list is ', this.schoolDistricts);
      },
      error => {
        console.log('PaymentsListComponent.ngOnInit():  error is ', error);
      }
    );
  }

  isSplitPayment(payment: Payment): boolean {
    return this.allPayments.filter((p) => p.paymentId === payment.paymentId).length > 1;
  }

  getPaymentAmount(payment: Payment): number {
    const payments: Payment[] = this.allPayments.filter((p) => p.paymentId === payment.paymentId);
    let sum = 0;

    if (payments.length > 1) {
      sum = payments.reduce((acc, p) => acc + p.amount, 0);
    } else {
      sum = payments[0].amount;
    }

    return sum;
  }

  sort(property) {
    this.isDescending = !this.isDescending; // change the direction
    this.property = property;
    this.direction = this.isDescending ? 1 : -1;
  }

  filterPaymentRecords() {
    this.payments = this.allPayments.filter(
      (i) => {
        const re = new RegExp(this.searchText, 'gi');
        if (
          i.externalId.toString().search(re) !== -1 ||
          i.schoolDistrict.aun.toString().search(re) !== -1 ||
          i.schoolDistrict.name.search(re) !== -1 ||
          i.type.search(re) !== -1 ||
          i.paymentId.search(re) !== -1
        ) {
          return true;
        }
        return false;
      }
    ).filter((p) => p.split === 1);
    console.log('PaymentsListComponent.filterPaymentRecords():  payments is ', this.payments);
  }

  resetPaymentRecords() {
    this.payments = this.allPayments;
    this.searchText = '';
    this.skip = 0;
    this.refreshPaymentList();
  }

  refreshPaymentList() {
    this.spinnerMsg = 'Loading payments list.  Please wait...';
    this.ngxSpinnerService.show();
    this.paymentsService.getPayments(this.skip).subscribe(
      data => {
        this.allPayments = data['payments'];
        this.payments = data['payments'].filter((p) => p.split === 1);
        this.ngxSpinnerService.hide();
        console.log('PaymentsListComponent.ngOnInit(): payments are ', this.allPayments);
      },
      error => {
        console.log('PaymentsListComponent.ngOnInit(): error is ', error);
        this.ngxSpinnerService.hide();
      }
    );
  }

  getAdditionalPayments($event) {
    this.paymentsService.getPayments(this.skip).subscribe(
      data => {
        if (data['payments'].length > 0) {
          this.allPayments = this.allPayments.concat(data['payments']);
          this.payments = this.payments.concat(data['payments'].filter((p) => p.split === 1));
          this.updateScrollingSkip(data['payments'].length > this.globals.take ? this.globals.take : data['payments'].length);
        }

        console.log('PaymentsListComponent.getPayments():  payments are ', this.payments);
      },
      error => {
        console.log('PaymentsListComponent.getPayments():  error is ', error);
      }
    );
  }

  createPayment() {
    const modal = this.ngbModalService.open(PaymentUpsertFormComponent, { centered: true, size: 'lg' });
    modal.componentInstance.op = 'create';
    modal.componentInstance.schoolDistricts = this.schoolDistricts;

    modal.result.then(
      (result) => {
        console.log('PaymentsListComponent.createPayment():  result is ', result);
        this.refreshPaymentList();
      },
      (reason) => {
        console.log('PaymentsListComponent.createPayment():  reason is ', reason);
      }
    );
  }

  editPayment(p: Payment) {
    const modal = this.ngbModalService.open(PaymentUpsertFormComponent, { centered: true, size: 'lg' });
    modal.componentInstance.op = 'update';
    modal.componentInstance.schoolDistricts = this.schoolDistricts;
    modal.componentInstance.paymentRecord = p;

    modal.result.then(
      (result) => {
        console.log('PaymentsListComponent.editPayment():  result is ', result);
        this.refreshPaymentList();
      },
      (reason) => {
        console.log('PaymentsListComponent.editPayment():  result is ', reason);
      }
    );
  }

  onScroll($event) {
    // this.getAdditionalPayments($event);
  }

  listDisplayableFields() {
    if (this.allPayments) {
      const fields = this.utilitiesService.objectKeys(this.allPayments[0]);
      const rejected = ['id', 'paymentId', 'type'];
      return fields.filter((i) => !rejected.includes(i));
    }
  }

  listDisplayableValues(payment: Payment) {
    if (payment) {
      const vkeys = this.listDisplayableFields();
      const selected = this.utilitiesService.pick(payment, vkeys);

      return this.utilitiesService.objectValues(selected);
    }
  }

  public importPDEPayments(dlgContent): void {
    this.ngbModalService.open(dlgContent, { centered: true }).result.then(
      (result) => {
        this.refreshPaymentList();
      },
      (reason) => {
        console.log('AdministrationTemplateListComponent.importTemplate():  reason is ', reason);
      }
    );
  }

  public setImportPaymentsUrl($event): void {
    if ($event) {
      if ($event.target.files && $event.target.files.length > 0) {
        this.selectedBulkImportFile = $event.target.files;
      }
    }
  }

  doImport(): void {
    if (this.selectedBulkImportFile) {
      const importData = new FormData();

      importData.append(
        'file',
        this.selectedBulkImportFile[0],
      );

      this.ngxSpinnerService.show();
      this.paymentsService.updatePDEPayments(importData).subscribe(
        data => {
          console.log('PaymentListComponent.doImport():  ', data['schoolDistricts']);
          this.ngxSpinnerService.hide();
          this.refreshPaymentList();
        },
        error => {
          console.log('PaymentListComponent.doImport():  ', error);
          this.ngxSpinnerService.hide();
          this.refreshPaymentList();
        }
      );
    }
  }

  private updateScrollingSkip(updateSkip: number) {
    this.skip += updateSkip;
  }
}

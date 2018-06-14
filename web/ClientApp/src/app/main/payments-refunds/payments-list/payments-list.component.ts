import { Component, OnInit } from '@angular/core';

import { UtilitiesService } from '../../../services/utilities.service';
import { PaymentsService } from '../../../services/payments.service';
import { SchoolDistrictService } from '../../../services/school-district.service';

import { Payment } from '../../../models/payment.model';
import { SchoolDistrict } from '../../../models/school-district.model';

import { NormalizeFieldNamePipe } from '../../../pipes/normalize-field-name.pipe';

import { PaymentUpsertFormComponent } from '../payment-upsert-form/payment-upsert-form.component';

import { Globals } from '../../../globals';

import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-payments-list',
  templateUrl: './payments-list.component.html',
  styleUrls: ['./payments-list.component.scss']
})
export class PaymentsListComponent implements OnInit {
  private searchText: string;
  private direction: number;
  private property: string;
  private isDescending: boolean;
  private allPayments: Payment[];
  private payments: Payment[];
  private schoolDistricts: SchoolDistrict[];
  private skip: number;

  constructor(
    private globals: Globals,
    private utilitiesService: UtilitiesService,
    private paymentsService: PaymentsService,
    private schoolDistrictsService: SchoolDistrictService,
    private ngbModalService: NgbModal
  ) {
    this.property = 'schoolDistrictName';
    this.direction = 1;
    this.skip = 0;
  }

  ngOnInit() {
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
          i.type.search(re) !== -1 ||
          i.paymentId.search(re) !== -1
        ) {
          return true;
        }
        return false;
      }
    );
    console.log('PaymentsListComponent.filterPaymentRecords():  payments is ', this.payments);
  }

  resetPaymentRecords() {
    this.payments = this.allPayments;
  }

  refreshPaymentList() {
    this.paymentsService.getPayments(this.skip).subscribe(
      data => {
        this.allPayments = this.payments = data['payments'];
        console.log('PaymentsListComponent.ngOnInit(): payments are ', this.allPayments);
      },
      error => {
        console.log('PaymentsListComponent.ngOnInit(): error is ', error);
      }
    );
  }

  getAdditionalPayments($event) {
    this.paymentsService.getPayments(this.skip).subscribe(
      data => {
        this.payments = this.payments.concat(data['students']);
        this.updateScrollingSkip();
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
    this.getAdditionalPayments($event);
  }

  listDisplayableFields() {
    if (this.allPayments) {
      const fields = this.utilitiesService.objectKeys(this.allPayments[0]);
      const rejected = ['id'];
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

  private updateScrollingSkip() {
    this.skip += this.globals.take;
  }

}

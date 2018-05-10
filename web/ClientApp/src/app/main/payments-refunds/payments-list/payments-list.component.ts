import { Component, OnInit } from '@angular/core';

import { UtilitiesService } from '../../../services/utilities.service';

import { Payment } from '../../../models/payment.model';

import { NormalizeFieldNamePipe } from '../../../pipes/normalize-field-name.pipe';

import { PaymentUpsertFormComponent } from '../payment-upsert-form/payment-upsert-form.component';

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

  constructor(
    private utilitiesService: UtilitiesService,
    private ngbModalService: NgbModal
  ) {
    this.property = 'schoolDistrictName';
    this.direction = 1;
    this.payments = this.allPayments;
  }

  ngOnInit() {
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
        if (+i.schoolDistrictId.search(re) !== -1 || i.schoolDistrictName.search(re) !== -1 || i.schoolDistrictId.search(re) !== -1) {
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

  createPayment(content) {
    this.ngbModalService.open(PaymentUpsertFormComponent, { centered: true }).result.then(
      (result) => {
        console.log('PaymentsListComponent.createPayment():  result is ', result);
      },
      (reason) => {
        console.log('PaymentsListComponent.createPayment():  reason is ', reason);
      }
    );
  }

  editPayment(p: Payment) {

  }
}

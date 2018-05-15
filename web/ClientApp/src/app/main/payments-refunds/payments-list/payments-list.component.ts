import { Component, OnInit } from '@angular/core';

import { UtilitiesService } from '../../../services/utilities.service';
import { PaymentsService } from '../../../services/payments.service';
import { SchoolDistrictService } from '../../../services/school-district.service';

import { Payment } from '../../../models/payment.model';
import { SchoolDistrict } from '../../../models/school-district.model';

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
  private schoolDistricts: SchoolDistrict[];

  constructor(
    private utilitiesService: UtilitiesService,
    private paymentsService: PaymentsService,
    private schoolDistrictsService: SchoolDistrictService,
    private ngbModalService: NgbModal
  ) {
    this.property = 'schoolDistrictName';
    this.direction = 1;
    this.payments = this.allPayments;
  }

  ngOnInit() {
    this.paymentsService.getPayments().subscribe(
      data => {
        this.allPayments = this.payments = data;
        console.log('PaymentsListComponent.ngOnInit(): mocked data is ', this.allPayments);
      }
    );

    this.schoolDistrictsService.getSchoolDistricts().subscribe(
      data => {
        this.schoolDistricts = data['schoolDistricts'];
        console.log('PaymentsListComponent.ngOnInit():  school district list is ', this.schoolDistricts);
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
          i.schoolDistrictId.toString().search(re) !== -1 ||
          i.schoolDistrictName.search(re) !== -1 ||
          i.type.search(re) !== -1
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

  createPayment() {
    const modal = this.ngbModalService.open(PaymentUpsertFormComponent, { centered: true, size: 'lg' });
    modal.componentInstance.op = 'create';
    modal.componentInstance.schoolDistricts = this.schoolDistricts;

    modal.result.then(
      (result) => {
        console.log('PaymentsListComponent.createPayment():  result is ', result);
      },
      (reason) => {
        console.log('PaymentsListComponent.createPayment():  reason is ', reason);
      }
    );
  }

  editPayment(p: Payment) {
    const modal = this.ngbModalService.open(PaymentUpsertFormComponent, { centered: true });
    modal.componentInstance.op = 'update';
    modal.componentInstance.schoolDistricts = this.schoolDistricts;
    modal.componentInstance.paymentRecord = p;

    modal.result.then(
      (result) => {
        console.log('PaymentsListComponent.editPayment():  result is ', result);
      },
      (reason) => {
        console.log('PaymentsListComponent.editPayment():  result is ', reason);
      }
    );
  }
}

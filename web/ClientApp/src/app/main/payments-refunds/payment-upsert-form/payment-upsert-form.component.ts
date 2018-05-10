import { Component, OnInit } from '@angular/core';

import { SchoolDistrict } from '../../../models/school-district.model';

import { PaymentsService } from '../../../services/payments.service';
import { SchoolDistrictService } from '../../../services/school-district.service';

import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-payment-upsert-form',
  templateUrl: './payment-upsert-form.component.html',
  styleUrls: ['./payment-upsert-form.component.sass']
})
export class PaymentUpsertFormComponent implements OnInit {
  private amount: number;
  private schoolDistricts: SchoolDistrict[];
  private operation: string;
  private selectedSchoolDistrict: SchoolDistrict;

  constructor(
    private activeModal: NgbActiveModal,
    private paymentsService: PaymentsService,
    private schoolDistrictService: SchoolDistrictService
  ) {
  }

  ngOnInit() {
    this.operation = 'create';

    this.schoolDistrictService.getSchoolDistricts().subscribe(
      data => {
        this.schoolDistricts = data['schoolDistricts'];
      }
    );
  }

  upsertPayment() {
    if (this.operation === 'create') {
      this.paymentsService.createPayment(payment).subscribe(
        data => {
          console.log('PaymentUpsertFormComponent.createPayment():  payment successfully created.');
        },
        error => {
          console.log('PaymentUpsertFormComponent.createPayment():  payment error: ', error);
        }
      );
    } else if (this.operation === 'update') {
      this.paymentsService.updatePayment(payment).subscribe(
        data => {
          console.log('PaymentUpsertFormComponent.updatePayment():  payment successfully created.');
        },
        error => {
          console.log('PaymentUpsertFormComponent.updatePayment():  payment error: ', error);
        }
      );
    }
  }

  setSelectedSchoolDistrict(sd: SchoolDistrict) {
    this.selectedSchoolDistrict = sd;
  }
}

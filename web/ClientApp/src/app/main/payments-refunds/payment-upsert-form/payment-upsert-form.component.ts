import { Component, Input, OnInit } from '@angular/core';

import { Observable } from 'rxjs/Observable';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';

import { SchoolDistrict } from '../../../models/school-district.model';

import { PaymentsService } from '../../../services/payments.service';
import { SchoolDistrictService } from '../../../services/school-district.service';

import { Payment } from '../../../models/payment.model';

import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-payment-upsert-form',
  templateUrl: './payment-upsert-form.component.html',
  styleUrls: ['./payment-upsert-form.component.scss']
})
export class PaymentUpsertFormComponent implements OnInit {
  private amount: number;
  private selectedSchoolDistrict: SchoolDistrict;
  private schoolDistrictName: string;
  private selectedAcademicYear: string;
  private date: Date;
  private academicYear: string;
  private paymentTypeId: string;
  private schoolYears: string[] = [
    '2012-2013',
    '2013-2014',
    '2014-2015',
    '2016-2016',
    '2016-2017',
    '2017-2018'
  ];

  @Input() op: string;
  @Input() schoolDistricts: SchoolDistrict;
  @Input() paymentRecord: Payment;

  constructor(
    private activeModal: NgbActiveModal,
    private paymentsService: PaymentsService,
    private schoolDistrictService: SchoolDistrictService
  ) {
  }

  ngOnInit() {
    console.log('op is ', this.op);
    console.log('schoolDistricts are ', this.schoolDistricts);

    if (this.op === 'update') {
      this.schoolDistrictService.getSchoolDistrict(this.paymentRecord.schoolDistrictId).subscribe(
        data => {
          this.selectedSchoolDistrict = data['schoolDistrict'];
        }
      );
    }

    if (this.paymentRecord) {
      this.amount = this.paymentRecord.paymentAmt;
      this.paymentTypeId = this.paymentRecord.type;
      this.academicYear = this.paymentRecord.academicYear;
      this.date = this.paymentRecord.paymentDate;
      this.selectedAcademicYear = this.paymentRecord.academicYear;
    }
  }

  fillPaymentRecord() {
    Object.assign(this.paymentRecord, {
      type: this.paymentTypeId,
      schoolDistrictName: this.selectedSchoolDistrict.name,
      schoolDistrictId: this.selectedSchoolDistrict.id,
      paymentDate: this.date,
      paymentAmt: this.amount,
      academicYear: this.academicYear
    });
 }

  upsertPayment() {
    this.fillPaymentRecord();
    if (this.op === 'create') {
      this.paymentsService.createPayment(this.paymentRecord).subscribe(
        data => {
          console.log('PaymentUpsertFormComponent.createPayment():  payment successfully created.');
        },
        error => {
          console.log('PaymentUpsertFormComponent.createPayment():  payment error: ', error);
        }
      );
    } else if (this.op === 'update') {
      this.paymentsService.updatePayment(this.paymentRecord).subscribe(
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

  setSelectedAcademicYear(year: string) {
    this.selectedAcademicYear = year;
  }

  // onDateChanged() {
  //   this.date = new Date(this.model.year, this.model.month - 1, this.model.day); // yes, that bit of math on the month value is necessary
  //   this.dateSelected.emit(this.date);
  // }

  search = (text$: Observable<string>) => {
    return text$.pipe(
      debounceTime(200),
      distinctUntilChanged(),
      map((term) => {
        term.length < 2 ? [] : this.schoolDistricts.filter(
          (sd) => {
            // new RegExp(term, 'gi').test(sd.name);
            if (sd.name.toLowerCase().indexOf(term.toLowerCase()) > -1) {
              return true;
            } else {
              return false;
            }
          }
        ).slice(0, 10);
      })
    );
  }
}

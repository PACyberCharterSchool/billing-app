import { Component, Input, OnInit } from '@angular/core';

import { Observable } from 'rxjs/Observable';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';

import { SchoolDistrict } from '../../../models/school-district.model';

import { PaymentsService } from '../../../services/payments.service';
import { SchoolDistrictService } from '../../../services/school-district.service';
import { AcademicYearsService } from '../../../services/academic-years.service';

import { Payment } from '../../../models/payment.model';

import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-payment-upsert-form',
  templateUrl: './payment-upsert-form.component.html',
  styleUrls: ['./payment-upsert-form.component.scss']
})
export class PaymentUpsertFormComponent implements OnInit {
  private amount: number;
  private splitAmount: number;
  private selectedSchoolDistrict: SchoolDistrict;
  private selectedAcademicYear: string;
  private selectedAcademicYearSplit: string;
  private date: Date;
  private paymentType: string;
  private paymentTypeId: string;
  private schoolYears: string[];
  private isSplit: boolean;

  public model: any;

  @Input() op: string;
  @Input() schoolDistricts: SchoolDistrict[];
  @Input() paymentRecord: Payment;

  constructor(
    private activeModal: NgbActiveModal,
    private paymentsService: PaymentsService,
    private schoolDistrictService: SchoolDistrictService,
    private academicYearsService: AcademicYearsService
  ) {
  }

  ngOnInit() {
    console.log('op is ', this.op);
    console.log('schoolDistricts are ', this.schoolDistricts);

    this.schoolYears = this.academicYearsService.getAcademicYears();

    if (this.op === 'update') {
      this.schoolDistrictService.getSchoolDistrict(this.paymentRecord.schoolDistrict.id).subscribe(
        data => {
          this.selectedSchoolDistrict = data['schoolDistrict'];
        }
      );
    }

    if (this.paymentRecord) {
      this.amount = this.paymentRecord.amount;
      this.paymentType = this.paymentRecord.type;
      this.date = this.paymentRecord.date;
      this.selectedAcademicYear = this.paymentRecord.schoolYear;
    } else {
      this.paymentRecord = new Payment();
    }

    this.isSplit = false;
  }

  fillSplitsColumn() {
    let splits: Object[];

    splits = [{ 'amount': this.amount, 'schoolYear': this.selectedAcademicYear }];
    if (this.splitAmount) {
      splits.push({ 'amount': this.splitAmount, 'schoolYear': this.selectedAcademicYearSplit });
    }

    return splits;
  }

  fillPaymentRecord() {

    Object.assign(this.paymentRecord, {
      splits: this.fillSplitsColumn(),
      date: this.date,
      externalId: this.selectedSchoolDistrict.id,
      type: this.paymentType ? 'Check' : 'UniPay',
      schoolDistrictAun: +this.selectedSchoolDistrict.aun
    });
 }

  upsertPayment() {
    this.fillPaymentRecord();
    if (this.op === 'create') {
      this.paymentsService.createPayment(this.paymentRecord).subscribe(
        data => {
          console.log('PaymentUpsertFormComponent.createPayment():  payment successfully created.');
          this.activeModal.close('success');
        },
        error => {
          console.log('PaymentUpsertFormComponent.createPayment():  payment error: ', error);
          this.activeModal.close('error');
        }
      );
    } else if (this.op === 'update') {
      this.paymentsService.updatePayment(this.paymentRecord).subscribe(
        data => {
          console.log('PaymentUpsertFormComponent.updatePayment():  payment successfully created.');
          this.activeModal.close('success');
        },
        error => {
          console.log('PaymentUpsertFormComponent.updatePayment():  payment error: ', error);
          this.activeModal.close('error');
        }
      );
    }
  }

  setSelectedSchoolDistrict($event) {
    this.selectedSchoolDistrict = this.schoolDistricts.find((sd) => sd.name === $event.item);
  }

  setSelectedAcademicYear(year: string) {
    this.selectedAcademicYear = year;
  }

  setSelectedAcademicYearSplit(year: string) {
    this.selectedAcademicYearSplit = year;
  }

  onDateChanged() {
    this.date = new Date(this.model.year, this.model.month - 1, this.model.day); // yes, that bit of math on the month value is necessary
  }

  search = (text$: Observable<string>) => {
    const results = text$.pipe(
      debounceTime(200),
      distinctUntilChanged(),
      map((term) => {
        return term.length < 2 ? [] : this.schoolDistricts.filter(
          (sd) => {
            if (sd.name.toLowerCase().indexOf(term.toLowerCase()) > -1) {
              return true;
            } else {
              return false;
            }
          }).map((sd) => sd.name);
      })
    );

    return results;
  }
}

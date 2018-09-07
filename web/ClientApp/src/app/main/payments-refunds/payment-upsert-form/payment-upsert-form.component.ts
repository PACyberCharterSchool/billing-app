import { Component, Input, OnInit } from '@angular/core';

import { Observable } from 'rxjs/Observable';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';

import { SchoolDistrict } from '../../../models/school-district.model';

import { PaymentsService } from '../../../services/payments.service';
import { SchoolDistrictService } from '../../../services/school-district.service';
import { AcademicYearsService } from '../../../services/academic-years.service';

import { Payment, PaymentType } from '../../../models/payment.model';

import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-payment-upsert-form',
  templateUrl: './payment-upsert-form.component.html',
  styleUrls: ['./payment-upsert-form.component.scss']
})
export class PaymentUpsertFormComponent implements OnInit {
  public amount: number;
  public splitAmount: number;
  public selectedSchoolDistrict: SchoolDistrict;
  public selectedAcademicYear: string;
  public selectedAcademicYearSplit: string;
  public externalId: string;
  public schoolYears: string[];
  public isSplit: boolean;
  public date: Date;
  public dateModel: any;
  public dateString: string;
  public schoolDistrictNameModel: string;
  public upsertError: string;
  public checkNumber: string;

  @Input() op: string;
  @Input() schoolDistricts: SchoolDistrict[];
  @Input() paymentRecord: Payment;

  constructor(
    public activeModal: NgbActiveModal,
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
      this.updatePaymentComponentValues();
    } else {
      this.paymentRecord = new Payment();
    }
  }

  updatePaymentComponentValues() {
    this.selectedSchoolDistrict = this.paymentRecord.schoolDistrict;
    this.schoolDistrictNameModel = this.paymentRecord.schoolDistrict.name;
    this.amount = this.paymentRecord.amount;
    this.splitAmount = this.paymentRecord.splitAmount;

    const date = new Date(this.paymentRecord.date);
    this.dateModel = { 'month': date.getMonth() + 1, 'day': date.getDate(), 'year': date.getFullYear() };

    this.selectedAcademicYear = this.paymentRecord.schoolYear;
    this.selectedAcademicYearSplit = this.paymentRecord.schoolYearSplit;
    this.checkNumber = this.paymentRecord.externalId;
    this.isSplit = this.paymentRecord.split === 2 ? true : false ;
  }

  updatePaymentRecord() {
    this.paymentRecord.schoolDistrict = this.selectedSchoolDistrict;
    this.paymentRecord.amount = this.amount;
    this.paymentRecord.externalId  = this.checkNumber;
    this.paymentRecord.splitAmount = this.splitAmount;
    this.paymentRecord.type = PaymentType.Check;
    this.paymentRecord.date = new Date(`${this.dateModel.month}/${this.dateModel.day}/${this.dateModel.year}`);
    this.paymentRecord.schoolYear = this.selectedAcademicYear !== null ? this.selectedAcademicYear.replace(/\s+/g, '') : null;
    this.paymentRecord.schoolYearSplit =
      this.selectedAcademicYearSplit !== null ? this.selectedAcademicYearSplit.replace(/s\+/g, '') : null;
    this.paymentRecord.split = this.isSplit ? 2 : 1;
  }

  upsertPayment() {
    this.updatePaymentRecord();
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
    // yes, that bit of math on the month value is necessary
    console.log('PaymentUpsertFormComponent.onDateChanged():  dateModel type is ', typeof(this.dateModel));
    console.log('PaymentUpsertFormComponent.onDateChanged(): dateModel is ', this.dateModel);
    this.date = new Date(`${this.dateModel.month}/${this.dateModel.day}/${this.dateModel.year}`);
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

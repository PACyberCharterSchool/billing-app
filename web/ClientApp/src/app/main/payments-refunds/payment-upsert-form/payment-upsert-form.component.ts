import { Component, Input, OnInit, ViewChild, ElementRef, ViewChildren } from '@angular/core';

import { NgForm } from '@angular/forms';

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
  private paymentId: string;
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
  public payment: Payment;
  public academicYearVisibility: boolean;
  public academicYearSplitVisibility: boolean;

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
    this.academicYearVisibility = false;
    this.academicYearSplitVisibility = false;

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

  initPaymentRecord(payments: Object[]): void {
    if (payments) {
      this.paymentId = payments[0]['paymentId'];
      this.isSplit = payments.length > 1;
      this.date = payments[0]['date'];
      this.externalId = payments[0]['externalId'];
      this.schoolDistrictNameModel = payments[0]['schoolDistrict']['name'];
      this.amount = payments[0]['amount'];
      this.selectedAcademicYear = payments[0]['schoolYear'];

      if (payments.length > 1) {
        this.splitAmount = payments[1]['amount'];
        this.selectedAcademicYearSplit = payments[1]['schoolYear'];
      }

      const date = new Date(this.paymentRecord.date);
      this.dateModel = { 'month': date.getMonth() + 1, 'day': date.getDate(), 'year': date.getFullYear() };

      this.checkNumber = this.paymentRecord.externalId;
    }
  }

  updatePaymentComponentValues() {
    this.paymentsService.getPaymentsByPaymentId(this.paymentRecord.paymentId).subscribe(
      data => {
        this.initPaymentRecord(data['payments']);
      },
      error => {
        console.log('PaymentUpsertFormComponent.ngOnInit():  error is ', error);
      }
    );
  }

  private areAcademicYearsEqual(): boolean {
    return this.selectedAcademicYear &&
      this.selectedAcademicYearSplit &&
      this.selectedAcademicYear.replace(/\s+/, '').toLowerCase() === this.selectedAcademicYearSplit.replace(/\s+/, '').toLowerCase();
  }

  private updatePaymentRecord(): void {
    this.paymentRecord.schoolDistrict = this.selectedSchoolDistrict;
    this.paymentRecord.split = this.isSplit ? 2 : 1;
    this.paymentRecord.amount = this.amount;
    this.paymentRecord.splitAmount = this.splitAmount;
    this.paymentRecord.externalId  = this.checkNumber;
    this.paymentRecord.type = PaymentType.Check;
    this.paymentRecord.date = new Date(`${this.dateModel.month}/${this.dateModel.day}/${this.dateModel.year}`);
    this.paymentRecord.schoolYear = this.selectedAcademicYear ? this.selectedAcademicYear.replace(/\s+/g, '') : null;
    this.paymentRecord.schoolYearSplit = this.selectedAcademicYearSplit ? this.selectedAcademicYearSplit.replace(/\s+/g, '') : null;
  }

  public onSubmit(form: NgForm): void {
    if (this.areAcademicYearsEqual()) {
      this.academicYearSplitVisibility = true;
      this.academicYearVisibility = true;
      return;
    }

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
    this.academicYearVisibility = this.academicYearSplitVisibility = false;
  }

  setSelectedAcademicYearSplit(year: string) {
    this.selectedAcademicYearSplit = year;
    this.academicYearSplitVisibility = this.academicYearVisibility = false;
  }

  onDateChanged() {
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

import { Component, Input, OnInit } from '@angular/core';

import { Observable } from 'rxjs/Observable';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';

import { SchoolDistrict } from '../../../models/school-district.model';

import { RefundsService } from '../../../services/refunds.service';
import { SchoolDistrictService } from '../../../services/school-district.service';
import { AcademicYearsService } from '../../../services/academic-years.service';

import { Refund } from '../../../models/refund.model';

import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-refund-upsert-form',
  templateUrl: './refund-upsert-form.component.html',
  styleUrls: ['./refund-upsert-form.component.scss']
})
export class RefundUpsertFormComponent implements OnInit {
  public amount: number;
  public selectedSchoolDistrict: SchoolDistrict;
  public selectedAcademicYear: string;
  public selectedSchoolDistrictName: string;
  public dateModel: any;
  public refundCheckNumber: string;
  public schoolYears: string[];
  public model: any;
  private skip: number;
  public upsertError: string;

  @Input() op: string;
  @Input() schoolDistricts: SchoolDistrict[];
  @Input() refundRecord: Refund;

  constructor(
    public activeModal: NgbActiveModal,
    private refundsService: RefundsService,
    private academicYearsService: AcademicYearsService,
    private schoolDistrictService: SchoolDistrictService
  ) {
  }

  ngOnInit() {
    console.log('op is ', this.op);
    console.log('schoolDistricts are ', this.schoolDistricts);

    this.schoolYears = this.academicYearsService.getAcademicYears();

    if (this.op === 'update') {
      this.schoolDistrictService.getSchoolDistrict(this.refundRecord.schoolDistrict.id).subscribe(
        data => {
          this.selectedSchoolDistrict = data['schoolDistrict'];
        }
      );
    }

    if (this.refundRecord) {
      this.updateRefundComponentValues();
    } else {
      this.refundRecord = new Refund();
    }
  }

  updateRefundComponentValues() {
    this.amount = this.refundRecord.amount;
    this.refundCheckNumber = this.refundRecord.checkNumber;
    this.selectedAcademicYear = this.refundRecord.schoolYear;

    const date = new Date(this.refundRecord.date);

    this.dateModel = {
      'month': date.getMonth() + 1,
      'day': date.getDate(),
      'year': date.getFullYear()
    };

    this.selectedSchoolDistrict = this.refundRecord.schoolDistrict;
    this.selectedSchoolDistrictName = this.selectedSchoolDistrict.name;
  }

  updateRefundRecord() {
    this.refundRecord.amount = this.amount;
    this.refundRecord.checkNumber = this.refundCheckNumber;
    this.refundRecord.schoolYear = this.selectedAcademicYear;
    this.refundRecord.date = new Date(`${this.dateModel.month}/${this.dateModel.day}/${this.dateModel.year}`);
    this.refundRecord.schoolDistrict = this.schoolDistricts.find((sd) => sd.name === this.selectedSchoolDistrictName);
  }

  upsertRefund() {
    this.updateRefundRecord();
    if (this.op === 'create') {
      this.refundsService.createRefund(this.refundRecord).subscribe(
        data => {
          this.activeModal.close('success');
        },
        error => {
          this.activeModal.close('error');
          console.log('RefundUpsertFormComponent.upsertRefund(): error is ', error);
        }
      );
    } else if (this.op === 'update') {
      this.refundsService.updateRefund(this.refundRecord).subscribe(
        data => {
          this.activeModal.close('success');
        },
        error => {
          this.activeModal.close('error');
          console.log('RefundUpsertFormComponent.upsertRefund(): error is ', error);
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

  onDateChanged() {
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

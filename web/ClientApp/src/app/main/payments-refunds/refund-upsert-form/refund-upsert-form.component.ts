import { Component, Input, OnInit } from '@angular/core';

import { Observable } from 'rxjs/Observable';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';

import { SchoolDistrict } from '../../../models/school-district.model';

import { RefundsService } from '../../../services/refunds.service';
import { SchoolDistrictService } from '../../../services/school-district.service';

import { Refund } from '../../../models/refund.model';

import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-refund-upsert-form',
  templateUrl: './refund-upsert-form.component.html',
  styleUrls: ['./refund-upsert-form.component.scss']
})
export class RefundUpsertFormComponent implements OnInit {
  private amount: number;
  private selectedSchoolDistrict: SchoolDistrict;
  private schoolDistrictName: string;
  private selectedAcademicYear: string;
  private date: Date;
  private academicYear: string;
  private refundCheckNumber: string;
  private schoolYears: string[] = [
    '2012-2013',
    '2013-2014',
    '2014-2015',
    '2016-2016',
    '2016-2017',
    '2017-2018'
  ];

  public model: any;

  @Input() op: string;
  @Input() schoolDistricts: SchoolDistrict[];
  @Input() refundRecord: Refund;

  constructor(
    private activeModal: NgbActiveModal,
    private paymentsService: RefundsService,
    private schoolDistrictService: SchoolDistrictService
  ) {
  }

  ngOnInit() {
    console.log('op is ', this.op);
    console.log('schoolDistricts are ', this.schoolDistricts);

    if (this.op === 'update') {
      this.schoolDistrictService.getSchoolDistrict(this.refundRecord.schoolDistrictId).subscribe(
        data => {
          this.selectedSchoolDistrict = data['schoolDistrict'];
        }
      );
    }

    if (this.refundRecord) {
      this.amount = this.refundRecord.refundAmt;
      this.refundCheckNumber = this.refundRecord.refundCheckNumber;
      this.academicYear = this.refundRecord.academicYear;
      this.date = this.refundRecord.refundDate;
      this.selectedAcademicYear = this.refundRecord.academicYear;
    }
  }

  fillRefundRecord() {
    this.schoolDistrictName = this.selectedSchoolDistrict.name;
    Object.assign(this.refundRecord, {
      type: this.refundCheckNumber,
      schoolDistrictName: this.selectedSchoolDistrict.name,
      schoolDistrictId: this.selectedSchoolDistrict.id,
      refundDate: this.date,
      refundAmt: this.amount,
      academicYear: this.academicYear
    });
 }

  upsertRefund() {
    this.fillRefundRecord();
    if (this.op === 'create') {
      this.paymentsService.createRefund(this.refundRecord).subscribe(
        data => {
        },
        error => {
        }
      );
    } else if (this.op === 'update') {
      this.paymentsService.updateRefund(this.refundRecord).subscribe(
        data => {
        },
        error => {
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

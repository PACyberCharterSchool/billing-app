import { Component, OnInit } from '@angular/core';

import { Refund } from '../../../models/refund.model';
import { SchoolDistrict } from '../../../models/school-district.model';

import { UtilitiesService } from '../../../services/utilities.service';
import { RefundsService } from '../../../services/refunds.service';
import { SchoolDistrictService } from '../../../services/school-district.service';
import { AcademicYearsService } from '../../../services/academic-years.service';

import { RefundUpsertFormComponent } from '../refund-upsert-form/refund-upsert-form.component';

import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

import { Globals } from '../../../globals';

@Component({
  selector: 'app-refunds-list',
  templateUrl: './refunds-list.component.html',
  styleUrls: ['./refunds-list.component.scss']
})
export class RefundsListComponent implements OnInit {
  private searchText: string;
  private direction: number;
  private property: string;
  private isDescending: boolean;
  private allRefunds: Refund[];
  private refunds: Refund[];
  private schoolDistricts: SchoolDistrict[];
  private skip: number;

  constructor(
    private globals: Globals,
    private utilitiesService: UtilitiesService,
    private refundsService: RefundsService,
    private schoolDistrictsService: SchoolDistrictService,
    private ngbModalService: NgbModal
  ) {
    this.property = 'schoolDistrictName';
    this.direction = 1;
    this.refunds = this.allRefunds;
    this.skip = 0;
  }

  ngOnInit() {
    this.refreshRefundList();
    this.refundsService.getRefunds(this.skip).subscribe(
      data => {
        this.allRefunds = this.refunds = data['refunds'];
        console.log('RefundsListComponent.ngOnInit(): mocked data is ', this.allRefunds);
      },
      error => {
        console.log('RefundsListComponent.ngOnInit(): error is ', error);
      }
    );

    this.schoolDistrictsService.getSchoolDistricts().subscribe(
      data => {
        this.schoolDistricts = data['schoolDistricts'];
        console.log('RefundsListComponent.ngOnInit():  school district list is ', this.schoolDistricts);
      },
      error => {
        console.log('RefundsListComponent.ngOnInit():  error is ', error);
      }
    );
  }

  sort(property) {
    this.isDescending = !this.isDescending; // change the direction
    this.property = property;
    this.direction = this.isDescending ? 1 : -1;
  }

  refreshRefundList() {
    this.refundsService.getRefunds(this.skip).subscribe(
      data => {
        this.allRefunds = this.refunds = data['refunds'];
        console.log('PaymentsListComponent.ngOnInit(): payments are ', this.allRefunds);
      },
      error => {
        console.log('PaymentsListComponent.ngOnInit(): error is ', error);
      }
    );
  }

  listDisplayableFields() {
    if (this.allRefunds) {
      const fields = this.utilitiesService.objectKeys(this.allRefunds[0]);
      const rejected = ['id'];
      return fields.filter((i) => !rejected.includes(i));
    }
  }

  listDisplayableValues(refund: Refund) {
    if (refund) {
      const vkeys = this.listDisplayableFields();
      const selected = this.utilitiesService.pick(refund, vkeys);

      return this.utilitiesService.objectValues(selected);
    }
  }

  filterRefundRecords() {
    this.refunds = this.allRefunds.filter(
      (i) => {
        const re = new RegExp(this.searchText, 'gi');
        if (
          i.amount.toString().search(re) !== -1 ||
          i.checkNumber.search(re) !== -1 ||
          i.schoolDistrict.name.search(re) !== -1
        ) {
          return true;
        }
        return false;
      }
    );
  }

  resetRefundRecords() {
    this.refunds = this.allRefunds;
  }

  createRefund() {
    const modal = this.ngbModalService.open(RefundUpsertFormComponent, { centered: true });
    modal.componentInstance.op = 'create';
    modal.componentInstance.schoolDistricts = this.schoolDistricts;

    modal.result.then(
      (result) => {
        console.log('RefundsListComponent.createRefund():  result is ', result);
        this.refreshRefundList();
      },
      (reason) => {
        console.log('RefundsListComponent.createRefund():  reason is ', reason);
      }
    );
  }

  editRefund(r: Refund) {
    const modal = this.ngbModalService.open(RefundUpsertFormComponent, { centered: true });
    modal.componentInstance.op = 'update';
    modal.componentInstance.schoolDistricts = this.schoolDistricts;
    modal.componentInstance.refundRecord = r;

    modal.result.then(
      (result) => {
        console.log('RefundsListComponent.editPayment():  result is ', result);
        this.refreshRefundList();
      },
      (reason) => {
        console.log('RefundsListComponent.editPayment():  result is ', reason);
      }
    );
  }

  updateScrollingSkip() {
    this.skip += this.globals.take;
  }
}

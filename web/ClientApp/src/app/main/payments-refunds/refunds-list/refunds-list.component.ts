import { Component, OnInit } from '@angular/core';

import { Refund } from '../../../models/refund.model';
import { SchoolDistrict } from '../../../models/school-district.model';

import { UtilitiesService } from '../../../services/utilities.service';
import { RefundsService } from '../../../services/refunds.service';
import { SchoolDistrictService } from '../../../services/school-district.service';

import { RefundUpsertFormComponent } from '../refund-upsert-form/refund-upsert-form.component';

import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

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

  constructor(
    private utilitiesService: UtilitiesService,
    private refundsService: RefundsService,
    private schoolDistrictsService: SchoolDistrictService,
    private ngbModalService: NgbModal
  ) {
    this.property = 'schoolDistrictName';
    this.direction = 1;
    this.refunds = this.allRefunds;
  }

  ngOnInit() {
    this.refundsService.getRefunds().subscribe(
      data => {
        this.allRefunds = this.refunds = data;
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

  filterRefundRecords() {
    this.refunds = this.allRefunds.filter(
      (i) => {
        const re = new RegExp(this.searchText, 'gi');
        if (
          i.schoolDistrictId.toString().search(re) !== -1 ||
          i.schoolDistrictName.search(re) !== -1 ||
          i.refundCheckNumber.search(re) !== -1
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
      },
      (reason) => {
        console.log('RefundsListComponent.editPayment():  result is ', reason);
      }
    );
  }
}

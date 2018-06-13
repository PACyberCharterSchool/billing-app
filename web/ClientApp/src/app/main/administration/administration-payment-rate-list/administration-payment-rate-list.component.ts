import { Component, OnInit } from '@angular/core';

import { SchoolDistrict } from '../../../models/school-district.model';

import { SchoolDistrictService } from '../../../services/school-district.service';
import { UtilitiesService } from '../../../services/utilities.service';

import { Globals } from '../../../globals';

import { NgbModal, ModalDismissReasons } from '@ng-bootstrap/ng-bootstrap';

import { AdministrationPaymentRateUpdateFormComponent } from '../administration-payment-rate-update-form/administration-payment-rate-update-form.component';

@Component({
  selector: 'app-administration-payment-rate-list',
  templateUrl: './administration-payment-rate-list.component.html',
  styleUrls: ['./administration-payment-rate-list.component.scss']
})
export class AdministrationPaymentRateListComponent implements OnInit {
  private schoolDistricts: SchoolDistrict[];
  private allSchoolDistricts: SchoolDistrict[];
  private property: string;
  private isDescending: boolean;
  private direction: number;
  private searchText: string;

  model;

  constructor(
    private globals: Globals,
    private schoolDistrictService: SchoolDistrictService,
    private utilitiesService: UtilitiesService,
    private ngbModal: NgbModal
  ) {
    this.model  = new SchoolDistrict();
    this.property = 'name';
    this.direction = 1;
  }

  ngOnInit() {
    this.schoolDistrictService.getSchoolDistricts().subscribe(
      data => {
        this.schoolDistricts = this.allSchoolDistricts = data['schoolDistricts'];
        console.log('AdministrationPaymentRateListComponent.ngOnInit(): school districts are ', data['schoolDistricts']);
      }
    );
  }

  sort(property) {
    this.isDescending = !this.isDescending; // change the direction
    this.property = property;
    this.direction = this.isDescending ? 1 : -1;
  }

  filterSchoolDistrictRecords() {
    this.schoolDistricts = this.allSchoolDistricts.filter(
      (i) => {
        const re = new RegExp(this.searchText, 'gi');
        if (i &&
          i.aun.toString().search(re) !== -1 ||
          i.name.search(re) !== -1 ||
          i.paymentType.search(re) !== -1 ||
          i.rate.toString().search(re) !== -1 ||
          (i.alternateRate && i.alternateRate.toString().search(re) !== -1)
        ) {
          return true;
        }
        return false;
      }
    );
    console.log('PaymentsListComponent.filterSchoolDistrictRecords():  schoolDistricts is ', this.schoolDistricts);
  }

  resetSchoolDistrictRecords() {
    this.schoolDistricts = this.allSchoolDistricts;
    this.searchText = '';
  }

  getAdditionalSchoolDistricts($event) {
    this.schoolDistrictService.getSchoolDistricts().subscribe(
      data => {
        this.schoolDistricts = this.schoolDistricts.concat(data['schoolDistricts']);
        console.log('PaymentsListComponent.getPayments():  school districts are ', this.schoolDistricts);
      },
      error => {
        console.log('PaymentsListComponent.getPayments():  error is ', error);
      }
    );
  }

  refreshSchoolDistrict(sd: SchoolDistrict) {
    console.log('AdministrationPaymentRateListComponent.refreshSchoolDistrict(): school district is ', sd);
    this.model = sd;
    console.log('AdministrationPaymentRateListComponent.refreshSchoolDistrict(): model is ', this.model);
  }

  listDisplayableFields(): Object[] {
    if (this.schoolDistricts) {
      const fields = this.utilitiesService.objectKeys(this.schoolDistricts[0]);
      const rejected = ['id'];
      return fields.filter((i) => !rejected.includes(i));
    }
  }

  listDisplayableValues(sd: SchoolDistrict): Object[] {
    if (sd) {
      const vkeys = this.listDisplayableFields();
      const selected = this.utilitiesService.pick(sd, vkeys);

      return this.utilitiesService.objectValues(selected);
    }
  }

  selectSchoolDistrict(sd: SchoolDistrict) {
    this.model = sd;
  }

  refreshSchoolDistrictList() {
    this.schoolDistrictService.getSchoolDistricts().subscribe(
      data => {
        this.allSchoolDistricts = data['schoolDistricts'];
        console.log('success');
      },
      error => {
        console.log('error: ', error);
      }
    )
  }

  editSchoolDistrictPaymentRate(sd: SchoolDistrict) {
    this.model = sd;

    const modal = this.ngbModal.open(AdministrationPaymentRateUpdateFormComponent, { centered: true });
    modal.componentInstance.schoolDistrict = sd;
    modal.result.then(
       (result) => {
         console.log(`Closed with: ${result}.`);
       },
       (reason) => {
         console.log(`Dismissed: ${this.getDismissReasons(reason)}.`);
       }
    );
  }

  private getDismissReasons(reason: any) {
    if (reason === ModalDismissReasons.ESC) {
        return 'by pressing ESC';
    } else if (reason === ModalDismissReasons.BACKDROP_CLICK) {
        return 'by clicking on backdrop.';
    } else {
        return `with ${reason}.`;
    }
  }

}

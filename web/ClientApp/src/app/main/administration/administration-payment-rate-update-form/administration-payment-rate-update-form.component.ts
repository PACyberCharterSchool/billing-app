import { Component, Input, OnInit } from '@angular/core';

import { NgbActiveModal, ModalDismissReasons } from '@ng-bootstrap/ng-bootstrap';

import { SchoolDistrictService } from '../../../services/school-district.service';

import { SchoolDistrict } from '../../../models/school-district.model';

@Component({
  selector: 'app-administration-payment-rate-update-form',
  templateUrl: './administration-payment-rate-update-form.component.html',
  styleUrls: ['./administration-payment-rate-update-form.component.scss']
})
export class AdministrationPaymentRateUpdateFormComponent implements OnInit {

  @Input() schoolDistrict: SchoolDistrict;

  constructor(
    private schoolDistrictService: SchoolDistrictService,
    public ngbActiveModal: NgbActiveModal
  ) { }

  ngOnInit() {
  }

  onSubmit() {
    console.log('AdministrationPaymentRateUpdateComponent.onSubmit():  schoolDistrict is ', this.schoolDistrict);
    this.schoolDistrict['aun'] = this.schoolDistrict.aun;
    this.schoolDistrict['name'] = this.schoolDistrict.name;
    this.schoolDistrict['paymentType'] = this.schoolDistrict.paymentType === 'SD' ? 'Check' : 'UniPay';
    this.schoolDistrictService.updateSchoolDistrict(this.schoolDistrict).subscribe(
      data => {
        this.ngbActiveModal.close('success');
        console.log('AdministrationPaymentRateUpdateComponent.onSubmit():  data is ', data);
      },
      error => {
        console.log('AdministrationPaymentRateUpdateComponent.onSubmit():  error is ', error);
        this.ngbActiveModal.dismiss('response');
      }
    );
  }

}

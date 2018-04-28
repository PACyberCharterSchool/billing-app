import { Component, OnInit } from '@angular/core';

import { SchoolDistrict } from '../../../models/school-district.model';

import { SchoolDistrictService } from '../../../services/school-district.service';

@Component({
  selector: 'app-administration-payment-rate',
  templateUrl: './administration-payment-rate.component.html',
  styleUrls: ['./administration-payment-rate.component.scss']
})
export class AdministrationPaymentRateComponent implements OnInit {

  private schoolDistricts: SchoolDistrict[];

  model;

  constructor(private schoolDistrictService: SchoolDistrictService) {
    this.model  = new SchoolDistrict();
  }

  ngOnInit() {
    this.schoolDistrictService.getSchoolDistricts().subscribe(
      data => {
        this.schoolDistricts = data['schoolDistricts'];
        console.log('AdministrationPaymentRateComponent.ngOnInit(): school districts are ', data['schoolDistricts']);
      }
    );
  }

  onSubmit() {
    console.log('AdministrationPaymentRateComponent.onSubmit():  model is ', this.model);
    this.model['Aun'] = this.model.aun;
    this.model['Name'] = this.model.name;
    this.model['PaymentType'] = this.model.paymentType;
    this.schoolDistrictService.updateSchoolDistrict(this.model).subscribe(
      val => {
        console.log('AdministrationPaymentRateComponent.onSubmit():  val is ', val);
      },
      response => {
        console.log('AdministrationPaymentRateComponent.onSubmit():  response is ', response);
      }
    );
  }

  refreshSchoolDistrict(sd: SchoolDistrict) {
    console.log('AdministrationPaymentRateComponent.refreshSchoolDistrict(): school district is ', sd);
    this.model = sd;
    console.log('AdministrationPaymentRateComponent.refreshSchoolDistrict(): model is ', this.model);
  }
}

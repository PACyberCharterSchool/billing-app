import { Component, OnInit } from '@angular/core';

import { UtilitiesService } from '../../../services/utilities.service';

import { Payment } from '../../../models/payment.model';

import { NormalizeFieldNamePipe } from '../../../pipes/normalize-field-name.pipe';

@Component({
  selector: 'app-payments-list',
  templateUrl: './payments-list.component.html',
  styleUrls: ['./payments-list.component.scss']
})
export class PaymentsListComponent implements OnInit {

  private direction: number;
  private property: string;
  private isDescending: boolean;
  private payments: Payment[] = [
    {
      schoolDistrictName: 'Seneca Valley SD',
      schoolDistrictId: '123456',
      paymentAmt: 4300.00,
      type: '#14534',
      paymentDate: new Date('04/01/2018'),
      academicYear: 2018
    },
    {
      schoolDistrictName: 'Mars SD',
      schoolDistrictId: '654321',
      paymentAmt: 1800.00,
      type: '#99999',
      paymentDate: new Date('07/22/2018'),
      academicYear: 2019
    },
    {
      schoolDistrictName: 'Indiana SD',
      schoolDistrictId: '789023',
      paymentAmt: 40000.00,
      type: '#99999',
      paymentDate: new Date('07/22/2018'),
      academicYear: 2019
    }
  ];

  constructor(private utilitiesService: UtilitiesService) {
    this.property = 'schoolDistrictName';
    this.direction = 1;
  }

  ngOnInit() {
  }

  sort(property) {
    this.isDescending = !this.isDescending; // change the direction
    this.property = property;
    this.direction = this.isDescending ? 1 : -1;
  }
}

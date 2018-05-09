import { Component, OnInit } from '@angular/core';

import { UtilitiesService } from '../../../services/utilities.service';

import { Payment } from '../../../models/payment.model';

@Component({
  selector: 'app-payments-list',
  templateUrl: './payments-list.component.html',
  styleUrls: ['./payments-list.component.scss']
})
export class PaymentsListComponent implements OnInit {

  private payments: Payment[] = [
    {
      schoolDistrictName: 'Seneca Valley SD',
      schoolDistrictAUN: '123456',
      paymentAmt: 4300.00,
      type: '#14534',
      paymentDate: new Date('04/01/2018'),
      academicYear: 2018
    },
    {
      schoolDistrictName: 'Mars SD',
      schoolDistrictAUN: '654321',
      paymentAmt: 1800.00,
      type: '#99999',
      paymentDate: new Date('07/22/2018'),
      academicYear: 2019
    },
    {
      schoolDistrictName: 'Indiana SD',
      schoolDistrictAUN: '789023',
      paymentAmt: 40000.00,
      type: '#99999',
      paymentDate: new Date('07/22/2018'),
      academicYear: 2019
    }
  ];

  constructor(private utilitiesService: UtilitiesService) { }

  ngOnInit() {
  }

}

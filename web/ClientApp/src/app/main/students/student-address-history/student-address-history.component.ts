import { Component, OnInit } from '@angular/core';

import { Student } from '../../../models/student.model';

import { UtilitiesService } from '../../../services/utilities.service';

@Component({
  selector: 'app-student-address-history',
  templateUrl: './student-address-history.component.html',
  styleUrls: ['./student-address-history.component.scss']
})
export class StudentAddressHistoryComponent implements OnInit {
  private addresses = [
    { Street1: '1600 Philadelphia Street', Street2: '', City: 'Mars', State: 'PA', 'Zip Code': '16063', 'Date': '05/11/2006' },
    { Street1: '120 Georgetown Ave', Street2: 'Apt. #22', City: 'Irwin', State: 'PA', 'Zip Code': '15004', 'Date': '07/21/2009'},
    { Street1: '920 Pentland Ave', Street2: '', City: 'Cranberry Twp', State: 'PA', 'Zip Code': '16066', 'Date': '11/5/2013' }
  ];

  constructor(private utilities: UtilitiesService) {
  }

  ngOnInit() {
  }

}

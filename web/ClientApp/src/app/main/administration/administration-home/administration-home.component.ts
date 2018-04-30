import { Component, OnInit } from '@angular/core';

import { Router } from '@angular/router';

enum BUTTON_IDS {
  SchoolDistrictRatesBtn = 'school-district-rates-btn',
  ImportStudentDataBtn = 'import-student-data-btn'
}

@Component({
  selector: 'app-administration-home',
  templateUrl: './administration-home.component.html',
  styleUrls: ['./administration-home.component.scss']
})
export class AdministrationHomeComponent implements OnInit {

  constructor(private router: Router) { }

  ngOnInit() {
  }

  handleClick(event) {
    const target = event.currentTarget;
    const idAttr = target.attributes.id;
    const id = idAttr.value;

    if (id) {
      this.handleAdministrationHomeBtnSelection(id);
    }
  }

  handleAdministrationHomeBtnSelection(id: string) {
    switch (id) {
      case BUTTON_IDS.SchoolDistrictRatesBtn:
        this.router.navigate(['/administration', { outlets: { 'action': ['payment-rates'] } }]);
        break;
      case BUTTON_IDS.ImportStudentDataBtn:
        this.router.navigate(['/administration', { outlets: { 'action': ['import-student-data'] } }]);
        break;
      default:
        this.router.navigate(['/administration', { outlets: { 'action': ['home'] } }]);
        break;
    }
  }
}

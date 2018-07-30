import { Component, OnInit } from '@angular/core';

import { Router } from '@angular/router';

enum BUTTON_IDS {
  SchoolDistrictRatesBtn = 'school-district-rates-btn',
  ImportStudentDataBtn = 'import-student-data-btn',
  SchoolCalendarBtn = 'school-calendar-btn',
  AuditsBtn = 'audits-btn',
  TemplatesBtn = 'templates-btn'
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
      case BUTTON_IDS.SchoolCalendarBtn:
        this.router.navigate(['/administration', { outlets: { 'action': ['school-calendar'] } }]);
        break;
      case BUTTON_IDS.AuditsBtn:
        this.router.navigate(['/administration', { outlets: { 'action': ['audits'] } }]);
        break;
      case BUTTON_IDS.TemplatesBtn:
        this.router.navigate(['/administration', { outlets: { 'action': ['templates'] } }]);
        break;
      default:
        this.router.navigate(['/administration', { outlets: { 'action': ['home'] } }]);
        break;
    }
  }
}

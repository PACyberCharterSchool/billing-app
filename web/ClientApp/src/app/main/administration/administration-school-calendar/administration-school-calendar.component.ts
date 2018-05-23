import { Component, OnInit } from '@angular/core';

import { SchoolCalendarService } from '../../../services/school-calendar.service';

import { Calendar } from '../../../models/calendar.model';

import { Globals } from '../../../globals';

@Component({
  selector: 'app-administration-school-calendar',
  templateUrl: './administration-school-calendar.component.html',
  styleUrls: ['./administration-school-calendar.component.scss']
})
export class AdministrationSchoolCalendarComponent implements OnInit {
  private schoolCalendar: Calendar;

  constructor(
    private schoolCalendarService: SchoolCalendarService,
    private globals: Globals
  ) { }

  ngOnInit() {
    this.schoolCalendarService.getByYear(this.globals.currentSchoolYear).subscribe(
      data => {
        console.log('AdministrationSchoolCalendarComponent.ngOnInit(): school calendar is ', data['calendar']);
      },
      error => {
        console.log('AdministrationSchoolCalendarComponent.ngOnInit(): error is ', error);
      }
    );
  }

}

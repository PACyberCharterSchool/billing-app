import { Component, ElementRef, ViewChild, OnInit } from '@angular/core';

import { SchoolCalendarService } from '../../../services/school-calendar.service';
import { UtilitiesService } from '../../../services/utilities.service';

import { Calendar } from '../../../models/calendar.model';
import { CalendarDay } from '../../../models/calendar-day.model';

import { Globals } from '../../../globals';

@Component({
  selector: 'app-administration-school-calendar',
  templateUrl: './administration-school-calendar.component.html',
  styleUrls: ['./administration-school-calendar.component.scss']
})
export class AdministrationSchoolCalendarComponent implements OnInit {
  private schoolCalendar: Calendar;
  public academicYear: string;
  private direction: number;
  private property: string;
  private isDescending: boolean;
  public days: CalendarDay[];
  public searchText: string;

  @ViewChild('calendarFileSelector') calendarFileSelector: ElementRef;

  constructor(
    private schoolCalendarService: SchoolCalendarService,
    private utilitiesService: UtilitiesService,
    private globals: Globals
  ) {
    this.academicYear = globals.currentSchoolYear;
    this.direction = 1;
    this.property = 'schoolDay';
  }

  ngOnInit() {
    this.schoolCalendarService.getByYear(this.globals.currentSchoolYear).subscribe(
      data => {
        console.log('AdministrationSchoolCalendarComponent.ngOnInit(): school calendar is ', data['calendar']);
        this.schoolCalendar = data['calendar'];
        this.days = this.schoolCalendar.days;
      },
      error => {
        console.log('AdministrationSchoolCalendarComponent.ngOnInit(): error is ', error);
      }
    );
  }

  handleFileSelection($event) {
    if ($event) {
      if ($event.target.files && $event.target.files.length > 0) {
        const files = $event.target.files;
        const formData = new FormData();

        formData.append('file', files[0], files[0].name);
        this.schoolCalendarService.updateByYear(this.academicYear, formData).subscribe(
          data => {
            console.log('AdministrationSchoolCalendarComponent.handleFileSelection(): data is ', data['calendar']);
            this.schoolCalendar = data['calendar'];
          },
          error => {
            console.log('AdministrationSchoolCalendarComponent.handleFileSelection(): error is ', error);
          }
        );
      }
    }
  }

  sort(property) {
    this.isDescending = !this.isDescending; // change the direction
    this.property = property;
    this.direction = this.isDescending ? 1 : -1;
  }

  listDisplayableFields() {
    if (this.schoolCalendar && this.schoolCalendar.days && this.schoolCalendar.days.length > 0) {
      const rejected = ['lazyLoader'];
      const fields = this.utilitiesService.objectKeys(this.schoolCalendar.days[0]);
      if (fields) {
        return fields.filter((i) => !rejected.includes(i));
      }
    }
  }

  listDisplayableValues(day) {
    const vkeys = this.listDisplayableFields();
    const selected = this.utilitiesService.pick(day, vkeys);

    return this.utilitiesService.objectValues(selected);
  }

  filterDays() {
    this.days = this.schoolCalendar.days.filter((d) => {
      if (d.dayOfWeek.includes(this.searchText) ||
        d.membership.toString().includes(this.searchText) ||
        d.schoolDay.toString().includes(this.searchText)) {
        return true;
      }

      if (this.utilitiesService.isDateValue(this.searchText)) {
        const d1: Date = new Date(d.date);
        const d2: Date = new Date(this.searchText);
        if (d1.getTime() === d2.getTime()) {
          return true;
        }
      }

      return false;
    });
  }

  resetDays() {
    this.days = this.schoolCalendar.days;
  }
}

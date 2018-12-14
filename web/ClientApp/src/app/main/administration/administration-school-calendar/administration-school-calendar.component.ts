import { Component, ElementRef, ViewChild, OnInit } from '@angular/core';

import { SchoolCalendarService } from '../../../services/school-calendar.service';
import { UtilitiesService } from '../../../services/utilities.service';
import { AcademicYearsService } from '../../../services/academic-years.service';

import { Calendar } from '../../../models/calendar.model';
import { CalendarDay } from '../../../models/calendar-day.model';

import { Globals } from '../../../globals';

import { NgxSpinnerService } from 'ngx-spinner';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-administration-school-calendar',
  templateUrl: './administration-school-calendar.component.html',
  styleUrls: ['./administration-school-calendar.component.scss']
})
export class AdministrationSchoolCalendarComponent implements OnInit {
  private schoolCalendar: Calendar;
  private direction: number;
  private property: string;
  private isDescending: boolean;
  public days: CalendarDay[];
  public searchText: string;
  private calendarImportFormData: FormData = new FormData();
  public selectedAcademicYear: string;
  public selectedImportAcademicYear: string;
  public calendarSchoolYears: string[];

  constructor(
    private schoolCalendarService: SchoolCalendarService,
    private utilitiesService: UtilitiesService,
    private academicYearsService: AcademicYearsService,
    private ngxSpinnerService: NgxSpinnerService,
    private ngbModal: NgbModal,
    private globals: Globals
  ) {
    this.direction = 1;
    this.property = 'schoolDay';
  }

  ngOnInit() {
    this.schoolCalendarService.getAcademicYears().subscribe(
      data => {
        this.calendarSchoolYears = data['years'];
        this.selectedAcademicYear = this.calendarSchoolYears[0];
        console.log('AdministrationSchoolCalendarComponent.ngOnInit(): data is ', data['years']);

        this.schoolCalendarService.getByYear(this.selectedAcademicYear).subscribe(
          data2 => {
            console.log('AdministrationSchoolCalendarComponent.ngOnInit(): school calendar is ', data2['calendar']);
            this.schoolCalendar = data2['calendar'];
            this.days = this.schoolCalendar.days;
          },
          error => {
            console.log('AdministrationSchoolCalendarComponent.ngOnInit(): error is ', error);
          }
        );
      },
      error => {
        console.log('AdministrationSchoolCalendarComponent.ngOnInit():  error is ', error);
      }
    );
  }

  sort(property) {
    this.isDescending = !this.isDescending; // change the direction
    this.property = property;
    this.direction = this.isDescending ? 1 : -1;
  }

  getSortClass(property: string): object {
    return this.utilitiesService.getSortClass({ property: this.property, isDescending: this.isDescending }, property);
  }

  listDisplayableFields() {
    if (this.schoolCalendar && this.schoolCalendar.days && this.schoolCalendar.days.length > 0) {
      const rejected = ['id', 'lazyLoader'];
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

  public selectImportAcademicYear(year: string): void {
    this.selectedImportAcademicYear = year;
  }

  private doCalendarImport(): void {
    this.schoolCalendarService.updateByYear(this.selectedImportAcademicYear, this.calendarImportFormData).subscribe(
      data => {
        console.log('AdministrationSchoolCalendarComponent.handleFileSelection(): data is ', data['calendar']);
        this.schoolCalendar = data['calendar'];
        this.resetDays();
      },
      error => {
        console.log('AdministrationSchoolCalendarComponent.handleFileSelection(): error is ', error);
      }
    );
  }

  public filterCalendarSchoolYear(year: string): void {
    if (year) {
      this.schoolCalendarService.getByYear(year).subscribe(
        data => {
          console.log('AdministrationSchoolCalendarComponent.ngOnInit(): school calendar is ', data['calendar']);
          this.schoolCalendar = data['calendar'];
          this.days = this.schoolCalendar.days;
          this.selectedAcademicYear = year;
        },
        error => {
          console.log('AdministrationSchoolCalendar.filterCalendarSchoolYear():  error is ', error);
        }
      );
    }
  }

  handleFileSelection($event) {
    if ($event) {
      if ($event.target.files && $event.target.files.length > 0) {
        const files = $event.target.files;

        this.calendarImportFormData.append('file', files[0], files[0].name);
      }
    }
  }

  public importSchoolYearCalendar(importCalendarContent): void {
    const modal = this.ngbModal.open(importCalendarContent, { centered: true });
    modal.result.then(
      (result) => {
        this.doCalendarImport();
      },
      (response) => {
        console.log('AdministrationSchoolCalendarComponent.importSchoolYearCalendar():  response is ', response);
      }
    );
  }
}

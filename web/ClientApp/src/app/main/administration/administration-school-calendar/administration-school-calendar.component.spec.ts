import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import {
  AdministrationSchoolCalendarComponent
} from './administration-school-calendar.component';

describe('AdministrationSchoolCalendarComponent', () => {
  let component: AdministrationSchoolCalendarComponent;
  let fixture: ComponentFixture<AdministrationSchoolCalendarComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AdministrationSchoolCalendarComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AdministrationSchoolCalendarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

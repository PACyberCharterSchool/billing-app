import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentAdvancedFilterComponent } from './student-advanced-filter.component';
import { StudentDatepickerComponent } from '../student-datepicker/student-datepicker.component';

import { OrderByPipe } from '../../../pipes/orderby.pipe';
import { IepEnrolledPipe } from '../../../pipes/iep-enrolled.pipe';

describe('StudentAdvancedFilterComponent', () => {
  let component: StudentAdvancedFilterComponent;
  let fixture: ComponentFixture<StudentAdvancedFilterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StudentAdvancedFilterComponent, StudentDatepickerComponent, IepEnrolledPipe, OrderByPipe ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StudentAdvancedFilterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

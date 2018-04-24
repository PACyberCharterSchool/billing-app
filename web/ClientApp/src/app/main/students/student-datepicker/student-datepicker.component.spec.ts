import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentDatepickerComponent } from './student-datepicker.component';

describe('StudentDatepickerComponent', () => {
  let component: StudentDatepickerComponent;
  let fixture: ComponentFixture<StudentDatepickerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StudentDatepickerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StudentDatepickerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

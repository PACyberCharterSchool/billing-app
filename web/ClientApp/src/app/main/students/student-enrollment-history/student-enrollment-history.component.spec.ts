import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentEnrollmentHistoryComponent } from './student-enrollment-history.component';

describe('StudentEnrollmentHistoryComponent', () => {
  let component: StudentEnrollmentHistoryComponent;
  let fixture: ComponentFixture<StudentEnrollmentHistoryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StudentEnrollmentHistoryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StudentEnrollmentHistoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

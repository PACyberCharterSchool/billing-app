import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentActivityHistoryComponent } from './student-activity-history.component';

describe('StudentActivityHistoryComponent', () => {
  let component: StudentActivityHistoryComponent;
  let fixture: ComponentFixture<StudentActivityHistoryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StudentActivityHistoryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StudentActivityHistoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

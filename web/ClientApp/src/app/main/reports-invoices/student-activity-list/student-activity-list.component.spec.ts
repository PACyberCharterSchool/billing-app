import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentActivityListComponent } from './student-activity-list.component';

describe('StudentActivityListComponent', () => {
  let component: StudentActivityListComponent;
  let fixture: ComponentFixture<StudentActivityListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StudentActivityListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StudentActivityListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

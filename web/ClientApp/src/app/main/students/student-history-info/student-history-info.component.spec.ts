import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentHistoryInfoComponent } from './student-history-info.component';

describe('StudentHistoryInfoComponent', () => {
  let component: StudentHistoryInfoComponent;
  let fixture: ComponentFixture<StudentHistoryInfoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StudentHistoryInfoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StudentHistoryInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

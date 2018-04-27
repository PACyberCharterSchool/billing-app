import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentHistoryInfoComponent } from './student-history-info.component';
import { StudentAddressHistoryComponent } from '../student-address-history/student-address-history.component';
import { StudentEnrollmentHistoryComponent } from '../student-enrollment-history/student-enrollment-history.component';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

describe('StudentHistoryInfoComponent', () => {
  let component: StudentHistoryInfoComponent;
  let fixture: ComponentFixture<StudentHistoryInfoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [
        StudentHistoryInfoComponent,
        StudentAddressHistoryComponent,
        StudentEnrollmentHistoryComponent
      ],
      imports: [ NgbModule ]
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

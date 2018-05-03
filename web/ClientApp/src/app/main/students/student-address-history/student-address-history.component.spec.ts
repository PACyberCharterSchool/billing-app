import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentAddressHistoryComponent } from './student-address-history.component';

describe('StudentAddressHistoryComponent', () => {
  let component: StudentAddressHistoryComponent;
  let fixture: ComponentFixture<StudentAddressHistoryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StudentAddressHistoryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StudentAddressHistoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

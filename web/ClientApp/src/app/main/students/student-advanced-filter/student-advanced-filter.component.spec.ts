import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentAdvancedFilterComponent } from './student-advanced-filter.component';

describe('StudentAdvancedFilterComponent', () => {
  let component: StudentAdvancedFilterComponent;
  let fixture: ComponentFixture<StudentAdvancedFilterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StudentAdvancedFilterComponent ]
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

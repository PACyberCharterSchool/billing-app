import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AdministrationImportStudentDataComponent } from './administration-import-student-data.component';

describe('AdministrationImportStudentDataComponent', () => {
  let component: AdministrationImportStudentDataComponent;
  let fixture: ComponentFixture<AdministrationImportStudentDataComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AdministrationImportStudentDataComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AdministrationImportStudentDataComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RouterTestingModule } from '@angular/router/testing';

import { StudentDetailsInfoComponent } from './student-details-info.component';

import { CurrentStudentService } from '../../../services/current-student.service';

describe('StudentDetailsInfoComponent', () => {
  let component: StudentDetailsInfoComponent;
  let fixture: ComponentFixture<StudentDetailsInfoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StudentDetailsInfoComponent ],
      providers: [ CurrentStudentService ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StudentDetailsInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

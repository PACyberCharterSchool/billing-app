import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';

import { StudentDatepickerComponent } from './student-datepicker.component';

import { NgbModule, NgbCalendar, NgbDropdownConfig, NgbDateParserFormatter, NgbDateAdapter  } from '@ng-bootstrap/ng-bootstrap';

describe('StudentDatepickerComponent', () => {
  let component: StudentDatepickerComponent;
  let fixture: ComponentFixture<StudentDatepickerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StudentDatepickerComponent ],
      imports: [ FormsModule, NgbModule ],
      providers: [
        NgbDropdownConfig,
        NgbCalendar,
        NgbDateParserFormatter,
        NgbDateAdapter
      ]
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

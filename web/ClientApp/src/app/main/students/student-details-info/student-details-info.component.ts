import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { StudentRecord } from '../../../models/student-record.model';
import { SchoolDistrict } from '../../../models/school-district.model';
import { CurrentStudentService } from '../../../services/current-student.service';
import { SchoolDistrictService } from '../../../services/school-district.service';
import { StudentRecordsService } from '../../../services/student-records.service';
import { Observable } from 'rxjs/Observable';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { validateZipCode } from '../../../validators/zip';
import { validateState } from '../../../validators/state';
import { validateGrade } from '../../../validators/grade';
import { States } from '../../../models/states';
import { Grades } from '../../../models/grades';

@Component({
  selector: 'app-student-details-info',
  templateUrl: './student-details-info.component.html',
  styleUrls: ['./student-details-info.component.scss']
})
export class StudentDetailsInfoComponent implements OnInit {
  public student: StudentRecord;
  public studentDetailForm: FormGroup;
  public spedStatuses = [
    'Yes',
    'No'
  ];
  public schoolDistricts: SchoolDistrict[];
  public selectedSchoolDistrict: SchoolDistrict;
  public scope: string;
  public updateOpMessage: string;
  public states = States.map(s => s.abbreviation);
  public grades = Grades;

  constructor(
    private currentStudentService: CurrentStudentService,
    private schoolDistrictService: SchoolDistrictService,
    private studentRecordsService: StudentRecordsService,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private ngbModal: NgbModal,
    private fb: FormBuilder
  ) { }

  ngOnInit() {
    this.activatedRoute.params.subscribe(
      (params) => {
        this.scope = params['scope'];
      },
      (stuff) => {
      }
    );

    this.currentStudentService.currentStudent.subscribe(s => {
      this.student = s;
    });

    this.schoolDistrictService.getSchoolDistricts().subscribe(
      data => {
        this.schoolDistricts = data.schoolDistricts;
      },
      error => {
        console.log('StudentDetailsInfoComponent.ngOnInit(): error is ', error);
      }
    );

    this.initStudentDetailsForm();
  }

  initStudentDetailsForm(): void {
    this.studentDetailForm = this.fb.group({
      personalInfo: this.fb.group({
        firstName: this.fb.control(this.student.studentFirstName, Validators.required),
        lastName: this.fb.control(this.student.studentLastName, Validators.required),
        middleInitial: this.fb.control(this.student.studentMiddleInitial),
        dateOfBirth: this.fb.control(this.student.studentDateOfBirth, Validators.required)
      }),
      addressInfo: this.fb.group({
        street1: this.fb.control(this.student.studentStreet1, Validators.required),
        street2: this.fb.control(this.student.studentStreet2),
        city: this.fb.control(this.student.studentCity, Validators.required),
        state: this.fb.control(this.student.studentState, Validators.compose([Validators.required, validateState])),
        zip: this.fb.control(this.student.studentZipCode, Validators.compose([Validators.required, validateZipCode]))
      }),
      studentInfo: this.fb.group({
        paSecuredId: this.fb.control(this.student.studentPaSecuredId, Validators.required),
        gradeLevel: this.fb.control(this.student.studentGradeLevel, Validators.compose([Validators.required, validateGrade])),
        enrollmentDate: this.fb.control(this.student.studentEnrollmentDate, Validators.required),
        withdrawalDate: this.fb.control(this.student.studentWithdrawalDate),
        spedStatus: this.fb.control(this.student.studentIsSpecialEducation),
        currentIepDate: this.fb.control(this.student.studentCurrentIep),
        formerIepDate: this.fb.control(this.student.studentFormerIep),
        schoolDistrict: this.fb.control(this.student.schoolDistrictName, Validators.required)
      })
    });

    if (this.student.studentDateOfBirth !== null) {
      this.studentDetailForm.get('personalInfo.dateOfBirth').setValue({
        year: new Date(this.student.studentDateOfBirth).getFullYear(),
        month: new Date(this.student.studentDateOfBirth).getMonth() + 1,
        day: new Date(this.student.studentDateOfBirth).getDate()
      });
    }

    if (this.student.studentEnrollmentDate !== null) {
      this.studentDetailForm.get('studentInfo.enrollmentDate').setValue({
        year: new Date(this.student.studentEnrollmentDate).getFullYear(),
        month: new Date(this.student.studentEnrollmentDate).getMonth() + 1,
        day: new Date(this.student.studentEnrollmentDate).getDate()
      });
    }

    if (this.student.studentWithdrawalDate !== null) {
      this.studentDetailForm.get('studentInfo.withdrawalDate').setValue({
        year: new Date(this.student.studentWithdrawalDate).getFullYear(),
        month: new Date(this.student.studentWithdrawalDate).getMonth() + 1,
        day: new Date(this.student.studentWithdrawalDate).getDate()
      });
    }

    if (this.student.studentCurrentIep !== null) {
      this.studentDetailForm.get('studentInfo.currentIepDate').setValue({
        year: new Date(this.student.studentCurrentIep).getFullYear(),
        month: new Date(this.student.studentCurrentIep).getMonth() + 1,
        day: new Date(this.student.studentCurrentIep).getDate()
      });
    }

    if (this.student.studentFormerIep !== null) {
      this.studentDetailForm.get('studentInfo.formerIepDate').setValue({
        year: new Date(this.student.studentFormerIep).getFullYear(),
        month: new Date(this.student.studentFormerIep).getMonth() + 1,
        day: new Date(this.student.studentFormerIep).getDate()
      });
    }

    this.studentDetailForm.get('studentInfo.spedStatus').setValue(this.student.studentIsSpecialEducation);
    this.studentDetailForm.get('studentInfo.schoolDistrict').setValue(this.student.schoolDistrictName);
  }

  public setSelectedSchoolDistrict($event) {
    this.selectedSchoolDistrict = this.schoolDistricts.find((sd) => sd.name === $event.item);
  }

  search = (text$: Observable<string>) => {
    const results = text$.pipe(
      debounceTime(200),
      distinctUntilChanged(),
      map((term) => {
        return term.length < 2 ? [] : this.schoolDistricts.filter(
          (sd) => {
            if (sd.name.toLowerCase().indexOf(term.toLowerCase()) > -1) {
              return true;
            } else {
              return false;
            }
          }).map((sd) => sd.name);
      })
    );

    return results;
  }

  updateStudentInfo(confirmationDlgContent): void {
    let month: number;
    let day: number;
    let year: number;

    this.student.studentFirstName = this.studentDetailForm.get('personalInfo.firstName').value;
    this.student.studentLastName = this.studentDetailForm.get('personalInfo.lastName').value;
    this.student.studentMiddleInitial = this.studentDetailForm.get('personalInfo.middleInitial').value;

    if (this.studentDetailForm.get('personalInfo.dateOfBirth').dirty) {
      if (this.studentDetailForm.get('personalInfo.dateOfBirth').value) {
        month = this.studentDetailForm.get('personalInfo.dateOfBirth').value.month;
        day = this.studentDetailForm.get('personalInfo.dateOfBirth').value.day;
        year = this.studentDetailForm.get('personalInfo.dateOfBirth').value.year;
        this.student.studentDateOfBirth = new Date(`${month}/${day}/${year}`);
      }
    }

    this.student.studentStreet1 = this.studentDetailForm.get('addressInfo.street1').value;
    this.student.studentStreet2 = this.studentDetailForm.get('addressInfo.street2').value;
    this.student.studentCity = this.studentDetailForm.get('addressInfo.city').value;
    this.student.studentState = this.studentDetailForm.get('addressInfo.state').value;
    this.student.studentZipCode = this.studentDetailForm.get('addressInfo.zip').value;

    this.student.schoolDistrictName = this.studentDetailForm.get('studentInfo.schoolDistrict').value;
    this.student.schoolDistrictId =
      this.schoolDistricts.find(
        (s) => s.name.replace(/\s+/g, '').toLowerCase() === this.student.schoolDistrictName.replace(/\s+/g, '').toLowerCase()).id;

    if (this.studentDetailForm.get('studentInfo.currentIepDate').dirty) {
      if (!this.studentDetailForm.get('studentInfo.currentIepDate').value) {
        this.student.studentCurrentIep = null;
      } else {
        month = this.studentDetailForm.get('studentInfo.currentIepDate').value.month;
        day = this.studentDetailForm.get('studentInfo.currentIepDate').value.day;
        year = this.studentDetailForm.get('studentInfo.currentIepDate').value.year;
        this.student.studentCurrentIep = new Date(`${month}/${day}/${year}`);
      }
    }

    if (this.studentDetailForm.get('studentInfo.formerIepDate').dirty) {
      if (!this.studentDetailForm.get('studentInfo.formerIepDate').value) {
        this.student.studentFormerIep = null;
      } else {
        month = this.studentDetailForm.get('studentInfo.formerIepDate').value.month;
        day = this.studentDetailForm.get('studentInfo.formerIepDate').value.day;
        year = this.studentDetailForm.get('studentInfo.formerIepDate').value.year;
        this.student.studentFormerIep = new Date(`${month}/${day}/${year}`);
      }
    }

    if (this.studentDetailForm.get('studentInfo.enrollmentDate').dirty) {
      if (this.studentDetailForm.get('studentInfo.enrollmentDate').value) {
        month = this.studentDetailForm.get('studentInfo.enrollmentDate').value.month;
        day = this.studentDetailForm.get('studentInfo.enrollmentDate').value.day;
        year = this.studentDetailForm.get('studentInfo.enrollmentDate').value.year;
        this.student.studentEnrollmentDate = new Date(`${month}/${day}/${year}`);
      }
    }

    if (this.studentDetailForm.get('studentInfo.withdrawalDate').dirty) {
      if (!this.studentDetailForm.get('studentInfo.withdrawalDate').value) {
        this.student.studentWithdrawalDate = null;
      } else {
        month = this.studentDetailForm.get('studentInfo.withdrawalDate').value.month;
        day = this.studentDetailForm.get('studentInfo.withdrawalDate').value.day;
        year = this.studentDetailForm.get('studentInfo.withdrawalDate').value.year;
        this.student.studentWithdrawalDate = new Date(`${month}/${day}/${year}`);
      }
    }

    this.student.studentGradeLevel = this.studentDetailForm.get('studentInfo.gradeLevel').value;
    this.student.studentPaSecuredId = this.studentDetailForm.get('studentInfo.paSecuredId').value;
    this.student.studentIsSpecialEducation = this.studentDetailForm.get('studentInfo.spedStatus').value;

    this.studentRecordsService.updateStudentRecord(this.scope, this.student).subscribe(
      data => {
        console.log('StudentDetailsInfoComponent.updateStudentInfo(): data is ', data);
        this.updateOpMessage = `Student update for student ${this.student.studentFirstName}
         ${this.student.studentLastName} was successful`;
        this.ngbModal.open(confirmationDlgContent).result.then(
          result => {
            this.router.navigate(['/students', { outlets: { 'action': ['list'] } }]);
          },
          reason => {
          }
        );
      },
      error => {
        console.log('StudentDetailsInfoComponent.updateStudentInfo(): error is ', error);
        this.updateOpMessage = `Failed to update student ${this.student.studentFirstName}
           ${this.student.studentLastName}.  Error: ` + error;
        this.ngbModal.open(confirmationDlgContent).result.then(
          result => {
            this.router.navigate(['/students', { outlets: { 'action': ['list'] } }]);
          },
          reason => {
          }
        );
      }
    );
  }

  setSelectedState(state: string): void {
    this.studentDetailForm.get('addressInfo.state').setValue(state);
  }

  setSelectedGrade(grade: string): void {
    this.studentDetailForm.get('studentInfo.gradeLevel').setValue(grade);
  }
}

import { Component, Input, OnInit } from '@angular/core';

import { ActivatedRoute, Params } from '@angular/router';

import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';

import { StudentRecord } from '../../../models/student-record.model';
import { SchoolDistrict } from '../../../models/school-district.model';

import { CurrentStudentService } from '../../../services/current-student.service';
import { SchoolDistrictService } from '../../../services/school-district.service';
import { StudentRecordsService } from '../../../services/student-records.service';

import { Observable } from 'rxjs/Observable';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';

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

  constructor(
    private currentStudentService: CurrentStudentService,
    private schoolDistrictService: SchoolDistrictService,
    private studentRecordsService: StudentRecordsService,
    private activatedRoute: ActivatedRoute,
    private fb: FormBuilder
  ) { }

  ngOnInit() {
    console.log('StudentDetailsInfoComponent.ngOnInit(): route is ', this.activatedRoute.url);
    this.activatedRoute.params.subscribe(
      (params) => {
        this.scope = params['scope'];
      }
    );

    this.currentStudentService.currentStudent.subscribe(s => {
      this.student = s;
    });
    console.log('StudentDetailsInfoComponent.ngOnInit(): student is ', this.student);

    this.schoolDistrictService.getSchoolDistricts().subscribe(
      data => {
        console.log('StudentDetailsInfoComponent.ngOnInit():  data is ', data);
        this.schoolDistricts = data['schoolDistricts'];
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
        firstName: this.fb.control(this.student.studentFirstName),
        lastName: this.fb.control(this.student.studentLastName),
        middleInitial: this.fb.control(this.student.studentMiddleInitial),
        dateOfBirth: this.fb.control(this.student.studentDateOfBirth)
      }),
      addressInfo: this.fb.group({
        street1: this.fb.control(this.student.studentStreet1),
        street2: this.fb.control(this.student.studentStreet2),
        city: this.fb.control(this.student.studentCity),
        state: this.fb.control(this.student.studentState),
        zip: this.fb.control(this.student.studentZipCode)
      }),
      studentInfo: this.fb.group({
        paSecuredId: this.fb.control(this.student.studentPaSecuredId),
        gradeLevel: this.fb.control(this.student.studentGradeLevel),
        enrollmentDate: this.fb.control(this.student.studentEnrollmentDate),
        withdrawalDate: this.fb.control(this.student.studentWithdrawalDate),
        spedStatus: this.fb.control(this.student.studentIsSpecialEducation),
        currentIepDate: this.fb.control(this.student.studentCurrentIep),
        formerIepDate: this.fb.control(this.student.studentFormerIep),
        schoolDistrict: this.fb.control(this.student.schoolDistrictName)
      })
    });

    this.studentDetailForm.get('personalInfo.dateOfBirth').setValue({
      year: new Date(this.student.studentDateOfBirth).getFullYear(),
      month: new Date(this.student.studentDateOfBirth).getMonth(),
      day: new Date(this.student.studentDateOfBirth).getDate()
    });

    this.studentDetailForm.get('studentInfo.enrollmentDate').setValue({
      year: new Date(this.student.studentEnrollmentDate).getFullYear(),
      month: new Date(this.student.studentEnrollmentDate).getMonth(),
      day: new Date(this.student.studentEnrollmentDate).getDate()
    });

    this.studentDetailForm.get('studentInfo.withdrawalDate').setValue({
      year: new Date(this.student.studentWithdrawalDate).getFullYear(),
      month: new Date(this.student.studentWithdrawalDate).getMonth(),
      day: new Date(this.student.studentWithdrawalDate).getDate()
    });

    this.studentDetailForm.get('studentInfo.currentIepDate').setValue({
      year: new Date(this.student.studentCurrentIep).getFullYear(),
      month: new Date(this.student.studentCurrentIep).getMonth(),
      day: new Date(this.student.studentCurrentIep).getDate()
    });

    this.studentDetailForm.get('studentInfo.formerIepDate').setValue({
      year: new Date(this.student.studentFormerIep).getFullYear(),
      month: new Date(this.student.studentFormerIep).getMonth(),
      day: new Date(this.student.studentFormerIep).getDate()
    });

    this.studentDetailForm.get('studentInfo.spedStatus').setValue(this.student.studentIsSpecialEducation);
    this.studentDetailForm.get('studentInfo.schoolDistrict').setValue(this.student.schoolDistrictName);
  }

  setSelectedSchoolDistrict($event) {
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

  updateStudentInfo(): void {
    let month: number;
    let day: number;
    let year: number;

    this.student.studentFirstName = this.studentDetailForm.get('personalInfo.firstName').value;
    this.student.studentLastName = this.studentDetailForm.get('personalInfo.lastName').value;
    this.student.studentMiddleInitial = this.studentDetailForm.get('personalInfo.middleInitial').value;

    month = this.studentDetailForm.get('personalInfo.dateOfBirth').value.month;
    day = this.studentDetailForm.get('personalInfo.dateOfBirth').value.day;
    year = this.studentDetailForm.get('personalInfo.dateOfBirth').value.year;
    this.student.studentDateOfBirth = new Date(`${month}/${day}/${year}`);

    this.student.studentStreet1 = this.studentDetailForm.get('addressInfo.street1').value;
    this.student.studentStreet2 = this.studentDetailForm.get('addressInfo.street2').value;
    this.student.studentCity = this.studentDetailForm.get('addressInfo.city').value;
    this.student.studentState = this.studentDetailForm.get('addressInfo.state').value;
    this.student.studentZipCode = this.studentDetailForm.get('addressInfo.zip').value;

    this.student.schoolDistrictName = this.studentDetailForm.get('studentInfo.schoolDistrict').value;
    this.student.schoolDistrictId =
      this.schoolDistricts.find(
        (s) => s.name.toLowerCase() === this.student.schoolDistrictName.toLowerCase()).id;

    month = this.studentDetailForm.get('studentInfo.currentIepDate').value.month;
    day = this.studentDetailForm.get('studentInfo.currentIepDate').value.day;
    year = this.studentDetailForm.get('studentInfo.currentIepDate').value.year;
    this.student.studentCurrentIep = new Date(`${month}/${day}/${year}`);

    month = this.studentDetailForm.get('studentInfo.formerIepDate').value.month;
    day = this.studentDetailForm.get('studentInfo.formerIepDate').value.day;
    year = this.studentDetailForm.get('studentInfo.formerIepDate').value.year;
    this.student.studentFormerIep = new Date(`${month}/${day}/${year}`);

    month = this.studentDetailForm.get('studentInfo.enrollmentDate').value.month;
    day = this.studentDetailForm.get('studentInfo.enrollmentDate').value.day;
    year = this.studentDetailForm.get('studentInfo.enrollmentDate').value.year;
    this.student.studentEnrollmentDate = new Date(`${month}/${day}/${year}`);

    month = this.studentDetailForm.get('studentInfo.withdrawalDate').value.month;
    day = this.studentDetailForm.get('studentInfo.withdrawalDate').value.day;
    year = this.studentDetailForm.get('studentInfo.withdrawalDate').value.year;
    this.student.studentWithdrawalDate = new Date(`${month}/${day}/${year}`);

    this.student.studentGradeLevel = this.studentDetailForm.get('studentInfo.gradeLevel').value;
    this.student.studentPaSecuredId = this.studentDetailForm.get('studentInfo.paSecuredId').value;
    this.student.studentIsSpecialEducation = this.studentDetailForm.get('studentInfo.spedStatus').value;

    this.studentRecordsService.updateStudentRecord(this.scope, this.student).subscribe(
      data => {
        console.log('StudentDetailsInfoComponent.updateStudentInfo(): data is ', data);
      },
      error => {
        console.log('StudentDetailsInfoComponent.updateStudentInfo(): error is ', error);
      }
    );
  }
}

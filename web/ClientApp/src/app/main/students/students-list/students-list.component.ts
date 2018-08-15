import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { Observable } from 'rxjs/Observable';

import { Globals } from '../../../globals';

import { StudentRecord } from '../../../models/student-record.model';
import { SchoolDistrict } from '../../../models/school-district.model';

import { StudentsService } from '../../../services/students.service';
import { StudentRecordsService } from '../../../services/student-records.service';
import { SchoolDistrictService } from '../../../services/school-district.service';
import { CurrentStudentService } from '../../../services/current-student.service';
import { UtilitiesService } from '../../../services/utilities.service';

import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-students-list',
  templateUrl: './students-list.component.html',
  styleUrls: ['./students-list.component.scss']
})
export class StudentsListComponent implements OnInit {

  public studentRecords: StudentRecord[];
  public allStudentRecords: StudentRecord[];
  private schoolDistricts: SchoolDistrict[];
  public scopes: string[];
  public currentScope: string;
  public advancedSearchEnabled: boolean;
  public searchText: string;
  public isDescending: boolean;
  public property: string;
  public direction: number;
  public startDate: Date;
  public endDate: Date;
  private selectedStudent: StudentRecord;
  private skip: number;
  private retrievingStudents: boolean;
  public canEdit: boolean;

  constructor(
    private globals: Globals,
    private studentsService: StudentsService,
    private studentRecordsService: StudentRecordsService,
    private schoolDistrictService: SchoolDistrictService,
    private currentStudentService: CurrentStudentService,
    private utilitiesService: UtilitiesService,
    private spinnerService: NgxSpinnerService,
    private router: Router) {
      this.advancedSearchEnabled = false;
      this.isDescending = false;
      this.property = 'paCyberId';
      this.direction = 1;
      this.skip = 0;
      this.studentRecords = this.allStudentRecords = [];
      this.scopes = [];
    }

  ngOnInit() {
    if (this.allStudentRecords && this.allStudentRecords.length <= 0) {
      this.studentRecordsService.getStudentRecordsHeaders().subscribe(
        data => {
          console.log('StudentsListComponent.ngOnInit(): data is ', data['scopes']);
          this.scopes = data['scopes'];
          if (this.scopes.length > 0) {
            this.currentScope = this.scopes[0];
            this.filterByStudentRecordScope(this.currentScope);
          } else {
            this.currentScope = 'Select scope...';
          }
        },
        error => {
          console.log('StudentsListComponent.ngOnInit():  error is ', error);
          this.currentScope = 'Select scope...';
        }
      );
    }

    if (this.schoolDistricts && this.schoolDistricts.length <= 0) {
      this.schoolDistrictService.getSchoolDistricts().subscribe(
        data => {
          this.schoolDistricts = data['schoolDistricts'];
          console.log('StudentsListComponent.ngOnInit():  school districts are', this.schoolDistricts);
        },
        error => {
          console.log('StudentsListComponent.ngOnInit():  error is ', error);
        }
      );
    }

    this.currentStudentService.currentStudent.subscribe((student) => this.selectedStudent = student, (error) => error);
  }

  public dateSelectedStartDateHandler(date: Date): void {
    this.startDate = date;
    this.studentRecords = this.allStudentRecords.filter((sr) => {
      if (this.endDate) {
        if (sr.studentEnrollmentDate >= this.startDate && sr.studentEnrollmentDate <= this.endDate) {
          return true;
        }
      }

      if (sr.studentEnrollmentDate <= date) {
        return true;
      }

      return false;
    });
  }

  public dateSelectedEndDateHandler(date: Date): void {
    this.endDate = date;
    this.studentsService.getStudentsFilteredByEndDate(date).subscribe(
      data => {
        this.studentRecords = data['students'];
      },
      error => {
        console.log('error');
      }
    );
  }

  public studentsUpdatedHandler(students: StudentRecord[]): void {
    this.studentRecords = students;
    console.log('StudentsListComponent.studentsUpdatedHandler():  students are ', this.studentRecords);
  }

  public handleCommitClick(): void {
    this.spinnerService.show();
    this.studentRecordsService.postLockStudentData(this.currentScope).subscribe(
      response => {
        this.spinnerService.hide();
      },
      error => {
        this.spinnerService.hide();
      }
    );
  }

  public filterByStudentRecordScope(scope: string): void {
    this.currentScope = scope;
    this.spinnerService.show();
    this.studentRecordsService.getStudentRecordsHeaderByScope(this.currentScope, this.skip).subscribe(
      data => {
        this.studentRecords = data['header']['records'];
        this.canEdit = data['header']['locked'];
        console.log('AdministrationImportStudentDataComponent.ngOnInit():  data is ', data);
        this.spinnerService.hide();
      },
      error => {
        console.log('AdministrationImportStudentDataComponent.ngOnInit():  error is ', error);
        this.spinnerService.hide();
      }
    );
  }

  getStudents($event) {
    this.studentsService.getStudents(this.skip).subscribe(
      data => {
        this.studentRecords = this.studentRecords.concat(data['students']);
        console.log('StudentsListComponent.getStudents():  students are ', this.studentRecords);
      },
      error => {
        console.log('error');
      }
    );
  }

  sort(property) {
    this.isDescending = !this.isDescending; // change the direction
    this.property = property;
    this.direction = this.isDescending ? 1 : -1;
  }

  showStudentDetails(studentRecord: StudentRecord) {
    console.log('StudentsListComponent.showStudentDetails():  studentId is ', studentRecord.id);
    if (this.canEdit) {
      const s: StudentRecord = this.studentRecords.find((student) => student.id === studentRecord.id);
      this.currentStudentService.changeStudent(s);
      this.router.navigate(['/students', { id: studentRecord.id, outlets: {'action': [`${studentRecord.id}`]} }]);
    }
  }

  toggleAdvancedSearchTools() {
    this.advancedSearchEnabled = !this.advancedSearchEnabled;
  }

  resetStudentList() {
    this.studentsService.getStudents(this.skip).subscribe(
      data => {
        this.studentRecords = this.allStudentRecords = data['students'];
      },
      error => {
        console.log('StudentsListComponent.resetStudentList(): error is ', error);
        this.studentRecords = this.allStudentRecords = [];
      }
    );
    this.searchText = '';
  }

  filterStudentListByNameOrId() {
    if (this.searchText) {
      this.studentRecords = this.studentRecords.filter((s) => {
        if (s.studentLastName.toLowerCase().includes(this.searchText.toLowerCase()) ||
          s.studentId.toLowerCase().includes(this.searchText.toLowerCase()) ||
          s.studentFirstName.toLowerCase().includes(this.searchText.toLowerCase()) ||
          s.schoolDistrictName.toLowerCase().includes(this.searchText.toLowerCase()) ||
          s.studentState.toLowerCase().includes(this.searchText.toLowerCase()) ||
          s.studentStreet1.toLowerCase().includes(this.searchText.toLowerCase()) ||
          s.studentStreet2.toLowerCase().includes(this.searchText.toLowerCase()) ||
          s.studentCity.toLowerCase().includes(this.searchText.toLowerCase())) {
            return true;
        }

        return false;
      });
    }
  }

  onScroll($event) {
    if (!this.retrievingStudents) {
      this.getStudents($event);
    }
  }

  private updateScrollingSkip() {
    this.skip += this.globals.take;
  }

  listDisplayableFields() {
    const fields = this.utilitiesService.objectKeys(this.studentRecords[0]);
    const rejected = [
      'id',
      'studentMiddleInitial',
      'studentState',
      'studentNorep',
      'lastUpdated',
      'lazyLoader',
      'activitySchoolYear',
      'studentPaSecuredId'
    ];

    if (fields) {
      const filtered = fields.filter((i) => !rejected.includes(i));

      return filtered;
    }
  }

  listDisplayableValues(activity) {
    const vkeys = this.listDisplayableFields();
    const selected = this.utilitiesService.pick(activity, vkeys);

    return this.utilitiesService.objectValues(selected);
  }
}

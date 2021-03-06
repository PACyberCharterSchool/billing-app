import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { Globals } from '../../../globals';
import { StudentRecord } from '../../../models/student-record.model';
import { SchoolDistrict } from '../../../models/school-district.model';
import { StudentsService } from '../../../services/students.service';
import { StudentRecordsService } from '../../../services/student-records.service';
import { SchoolDistrictService } from '../../../services/school-district.service';
import { CurrentStudentService } from '../../../services/current-student.service';
import { UtilitiesService } from '../../../services/utilities.service';
import { StudentAdvancedFilterComponent } from '../student-advanced-filter/student-advanced-filter.component';
import { NgxSpinnerService } from 'ngx-spinner';
import { or, has } from '../../../models/querydsl.model';

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
  public currentScopeCommitState: string;

  @ViewChild('advancedStudentFilter')
  private advancedStudentFilterComponent: StudentAdvancedFilterComponent;

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
    this.property = 'schoolDistrictName';
    this.direction = -1;
    this.skip = 0;
    this.studentRecords = this.allStudentRecords = [];
    this.scopes = [];
  }

  ngOnInit() {
    this.studentRecordsService.getHeaders(null).subscribe(
      data => {
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

    this.schoolDistrictService.getSchoolDistricts().subscribe(
      data => {
        this.schoolDistricts = data['schoolDistricts'];
        this.schoolDistricts = this.schoolDistricts.sort((a, b) => a.name.localeCompare(b.name));
      },
      error => {
        console.log('StudentsListComponent.ngOnInit():  error is ', error);
      }
    );

    this.currentStudentService.currentStudent.subscribe((student) => this.selectedStudent = student, (error) => error);
  }

  public studentsUpdatedHandler(students: StudentRecord[]): void {
    this.studentRecords = students;
  }

  public handleCommitClick(): void {
    this.spinnerService.show();
    this.studentRecordsService.postLockStudentData(this.currentScope).subscribe(
      response => {
        this.spinnerService.hide();
        this.resetStudentList();
      },
      error => {
        this.spinnerService.hide();
      }
    );
  }

  public filterByStudentRecordScope(scope: string): void {
    this.currentScope = scope;
    this.spinnerService.show();
    this.studentRecordsService.getHeaderByScope(this.currentScope, this.skip).subscribe(
      data => {
        this.studentRecords = data['header']['records'];
        this.canEdit = data['header']['locked'];
        this.currentScopeCommitState = this.canEdit ? 'Committed' : 'Not committed';
        this.spinnerService.hide();
      },
      error => {
        console.log('AdministrationImportStudentDataComponent.ngOnInit():  error is ', error);
        this.spinnerService.hide();
      }
    );
  }

  getStudents($event) {
    this.studentRecordsService.getHeaderByScope(this.currentScope, this.skip).subscribe(
      data => {
        this.studentRecords = this.studentRecords.concat(data['header']['records']);
        this.retrievingStudents = false;
        this.updateScrollingSkip();
      },
      error => {
        console.log('error');
        this.retrievingStudents = false;
      }
    );
  }

  sort(property: string): void {
    this.isDescending = !this.isDescending; // change the direction
    this.property = property;
    this.direction = this.isDescending ? 1 : -1;
  }

  getSortClass(property: string): object {
    return this.utilitiesService.getSortClass({ property: this.property, isDescending: this.isDescending }, property);
  }

  showStudentDetails(studentRecord: StudentRecord) {
    if (this.canEdit) {
      const s: StudentRecord = this.studentRecords.find((student) => student.id === studentRecord.id);
      this.currentStudentService.changeStudent(s);
      this.router.navigate(['/students', { outlets: { 'action': [this.currentScope, studentRecord.id] } }]);
    }
  }

  toggleAdvancedSearchTools() {
    this.advancedSearchEnabled = !this.advancedSearchEnabled;
  }

  private initAllFilterControls(): void {
    this.searchText = '';
    this.startDate = null;
    this.endDate = null;
  }

  resetStudentList() {
    this.studentRecordsService.getHeaders(null).subscribe(
      data => {
        this.scopes = data['scopes'];
        if (this.scopes.length > 0) {
          this.currentScope = this.scopes[0];
          this.filterByStudentRecordScope(this.currentScope);
        } else {
          this.currentScope = 'Select scope...';
        }
      },
      error => {
        console.log('StudentsListComponent.resetStudentList():  error is ', error);
        this.currentScope = 'Select scope...';
      }
    );

    this.initAllFilterControls();
    this.advancedStudentFilterComponent.initAllFilterControls();
  }

  filterStudentListByNameOrId() {
    const filter = or(
      has('schoolDistrictName', this.searchText),
      or(
        has('studentId', this.searchText),
        or(
          has('studentFirstName', this.searchText),
          or(
            has('studentLastName', this.searchText),
            or(
              has('studentGradeLevel', this.searchText),
              or(
                has('studentStreet1', this.searchText),
                or(
                  has('studentStreet2', this.searchText),
                  has('studentCity', this.searchText),
                )
              )
            )
          )
        )
      )
    );

    if (this.searchText) {
      this.studentRecordsService.getHeadersByFilter(this.currentScope, filter).subscribe(
        data => {
          this.studentRecords = data['header']['records'];
        },
        error => {
          console.log('StudentListComponent.filterStudentsByNameOrId(): error is ', error);
        }
      );
    }
  }

  onScroll($event) {
    if (!this.retrievingStudents) {
      this.retrievingStudents = true;
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
      'studentPaSecuredId',
      'schoolDistrictId',
      'studentStreet2'
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

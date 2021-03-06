import { Component, OnInit } from '@angular/core';

import { UtilitiesService } from '../../../services/utilities.service';
import { AuditRecordsService } from '../../../services/audit-records.service';

import { Globals } from '../../../globals';

import { AuditRecord, AuditRecordActivityType } from '../../../models/audit-record.model';

@Component({
  selector: 'app-administration-audit-list',
  templateUrl: './administration-audit-list.component.html',
  styleUrls: ['./administration-audit-list.component.scss']
})
export class AdministrationAuditListComponent implements OnInit {

  public auditTypes;
  public property: string;
  public direction: number;
  public isDescending: boolean;
  private allAudits: AuditRecord[];
  public audits: AuditRecord[];
  private skip: number;
  private collapsed: boolean;
  private selectedAuditRecord: AuditRecord;
  public searchText: string;
  public column: any;
  public k: any;
  public selectedAuditActivityType: string;

  constructor(
    private globals: Globals,
    private utilitiesService: UtilitiesService,
    private auditRecordService: AuditRecordsService
  ) {
    this.property = 'timestamp';
    this.isDescending = true;
    this.direction = 1;
    this.allAudits = this.audits = [];
    this.skip = 0;
    this.collapsed = true;
  }

  ngOnInit() {
    this.auditRecordService.getAll(this.skip).subscribe(
      data => {
        console.log('AdministrationAuditListComponent.ngOnInit(): data is ', data);
        this.allAudits = this.audits = data['audits'];
        this.initAuditTypes();
      }
    );
  }

  initAuditTypes() {
    const atypes = this.allAudits.map((a) => a.activity);
    this.auditTypes = this.utilitiesService.uniqueItemsInArray(atypes);
  }

  sort(property) {
    this.isDescending = !this.isDescending; // change the direction
    this.property = property;
    this.direction = this.isDescending ? 1 : -1;
  }

  getSortClass(property: string): object {
    return this.utilitiesService.getSortClass({ property: this.property, isDescending: this.isDescending }, property);
  }

  filterAuditRecordsBySearch() {
    this.audits = this.allAudits.filter(
      (i) => {
        const re = new RegExp(this.searchText, 'gi');
        if (
          i.activity.search(re) !== -1 ||
          i.username.search(re) !== -1
        ) {
          return true;
        }
        return false;
      }
    );
  }

  filterAuditRecordsBySelectedAuditType(type: any) {
    this.selectedAuditActivityType = type;
    this.audits = this.allAudits.filter((a) => a.activity === type);
  }

  resetAuditRecords() {
    this.audits = this.allAudits;
    this.searchText = '';
  }

  listDisplayableFields() {
    if (this.allAudits) {
      const rejected = ['next', 'previous', 'type', 'id'];
      const fields = this.utilitiesService.objectKeys(this.allAudits[0]);
      if (fields) {
        return fields.filter((i) => !rejected.includes(i));
      }
    }
  }

  listDisplayableValues(auditRecord) {
    if (auditRecord) {
      const vkeys = this.listDisplayableFields();
      const selected = this.utilitiesService.pick(auditRecord, vkeys);

      return this.utilitiesService.objectValues(selected);
    }
  }

  setSelectedAuditRecord(ar: AuditRecord) {
    this.selectedAuditRecord = ar;
  }

  isExpandedAuditRecord(ar: AuditRecord): boolean {
    if (ar && this.selectedAuditRecord) {
      if ((ar.id === this.selectedAuditRecord.id) && !this.collapsed) {
        return true;
      }
    }
    return false;
  }

  getExpandedClass(ar: AuditRecord): object {
    return {
      'fa-angle-left': !this.isExpandedAuditRecord(ar),
      'fa-angle-down': this.isExpandedAuditRecord(ar),
    };
  }
}

import { Component, OnInit } from '@angular/core';

import { UtilitiesService } from '../../../services/utilities.service';
import { AuditRecordsService } from '../../../services/audit-records.service';

import { Globals } from '../../../globals';

import { AuditRecord, AuditRecordType } from '../../../models/audit-record.model';

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

  constructor(
    private globals: Globals,
    private utilitiesService: UtilitiesService,
    private auditRecordService: AuditRecordsService
  ) {
    this.property = 'date';
    this.direction = 1;
    this.allAudits = this.audits = [];
    this.skip = 0;
    this.collapsed = true;
  }

  ngOnInit() {
    this.auditRecordService.getAll(this.skip).subscribe(
      data => {
        console.log('AdministrationAuditListComponent.ngOnInit(): data is ', data);
        this.allAudits = this.audits = data;
        this.initAuditTypes();
      }
    );
  }

  initAuditTypes() {
    const atypes = this.allAudits.map((a) => a.type);
    this.auditTypes = this.utilitiesService.uniqueItemsInArray(atypes);
  }

  sort(property) {
    this.isDescending = !this.isDescending; // change the direction
    this.property = property;
    this.direction = this.isDescending ? 1 : -1;
  }

  filterAuditRecordsBySearch() {
    this.audits= this.allAudits.filter(
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
    this.audits = this.allAudits.filter((a) => a.type === type);
  }

  resetAuditRecords() {
    this.audits = this.allAudits;
    this.searchText = '';
  }

  listDisplayableFields() {
    if (this.allAudits) {
      const rejected = ['oldValue', 'newValue', 'type'];
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
}

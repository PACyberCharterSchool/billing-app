import { Pipe, PipeTransform } from '@angular/core';

import { AuditRecordType } from '../models/audit-record.model';

@Pipe({
  name: 'interpretAuditType'
})
export class InterpretAuditTypePipe implements PipeTransform {

  transform(value: any, args?: any): any {
    let v = 'Unknown';

    switch (value) {
      case AuditRecordType.InvoiceTemplates:
        v = 'Invoice Template';
        break;
      case AuditRecordType.SchoolCalendars:
        v = 'School Calendar';
        break;
      case AuditRecordType.SchoolDistricts:
        v = 'School District';
        break;
      case AuditRecordType.StudentRecords:
        v = 'Student Records';
        break;
    }
    return v;
  }

}

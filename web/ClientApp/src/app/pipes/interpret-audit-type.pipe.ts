import { Pipe, PipeTransform } from '@angular/core';

import { AuditRecordActivityType } from '../models/audit-record.model';

@Pipe({
  name: 'interpretAuditType'
})
export class InterpretAuditTypePipe implements PipeTransform {

  transform(value: any, args?: any): any {
    let v = 'Unknown';

    switch (value) {
      case AuditRecordActivityType.UpdateTemplate:
        v = 'Updated Invoice Template';
        break;
      case AuditRecordActivityType.UpdateSchoolCalendar:
        v = 'Updated School Calendar';
        break;
      case AuditRecordActivityType.SchoolDistricts:
        v = 'School District';
        break;
      case AuditRecordActivityType.EditStudentRecord:
        v = 'Edit Student Record';
        break;
    }
    return v;
  }

}

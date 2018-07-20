import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'normalizeFieldName'
})
export class NormalizeFieldNamePipe implements PipeTransform {

  transform(value: any, args?: any): any {
    let fieldName = '';

    switch (value) {
      case 'id':
        fieldName = 'Id';
        break;
      case 'activitySchoolYear':
        fieldName = 'School Year';
        break;
      case 'studentFirstName':
        fieldName = 'First Name';
        break;
      case 'studentLastName':
        fieldName = 'Last Name';
        break;
      case 'schoolDistrictId':
      case 'aun':
        fieldName = 'AUN #';
        break;
      case 'studentCity':
        fieldName = 'City';
        break;
      case 'studentStreet1':
        fieldName = 'Street 1';
        break;
      case 'studentStreet2':
        fieldName = 'Street 2';
        break;
      case 'externalId':
        fieldName = 'SD/PDE #';
        break;
      case 'checkNumber':
        fieldName = 'SD #';
        break;
      case 'studentState':
        fieldName = 'State';
        break;
      case 'schoolDistrictName':
      case 'schoolDistrict':
        fieldName = 'School District Name';
        break;
      case 'activity':
        fieldName = 'Activity';
        break;
      case 'previousData':
        fieldName = 'Old Field';
        break;
      case 'nextData':
        fieldName = 'New Field';
        break;
      case 'timestamp':
        fieldName = 'Date Modified';
        break;
      case 'type':
        fieldName = 'Type';
        break;
      case 'city':
        fieldName = 'City';
        break;
      case 'street1':
        fieldName = 'Street1';
        break;
      case 'street2':
        fieldName = 'Street2';
        break;
      case 'state':
        fieldName = 'State';
        break;
      case 'grade':
      case 'studentGradeLevel':
        fieldName = 'Grade';
        break;
      case 'zipCode':
      case 'studentZipCode':
        fieldName = 'Zip Code';
        break;
      case 'paCyberId':
        fieldName = 'PA Cyber Id';
        break;
      case 'paSecuredId':
      case 'studentPaSecuredId':
        fieldName = 'PA Secure Id';
        break;
      case 'firstName':
        fieldName = 'First Name';
        break;
      case 'lastName':
        fieldName = 'Last Name';
        break;
      case 'middleInitial':
      case 'studentMiddleInitial':
        fieldName = 'Middle Init.';
        break;
      case 'currentIep':
      case 'studentCurrentIep':
        fieldName = 'Current IEP Date';
        break;
      case 'formerIep':
      case 'studentFormerIep':
        fieldName = 'Former IEP Date';
        break;
      case 'startDate':
        fieldName = 'Start Date';
        break;
      case 'endDate':
        fieldName = 'End Date';
        break;
      case 'dateOfBirth':
      case 'studentDateOfBirth':
        fieldName = 'Date of Birth';
        break;
      case 'created':
        fieldName = 'Created At';
        break;
      case 'lastUpdated':
        fieldName = 'Updated At';
        break;
      case 'date':
        fieldName = 'Date';
        break;
      case 'oldValue':
        fieldName = 'Old Value';
        break;
      case 'newValue':
        fieldName = 'New Value';
        break;
      case 'paymentAmt':
      case 'amount':
        fieldName = 'Amount';
        break;
      case 'paymentDate':
        fieldName = 'Date';
        break;
      case 'academicYear':
        fieldName = 'Academic Year';
        break;
      case 'studentId':
        fieldName = 'Student Id';
        break;
      case 'paymentId':
        fieldName = 'Payment Id';
        break;
      case 'studentEnrollmentDate':
        fieldName = 'Enrollment Date';
        break;
      case 'studentWithdrawalDate':
        fieldName = 'Withdrawal Date';
        break;
      case 'studentIsSpecialEducation':
        fieldName = 'Special Ed.?';
        break;
      case 'studentNorep':
        fieldName = 'NOREP';
        break;
      case 'split':
        fieldName = 'Split Payment';
        break;
      case 'schoolYear':
        fieldName = 'Academic Year';
        break;
      case 'userName':
        fieldName = 'User Name';
        break;
      case 'username':
        fieldName = 'Created By';
        break;
      case 'name':
        fieldName = 'Name';
        break;
      case 'rate':
        fieldName = 'Rate';
        break;
      case 'alternateRate':
        fieldName = 'Alt Rate';
        break;
      case 'specialEducationRate':
        fieldName = 'SPED Rate';
        break;
      case 'alternateSpecialEducationRate':
        fieldName = 'SPED Alt Rate';
        break;
      case 'paymentType':
        fieldName = 'PMT Type';
        break;
      case 'reportType':
        fieldName = 'Report Type';
        break;
      case 'dayOfWeek':
        fieldName = 'Day of Week';
        break;
      case 'membership':
        fieldName = 'Membership';
        break;
      case 'schoolDay':
        fieldName = 'School Day';
        break;
      case 'fileName':
        fieldName = 'File Name';
        break;
      case 'title':
        fieldName = 'Title';
        break;
      case 'approved':
        fieldName = 'Approved';
        break;
      default:
        fieldName = 'Column';
        break;
    }

    return fieldName;
  }

}

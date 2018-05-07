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
      case 'studentGradeLevel':
        fieldName = 'Grade';
        break;
      case 'schoolDistrictId':
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
      case 'studentState':
        fieldName = 'State';
        break;
      case 'schoolDistrictName':
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
        fieldName = 'Grade';
        break;
      case 'zipCode':
        fieldName = 'Zip Code';
        break;
      case 'paCyberId':
        fieldName = 'PA Cyber Id';
        break;
      case 'paSecuredId':
        fieldName = 'PA Secure Id';
        break;
      case 'firstName':
        fieldName = 'First Name';
        break;
      case 'lastName':
        fieldName = 'Last Name';
        break;
      case 'middleInitial':
        fieldName = 'Middle Initial';
        break;
      case 'currentIep':
        fieldName = 'Current IEP Date';
        break;
      case 'formerIep':
        fieldName = 'Former IEP Date';
        break;
      case 'startDate':
        fieldName = 'Start Date';
        break;
      case 'endDate':
        fieldName = 'End Date';
        break;
      case 'dateOfBirth':
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
      default:
        fieldName = 'Column';
        break;
    }

    return fieldName;
  }

}

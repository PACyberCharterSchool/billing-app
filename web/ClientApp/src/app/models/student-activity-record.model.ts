import { StudentActivity } from './student-activity.model';

export class StudentActivityRecord {
  Id: number;
  PACyberId: string;
  Activity: StudentActivity;
  Sequence: number;
  PreviousData: string;
  NextData: string;
  Timestamp: Date;
  BatchHash: string;
}

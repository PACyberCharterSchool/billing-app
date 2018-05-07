import { StudentActivity } from './student-activity.model';

export class StudentActivityRecord {
  id: number;
  paCyberId: string;
  activity: StudentActivity;
  sequence: number;
  previousData: string;
  nextData: string;
  timestamp: Date;
  batchHash: string;
}

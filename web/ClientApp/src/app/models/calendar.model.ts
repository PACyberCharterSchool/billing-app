import { CalendarDay } from './calendar-day.model';

export class Calendar {
  id: number;
  schoolYear: string;
  created: Date;
  lastUpdated: Date;
  days: CalendarDay[];
}

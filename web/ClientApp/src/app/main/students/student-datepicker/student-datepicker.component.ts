import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-student-datepicker',
  templateUrl: './student-datepicker.component.html',
  styleUrls: ['./student-datepicker.component.scss']
})
export class StudentDatepickerComponent {
  @Input() iconSize: string;
  @Input() date: Date;

  @Output() dateSelected: EventEmitter<Date> = new EventEmitter();

  model: { year: number; month: number; day: number };
  validateModel(): boolean {
    return this.model.year >= 1000 && this.model.year <= 9999
      && this.model.month >= 1 && this.model.month <= 12
      && this.model.day >= 1 && this.model.day <= 31;
  }

  onDateChanged(): void {
    if (this.model === undefined || this.model === null || typeof this.model !== 'object' || !this.validateModel()) {
      return;
    }

    this.date = new Date(this.model.year, this.model.month - 1, this.model.day); // yes, that bit of math on the month value is necessary
    if (isNaN(this.date.valueOf())) {
      return;
    }

    this.dateSelected.emit(this.date);
  }
}

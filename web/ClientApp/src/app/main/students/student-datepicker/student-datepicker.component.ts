import { Component, Input, OnChanges, OnInit, Output, EventEmitter } from '@angular/core';

import { StudentsService } from '../../../services/students.service';

@Component({
  selector: 'app-student-datepicker',
  templateUrl: './student-datepicker.component.html',
  styleUrls: ['./student-datepicker.component.scss']
})
export class StudentDatepickerComponent implements OnInit {
  private studentsService: StudentsService;

  @Input() iconSize: string;
  @Input() date: Date;

  @Output() dateSelected: EventEmitter<Date> = new EventEmitter();

  model;

  constructor() { }

  ngOnInit() {
  }

  onDateChanged() {
    this.date = new Date(this.model.year, this.model.month - 1, this.model.day); // yes, that bit of math on the month value is necessary
    this.dateSelected.emit(this.date);
  }
}

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

  @Output() dateSelected: EventEmitter<any> = new EventEmitter();

  model;

  constructor() { }

  ngOnInit() {
    console.log('StudentDatepickerComponent.ngOnInit(): iconSize is ', this.iconSize);
  }

  onDateChanged() {
    this.date = this.model;
    this.dateSelected.emit();
    console.log('StudentDatepickerComponent.ngOnChange():  dateSelected event emitted.');
  }
}

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

  @Output() dateSelected: EventEmitter<any> = new EventEmitter();

  model;

  constructor() { }

  ngOnInit() {
    console.log('StudentDatepickerComponent.ngOnInit(): iconSize is ', this.iconSize);
  }

  ngOnChange() {
    this.dateSelected.emit();
  }
}

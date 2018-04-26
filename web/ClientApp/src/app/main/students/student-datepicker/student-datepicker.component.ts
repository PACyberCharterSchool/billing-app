import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-student-datepicker',
  templateUrl: './student-datepicker.component.html',
  styleUrls: ['./student-datepicker.component.scss']
})
export class StudentDatepickerComponent implements OnInit {

  @Input() iconSize: string;

  constructor() { }

  ngOnInit() {
    console.log('StudentDatepickerComponent.ngOnInit(): iconSize is ', this.iconSize);
  }

}

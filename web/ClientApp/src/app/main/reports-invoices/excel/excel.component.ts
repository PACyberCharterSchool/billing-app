import { Component, Input, OnInit } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'app-excel',
  templateUrl: './excel.component.html',
  styleUrls: ['./excel.component.scss']
})
export class ExcelComponent implements OnInit {
  private table;

  @Input() html;

  constructor(
    private sanitizer: DomSanitizer
  ) { }

  ngOnInit() {
    this.table = this.sanitizer.bypassSecurityTrustHtml(this.html);
  }

}

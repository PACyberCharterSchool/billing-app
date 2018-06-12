import { Component, OnInit } from '@angular/core';

import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-invoice-preview-form',
  templateUrl: './invoice-preview-form.component.html',
  styleUrls: ['./invoice-preview-form.component.scss']
})
export class InvoicePreviewFormComponent implements OnInit {

  constructor(
    private ngbActiveModal: NgbActiveModal
  ) { }

  ngOnInit() {
  }

}

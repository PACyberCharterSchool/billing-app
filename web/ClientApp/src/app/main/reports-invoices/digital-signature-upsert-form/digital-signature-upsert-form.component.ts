import { Component, Input, OnInit } from '@angular/core';

import { DigitalSignature } from '../../../models/digital-signature.model';

import { DigitalSignaturesService } from '../../../services/digital-signatures.service';

import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-digital-signature-upsert-form',
  templateUrl: './digital-signature-upsert-form.component.html',
  styleUrls: ['./digital-signature-upsert-form.component.scss']
})
export class DigitalSignatureUpsertFormComponent implements OnInit {

  private userName: string;
  private title: string;
  private fileName: string;
  private imageUrl: string;
  private selectedFiles;

  @Input() op: string;
  @Input() digitalSignature: DigitalSignature;

  constructor(
    private ngbActiveModal: NgbActiveModal,
    private digitalSignaturesService: DigitalSignaturesService
  ) { }

  ngOnInit() {
    if (this.op === 'update') {
      this.digitalSignaturesService.getDigitalSignature(this.digitalSignature.id).subscribe(
        data => {
          console.log('DigitalSignatureUpsertFormComponent.ngOnInit(): data is ', data['digitalSignatures']);
        },
        error => {
          console.log('DigitalSignatureUpsertFormComponent.ngOnInit(): error is ', error);
        }
      )
    }

    if (this.digitalSignature) {
      this.updateDigitalSignatureComponentValues();
    }
    else {
      this.digitalSignature = new DigitalSignature();
    }
  }

  updateDigitalSignatureComponentValues() {
    this.title = this.digitalSignature.title;
    this.userName = this.digitalSignature.userName;
    this.fileName = this.digitalSignature.fileName;
  }

  updateDigitalSignatureRecord() {
    this.digitalSignature.title = this.title;
    this.digitalSignature.userName = this.userName;
    this.digitalSignature.fileName = this.fileName;
  }

  upsertDigitalSignature() {
    this.updateDigitalSignatureRecord();

    if (this.op === 'create' && this.selectedFiles[0]) {
      const formData = new FormData();

      formData.append('file', this.selectedFiles[0], this.selectedFiles[0].name);
      formData.append('title', this.digitalSignature.title ? this.digitalSignature.title : '');
      formData.append('fileName', this.digitalSignature.fileName ? this.digitalSignature.fileName : '');
      formData.append('userName', this.digitalSignature.userName ? this.digitalSignature.userName : '');

      this.digitalSignaturesService.createDigitalSignature(formData).subscribe(
        data => {
          console.log('DigitalSignatureUpsertFormComponent.upsertDigitalSignature(): data is ', data);
          this.ngbActiveModal.close();
        },
        error => {
          console.log('DigitalSignatureUpsertFormComponent.upsertDigitalSignature(): error is ', error);
          this.ngbActiveModal.close();
        }
      );

    }
    else if (this.op === 'update') {
      console.log('DigitalSignatureUpsertFormComponent.upsertDigitalSignature():  not implemented.');
    }
  }

  setImageUrl($event) {
    if ($event.target.files && $event.target.files[0]) {
      this.selectedFiles = $event.target.files;
    }
  }
}

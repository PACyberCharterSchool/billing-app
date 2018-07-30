import { Component, Input, OnInit } from '@angular/core';

import { DigitalSignature } from '../../../models/digital-signature.model';

import { DigitalSignaturesService } from '../../../services/digital-signatures.service';

import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

import { Globals } from '../../../globals';

@Component({
  selector: 'app-digital-signature-upsert-form',
  templateUrl: './digital-signature-upsert-form.component.html',
  styleUrls: ['./digital-signature-upsert-form.component.scss']
})
export class DigitalSignatureUpsertFormComponent implements OnInit {

  private userName: string;
  public title: string;
  public fileName: string;
  public imageUrl: string;
  private selectedFiles;
  private imgData: string;
  public isImageAssigned: boolean;

  @Input() op: string;
  @Input() digitalSignature: DigitalSignature;

  constructor(
    public ngbActiveModal: NgbActiveModal,
    private digitalSignaturesService: DigitalSignaturesService,
    private globals: Globals
  ) { }

  ngOnInit() {
    if (this.op === 'update') {
      this.digitalSignaturesService.getDigitalSignature(this.digitalSignature.id).subscribe(
        data => {
          console.log('DigitalSignatureUpsertFormComponent.ngOnInit(): data is ', data['digitalSignature']);

          this.digitalSignature = data['digitalSignature'];
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
      this.imageUrl = this.globals.sprite2X;
      this.isImageAssigned = false;
    }
  }

  updateDigitalSignatureComponentValues() {
    this.title = this.digitalSignature.title;
    this.userName = this.digitalSignature.userName;
    this.fileName = '';
    if (this.digitalSignature.imgData) {
      this.imageUrl = 'data:image/png;base64,' + this.digitalSignature.imgData;
      this.isImageAssigned = true;
    }
    else {
      this.isImageAssigned = false;
    }
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

  setImageUrl($event: any) {
    if ($event.target.files && $event.target.files[0]) {
      var reader = new FileReader();

      reader.onload = (event: any) => {
        this.imageUrl = event.target.result;
      }

      reader.readAsDataURL($event.target.files[0]);

      this.selectedFiles = $event.target.files;
      this.isImageAssigned = (this.selectedFiles.length > 0);
    }
  }
}

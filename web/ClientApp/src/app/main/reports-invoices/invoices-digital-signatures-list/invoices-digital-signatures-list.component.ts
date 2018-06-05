import { Component, OnInit } from '@angular/core';

import { DigitalSignature } from '../../../models/digital-signature.model';

import { DigitalSignaturesService } from '../../../services/digital-signatures.service';
import { UtilitiesService } from '../../../services/utilities.service';

import { DigitalSignatureUpsertFormComponent } from '../digital-signature-upsert-form/digital-signature-upsert-form.component';

import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-invoices-digital-signatures-list',
  templateUrl: './invoices-digital-signatures-list.component.html',
  styleUrls: ['./invoices-digital-signatures-list.component.scss']
})
export class InvoicesDigitalSignaturesListComponent implements OnInit {
  private searchText: string;
  private direction: number;
  private property: string;
  private isDescending: boolean;
  private skip: number;
  private allSignatures: DigitalSignature[];
  private signatures: DigitalSignature[];

  constructor(
    private digitalSignaturesService: DigitalSignaturesService,
    private utilitiesService: UtilitiesService,
    private ngbModal: NgbModal
  ) {
    this.property = 'title';
    this.direction = 1;
    this.skip = 0;
    this.isDescending = false;
  }

  ngOnInit() {
    this.digitalSignaturesService.getDigitalSignatures(this.skip).subscribe(
      data => {
        console.log('InvoicesDigitalSignaturesListComponent.ngOnInit():  data is ', data['digitalSignatures']);
        this.signatures = this.allSignatures = data['digitalSignatures'];
      },
      error => {
        console.log('InvoicesDigitalSignaturesListComponent.ngOnInit():  error is ', error);
      }
    );
  }

  sort(property) {
    this.isDescending = !this.isDescending; // change the direction
    this.property = property;
    this.direction = this.isDescending ? 1 : -1;
  }

  listDisplayableFields() {
    if (this.allSignatures) {
      const fields = this.utilitiesService.objectKeys(this.allSignatures[0]);
      return fields;
    }
  }

  createDigitalSignature() {
    const modalRef = this.ngbModal.open(DigitalSignatureUpsertFormComponent);
    modalRef.componentInstance.op = 'create';
    modalRef.componentInstance.digitalSignature = null;

    modalRef.result.then(
      (result) => {
        console.log('DigitalSignaturesListComponent.createDigitalSignature(): result is ', result);
      },
      (reason) => {
        console.log('DigitalSignaturesListComponent.createDigitalSignature(): reason is ', reason);
      }
    )
  }

  updateDigitalSignature(ds: DigitalSignature) {
    const modalRef = this.ngbModal.open(DigitalSignatureUpsertFormComponent);
    modalRef.componentInstance.op = 'update';
    modalRef.componentInstance.digitalSignature = ds;

    modalRef.result.then(
      (result) => {
        console.log('DigitalSignaturesListComponent.createDigitalSignature(): result is ', result);
      },
      (reason) => {
        console.log('DigitalSignaturesListComponent.createDigitalSignature(): reason is ', reason);
      }
    )
  }

}

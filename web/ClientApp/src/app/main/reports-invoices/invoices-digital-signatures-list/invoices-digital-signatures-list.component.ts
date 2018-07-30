import { Component, OnInit } from '@angular/core';

import { DigitalSignature } from '../../../models/digital-signature.model';

import { DigitalSignaturesService } from '../../../services/digital-signatures.service';
import { UtilitiesService } from '../../../services/utilities.service';

import { DigitalSignatureUpsertFormComponent } from '../digital-signature-upsert-form/digital-signature-upsert-form.component';

import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

import { Globals } from '../../../globals';

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

  private readonly CloseDeleteDlgWithYes = 'Yes click';
  private readonly CloseDeleteDlgWithNo = 'No click';

  constructor(
    private digitalSignaturesService: DigitalSignaturesService,
    private utilitiesService: UtilitiesService,
    private globals: Globals,
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
      const rejected = ['imgData'];
      return fields.filter((i) => !rejected.includes(i));
    }
  }

  listDisplayableValues(digitalSignature: DigitalSignature) {
    const vkeys = this.listDisplayableFields();
    const selected = this.utilitiesService.pick(digitalSignature, vkeys);

    return this.utilitiesService.objectValues(selected);
  }

  filterDigitalSignatures() {
    this.signatures = this.allSignatures.filter(
      (i) => {
        const re = new RegExp(this.searchText, 'gi');
        if (
          i.title.search(re) !== -1 ||
          i.fileName.search(re) !== -1 ||
          i.userName.search(re) !== -1
        ) {
          return true;
        }
        return false;
      }
    );
  }

  resetDigitalSignatures() {
    this.signatures = this.allSignatures;
  }

  refreshDigitalSignatures() {
    this.digitalSignaturesService.getDigitalSignatures(this.skip).subscribe(
      data => {
        this.signatures = this.allSignatures = data['digitalSignatures'];
      },
      error => {
        console.log('InvoicesDigitalSignaturesListComponent.ngOnInit():  error is ', error);
      }
    );
  }

  createDigitalSignature() {
    const modalRef = this.ngbModal.open(DigitalSignatureUpsertFormComponent, { size: 'lg', centered: true });
    modalRef.componentInstance.op = 'create';
    modalRef.componentInstance.digitalSignature = null;

    modalRef.result.then(
      (result) => {
        console.log('DigitalSignaturesListComponent.createDigitalSignature(): result is ', result);
        this.refreshDigitalSignatures();
      },
      (reason) => {
        console.log('DigitalSignaturesListComponent.createDigitalSignature(): reason is ', reason);
      }
    )
  }

  editDigitalSignature(ds: DigitalSignature) {
    const modalRef = this.ngbModal.open(DigitalSignatureUpsertFormComponent, { size: 'lg', centered: true });
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

  deleteDigitalSignature(content, ds: DigitalSignature) {
    const modalRef = this.ngbModal.open(content).result.then(
      (result) => {
        if (result === this.CloseDeleteDlgWithYes) {
          this.digitalSignaturesService.deleteDigitalSignature(ds.id).subscribe(
            data => {
              console.log('DigitalSignaturesListComponent.deleteDigitalSignature(): data is ', data);
              this.refreshDigitalSignatures();
            },
            error => {
              console.log('DigitalSignaturesListComponent.deleteDigitalSignature(): error is ', error);
            }
          );
        }
        else if (result === this.CloseDeleteDlgWithNo) {
          console.log('DigitalSignaturesListComponent.deleteDigitalSignature(): result is ', result);
        }
      },
      (reason) => {
        console.log('DigitalSignaturesListComponent.deleteDigitalSignature(): reason is ', reason);
      }
    )
  }

  updateScrollingSkip() {
    this.skip += this.globals.take;
  }

}

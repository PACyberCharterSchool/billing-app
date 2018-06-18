import { Component, OnInit } from '@angular/core';

import { UtilitiesService } from '../../../services/utilities.service';
import { TemplatesService } from '../../../services/templates.service';

import { Globals } from '../../../globals';

import { Template } from '../../../models/template.model';

import { NgbModal, ModalDismissReasons } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-administration-template-list',
  templateUrl: './administration-template-list.component.html',
  styleUrls: ['./administration-template-list.component.scss']
})
export class AdministrationTemplateListComponent implements OnInit {
  private property: string;
  private isDescending: boolean;
  private direction: number;
  private searchText: string;
  private skip: number;
  private templates: Template[];
  private allTemplates: Template[];

  constructor(
    private globals: Globals,
    private utilitiesService: UtilitiesService,
    private templatesService: TemplatesService,
    private ngbModal: NgbModal
  ) {
    this.property = 'name';
    this.direction = 1;
    this.skip = 0;
  }

  ngOnInit() {
    this.templatesService.getTemplates(this.skip).subscribe(
      data => {
        console.log('AdministrationTemplateListComponent.ngOnInit():  data is ', data);
      },
      error => {
        console.log('AdministrationTemplateListComponent.ngOnInit():  error is ', error);
      }
    );
  }

  sort(property): void {
    this.isDescending = !this.isDescending; // change the direction
    this.property = property;
    this.direction = this.isDescending ? 1 : -1;
  }

  filterTemplateRecords(): void {
    this.templates = this.allTemplates.filter(
      (i) => {
        const re = new RegExp(this.searchText, 'gi');
        if (i &&
          i.name.search(re) !== -1
        ) {
          return true;
        }
        return false;
      }
    );
    console.log('AdministrationTemplateListComponent.filterTemplateRecords():  templates are ', this.templates);
  }

  resetTemplateRecords(): void {
    this.templates = this.allTemplates;
    this.searchText = '';
  }

  listDisplayableFields(): Object[] {
    if (this.templates) {
      const fields = this.utilitiesService.objectKeys(this.templates[0]);
      const rejected = ['id'];
      return fields.filter((i) => !rejected.includes(i));
    }
  }

  listDisplayableValues(t: Template): Object[] {
    if (t) {
      const vkeys = this.listDisplayableFields();
      const selected = this.utilitiesService.pick(t, vkeys);

      return this.utilitiesService.objectValues(selected);
    }
  }

  getSchoolYears(): string[] {
    if (this.allTemplates) {
      const years = this.allTemplates.filter((obj, pos, arr) => {
        return arr.map(mo => mo['schoolYear']).indexOf(obj['schoolYear']) === pos;
      });
      return years.map(y => y.schoolYear);
    }
  }

  importTemplate(importTemplateContent): void {
    this.ngbModal.open(importTemplateContent).result.then(
      (result) => {
      },
      (reason) => {
      }
    );
  }

  setExcelTemplateUrl($event): void {
    if ($event) {
      if ($event.target.files && $event.target.files.length > 0) {
        const files = $event.target.files;
        const formData = new FormData();

        formData.append('file', files[0], files[0].name);
        this.templatesService.putTemplatesByTypeAndByYear('Invoice', files[0].name).subscribe(
          data => {
          },
          error => {
          }
        );
      }
    }
  }
}

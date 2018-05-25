import { Component, OnInit } from '@angular/core';

import { UtilitiesService } from '../../../services/utilities.service';

import { Globals } from '../../../globals';

@Component({
  selector: 'app-administration-audit-list',
  templateUrl: './administration-audit-list.component.html',
  styleUrls: ['./administration-audit-list.component.scss']
})
export class AdministrationAuditListComponent implements OnInit {

  private auditTypes;
  private property: string;
  private direction: number;
  private isDescending: boolean;

  constructor(
    private globals: Globals,
    private utilitiesService: UtilitiesService
  ) { }

  ngOnInit() {
  }

}

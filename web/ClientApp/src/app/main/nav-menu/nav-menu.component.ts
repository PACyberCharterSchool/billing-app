import { Component, OnInit } from '@angular/core';
import { AppTitleService } from '../../services/app-title.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss']
})
export class NavMenuComponent implements OnInit {
  isExpanded = false;
  showAdministrationSubItems = false;
  appTitle: string;

  constructor(private appTitleService: AppTitleService) { }

  ngOnInit() {
    this.appTitleService.currentAppTitle.subscribe(appTitle => this.appTitle = appTitle);
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  toggleAdministrationSubItems() {
    this.showAdministrationSubItems = !this.showAdministrationSubItems;
  }
}

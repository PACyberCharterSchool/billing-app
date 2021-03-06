import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';

import { ContentAreaComponent } from './content-area.component';
import { NavMenuComponent } from '../nav-menu/nav-menu.component';
// import { ActionContentComponent } from './action-content/action-content.component';

import { AppTitleService } from '../../services/app-title.service';

describe('ContentAreaComponent', () => {
  let component: ContentAreaComponent;
  let fixture: ComponentFixture<ContentAreaComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [
        NavMenuComponent,
        // ActionContentComponent,
        ContentAreaComponent
      ],
      imports: [
        RouterTestingModule
      ],
      providers: [
        AppTitleService
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ContentAreaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

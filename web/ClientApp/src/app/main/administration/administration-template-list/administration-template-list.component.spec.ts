import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AdministrationTemplateListComponent } from './administration-template-list.component';

describe('AdministrationTemplateListComponent', () => {
  let component: AdministrationTemplateListComponent;
  let fixture: ComponentFixture<AdministrationTemplateListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AdministrationTemplateListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AdministrationTemplateListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

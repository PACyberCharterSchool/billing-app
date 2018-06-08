import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AdministrationAuditListComponent } from './administration-audit-list.component';

describe('AdministrationAuditListComponent', () => {
  let component: AdministrationAuditListComponent;
  let fixture: ComponentFixture<AdministrationAuditListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AdministrationAuditListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AdministrationAuditListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

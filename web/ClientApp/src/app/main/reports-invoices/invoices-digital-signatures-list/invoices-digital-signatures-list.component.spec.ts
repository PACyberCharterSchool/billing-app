import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InvoicesDigitalSignaturesListComponent } from './invoices-digital-signatures-list.component';

describe('InvoicesDigitalSignaturesListComponent', () => {
  let component: InvoicesDigitalSignaturesListComponent;
  let fixture: ComponentFixture<InvoicesDigitalSignaturesListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InvoicesDigitalSignaturesListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InvoicesDigitalSignaturesListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

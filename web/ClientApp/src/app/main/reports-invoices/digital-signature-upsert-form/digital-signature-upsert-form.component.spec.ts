import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DigitalSignatureUpsertFormComponent } from './digital-signature-upsert-form.component';

describe('DigitalSignatureUpsertFormComponent', () => {
  let component: DigitalSignatureUpsertFormComponent;
  let fixture: ComponentFixture<DigitalSignatureUpsertFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DigitalSignatureUpsertFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DigitalSignatureUpsertFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

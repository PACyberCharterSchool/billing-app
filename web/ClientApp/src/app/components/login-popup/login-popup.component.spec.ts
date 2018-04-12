import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MatMenuModule, MatIconModule } from '@angular/material';

import { LoginPopupComponent } from '../login-popup/login-popup.component';


describe(LoginPopupComponent.name, () => {
  let component: LoginPopupComponent;
  let fixture: ComponentFixture<LoginPopupComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [
        LoginPopupComponent
      ],
      imports: [
        MatIconModule,
        MatMenuModule
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LoginPopupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

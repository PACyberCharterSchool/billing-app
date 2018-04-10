import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MatMenuModule, MatIconModule } from '@angular/material';

import { LoginTitleBarComponent } from './login-title-bar.component';
import { LoginPopupComponent } from '../login-popup/login-popup.component';

describe(LoginTitleBarComponent.name, () => {
  let component: LoginTitleBarComponent;
  let fixture: ComponentFixture<LoginTitleBarComponent>;
  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [
        LoginTitleBarComponent,
        LoginPopupComponent
      ],
      imports: [
        MatMenuModule,
        MatIconModule
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LoginTitleBarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

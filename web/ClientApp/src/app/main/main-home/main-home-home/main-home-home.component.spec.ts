import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MainHomeHomeComponent } from './main-home-home.component';

describe('MainHomeHomeComponent', () => {
  let component: MainHomeHomeComponent;
  let fixture: ComponentFixture<MainHomeHomeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MainHomeHomeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MainHomeHomeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

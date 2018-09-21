import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CsiuListComponent } from './csiu-list.component';

describe('CsiuListComponent', () => {
  let component: CsiuListComponent;
  let fixture: ComponentFixture<CsiuListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CsiuListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CsiuListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

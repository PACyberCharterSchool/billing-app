import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ContentAreaHomeComponent } from './content-area-home.component';

describe('ContentAreaHomeComponent', () => {
  let component: ContentAreaHomeComponent;
  let fixture: ComponentFixture<ContentAreaHomeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ContentAreaHomeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ContentAreaHomeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

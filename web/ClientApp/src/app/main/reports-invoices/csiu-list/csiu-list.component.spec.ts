import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';

import { CsiuListComponent } from './csiu-list.component';

import { NormalizeFieldNamePipe } from '../../../pipes/normalize-field-name.pipe';
import { NormalizeFieldValuePipe } from '../../../pipes/normalize-field-value.pipe';
import { OrderByPipe } from '../../../pipes/orderby.pipe';

xdescribe('CsiuListComponent', () => {
  let component: CsiuListComponent;
  let fixture: ComponentFixture<CsiuListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [
        CsiuListComponent,
        NormalizeFieldNamePipe,
        NormalizeFieldValuePipe,
        OrderByPipe
      ],
      imports: [ FormsModule ]
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

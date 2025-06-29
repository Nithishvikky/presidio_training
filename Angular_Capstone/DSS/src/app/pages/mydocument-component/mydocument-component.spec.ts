import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MydocumentComponent } from './mydocument-component';

describe('MydocumentComponent', () => {
  let component: MydocumentComponent;
  let fixture: ComponentFixture<MydocumentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MydocumentComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MydocumentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

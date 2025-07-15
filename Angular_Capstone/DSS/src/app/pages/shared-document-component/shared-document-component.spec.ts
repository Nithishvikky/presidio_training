import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SharedDocumentComponent } from './shared-document-component';

describe('SharedDocumentComponent', () => {
  let component: SharedDocumentComponent;
  let fixture: ComponentFixture<SharedDocumentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SharedDocumentComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SharedDocumentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

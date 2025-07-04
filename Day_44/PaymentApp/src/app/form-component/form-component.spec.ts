import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReactiveFormsModule } from '@angular/forms';
import { By } from '@angular/platform-browser';
import { FormComponent } from './form-component';

describe('FormComponent', () => {
  let component: FormComponent;
  let fixture: ComponentFixture<FormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({

      imports: [ReactiveFormsModule,FormComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(FormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });


  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should have invalid form when empty', () => {
    expect(component.paymentForm.invalid).toBeTrue();
  });

  it('should require all fields', () => {
    const form = component.paymentForm;
    form.setValue({
      amount: null,
      customerName: '',
      email: '',
      contactNumber: ''
    });

    expect(form.get('amount')?.hasError('required')).toBeTrue();
    expect(form.get('customerName')?.hasError('required')).toBeTrue();
    expect(form.get('email')?.hasError('required')).toBeTrue();
    expect(form.get('contactNumber')?.hasError('required')).toBeTrue();
  });

  it('should accept valid form input', () => {
    component.paymentForm.setValue({
      amount: 200,
      customerName: 'John Doe',
      email: 'john@example.com',
      contactNumber: '9876543210'
    });

    expect(component.paymentForm.valid).toBeTrue();
  });

  it('should call Razorpay open when form is valid and submitted', () => {
    // Arrange
    const mockOpen = jasmine.createSpy('open');
    const mockOn = jasmine.createSpy('on');

    const razorpaySpy = jasmine.createSpy('Razorpay').and.returnValue({
      open: mockOpen,
      on: mockOn
    });

    // @ts-ignore
    window.Razorpay = razorpaySpy;

    component.paymentForm.setValue({
      amount: 500,
      customerName: 'Test User',
      email: 'test@example.com',
      contactNumber: '9999999999'
    });

    // Act
    component.handleSubmit();

    // Assert
    expect(razorpaySpy).toHaveBeenCalled();
    expect(mockOpen).toHaveBeenCalled();
    expect(mockOn).toHaveBeenCalled();
  });
});

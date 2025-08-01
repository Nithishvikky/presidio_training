import { Component, ViewChild } from '@angular/core';
import { ContactUsRequest } from '../../Models/contactus.model';
import { ContactUsService } from '../../Services/ContactUs.service';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RecaptchaModule,RecaptchaComponent } from 'ng-recaptcha';

@Component({
  selector: 'app-contact-us-component',
  imports: [CommonModule,FormsModule,ReactiveFormsModule,RecaptchaModule],
  templateUrl: './contact-us-component.html',
  styleUrl: './contact-us-component.css'
})
export class ContactUsComponent {
  contactForm: FormGroup;
  captchaToken: string | null = null;
  message = '';

  @ViewChild(RecaptchaComponent) recaptchaComponent!: RecaptchaComponent;

  constructor(private fb: FormBuilder, private contactService: ContactUsService) {
    this.contactForm = this.fb.group({
      customerName: ['', Validators.required],
      customerEmail: ['', [Validators.required, Validators.email]],
      customerPhone: ['', Validators.required],
      customerContent: ['', Validators.required]
    });
  }

  onCaptchaResolved(token: string | null):void {
    if (token) {
      this.captchaToken = token;
    } else {
      this.captchaToken = null;
      console.warn('Captcha not resolved properly');
    }
  }

  onSubmit() {
    if (this.contactForm.invalid || !this.captchaToken) {
      this.message = 'Please fill all fields and complete the CAPTCHA.';
      return;
    }

    const formValue = this.contactForm.value;

    const request: ContactUsRequest = {
      ...formValue,
      captchaToken: this.captchaToken
    };

    this.contactService.submitContactForm(request).subscribe({
      next: (res) => {
        this.message = res.message;
        this.contactForm.reset();
        this.captchaToken = null;

        this.recaptchaComponent.reset();
      },
      error: (err) => {
        this.message = err.error?.message || 'Submission failed. Try again.';
      }
    });
  }
}

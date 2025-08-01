import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ContactUsRequest } from '../Models/contactus.model';

@Injectable()
export class ContactUsService {

  private baseUrl = 'http://localhost:5190/api/v1/ContactUs';

  constructor(private http: HttpClient) { }

  submitContactForm(data: ContactUsRequest) {
    return this.http.post<{ message: string, result: any }>(this.baseUrl, data);
  }
}

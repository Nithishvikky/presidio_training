import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { FormComponent } from "./form-component/form-component";

@Component({
  selector: 'app-root',
  imports: [FormComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'PaymentApp';
}

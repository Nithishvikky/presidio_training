import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-blank-component',
  imports: [],
  templateUrl: './blank-component.html',
  styleUrl: './blank-component.css'
})
export class BlankComponent {
  constructor(private router: Router) {}

  ngOnInit(): void {
    const authData = localStorage.getItem('authData');
    if (authData) {
      this.router.navigate(['/main/home']);
    } else {
      this.router.navigate(['/auth/signin']);
    }
  }
}

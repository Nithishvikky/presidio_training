import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-home',
  imports: [],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home implements OnInit {
   uname:string="";
   router = inject(ActivatedRoute)

  ngOnInit(): void {
    console.log("init");
    this.uname = this.router.snapshot.params["un"] as string
  }
}

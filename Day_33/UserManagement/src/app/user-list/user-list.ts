import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { UserModel } from '../models/user';
import { UserService } from '../services/user.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { debounceTime, distinctUntilChanged, fromEvent, map } from 'rxjs';

@Component({
  selector: 'app-user-list',
  imports: [FormsModule,CommonModule],
  templateUrl: './user-list.html',
  styleUrl: './user-list.css'
})
export class UserList implements OnInit{
  users:UserModel[] = [];
  selectedRole:string = "";
  selectedText:string = "";
  displayUsers:UserModel[] = [];

  @ViewChild('searchBox') searchInput!: ElementRef;

  constructor(private userService:UserService){}

  ngOnInit():void{
    this.userService.users$.subscribe({
      next:(data)=>{
        this.users = data;
        this.displayUsers = data;
        this.updateFilter();
      }
    })
  }

  ngAfterViewInit():void{
    fromEvent(this.searchInput.nativeElement, 'input')
      .pipe(
        map((event: any)=> event.target.value),
        debounceTime(300),
        distinctUntilChanged()
      )
      .subscribe((search: string)=>{
        this.selectedText = search;
        this.updateFilter();
      });
  }

  updateFilter(){
    this.displayUsers = this.users.filter(user =>{
      const matchesSearch = this.selectedText ? (`${user.username}`.toLowerCase().includes(this.selectedText.toLowerCase())) : true;
      const matchesRole = this.selectedRole ? user.role === this.selectedRole : true;

      return matchesRole && matchesSearch;
    });
  }
}

import { Component } from '@angular/core';
import { UserService } from '../../services/user.service';
import { UserResponseDto } from '../../models/userResponseDto';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { debounceTime } from 'rxjs';

@Component({
  selector: 'app-people-component',
  imports: [CommonModule,FormsModule,ReactiveFormsModule],
  templateUrl: './people-component.html',
  styleUrl: './people-component.css'
})
export class PeopleComponent {
  users: UserResponseDto[] | null = null;
  filterForm!:FormGroup;
  ascending:boolean = true;
  pageNumber:number = 1;
  totalPages:number = 0;
  sortBy:string = "";

  constructor(private fb:FormBuilder,private userService:UserService){}

  ngOnInit():void{
    this.filterForm = this.fb.group({
      userEmail:[''],
      userName: [''],
      filterBy:[''],
      sortBy: [''],
      ascending:[true]
    })

    this.userService.users$.subscribe((user:any) =>{
      if(user){
        this.totalPages = user.totalPages;
        this.users = user.items.$values;
      }
    })

    const values = this.filterForm.value;
    this.userService
      .GetAllUsers(values.userEmail,values.userName,values.filterBy,values.sortBy,values.ascending,this.pageNumber,6)
      .subscribe();

    this.filterForm.valueChanges
      .pipe(debounceTime(300))
      .subscribe(values => {
        this.pageNumber = 1;
        this.userService
          .GetAllUsers(values.userEmail,values.userName,values.filterBy,values.sortBy,values.ascending,this.pageNumber,6)
          .subscribe();
      });
  }

  fetchUsers(){
    const values = this.filterForm.value;
    this.userService
        .GetAllUsers(values.userEmail,values.userName,values.filterBy,values.sortBy,values.ascending,this.pageNumber,6)
        .subscribe();
  }

  nextPage(){
    this.pageNumber++;
    this.fetchUsers();
  }

  prevPage(){
    if(this.pageNumber > 1){
      this.pageNumber--;
      this.fetchUsers();
    }
  }

  clearFilters() {
    this.filterForm.setValue({
      userEmail: '',
      userName: '',
      filterBy:'',
      sortBy: '',
      ascending: true
    });
    this.ascending = true;
    this.sortBy = '';
    this.pageNumber = 1;
  }

  onSortby(event:Event){
    const value = (event.currentTarget as HTMLElement).id;


    this.filterForm.get('sortBy')?.setValue(value);
    this.sortBy = value;

    this.ascending = !this.ascending;
    this.filterForm.get('ascending')?.setValue(this.ascending);

  }

}


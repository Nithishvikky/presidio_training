import { Component, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { debounceTime } from 'rxjs';
import { UserService } from '../../services/user.service';
import { UserResponseDto } from '../../models/userResponseDto';
import { FormsModule } from '@angular/forms';
import { UserModalComponent } from '../user-modal-component/user-modal-component';
// import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-people-component',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule,UserModalComponent],
  templateUrl: './people-component.html',
  styleUrl: './people-component.css'
})
export class PeopleComponent {
  users: UserResponseDto[] = [];
  filterForm!: FormGroup;
  ascending = true;
  pageNumber = 1;
  totalPages = 0;
  sortBy = '';
  loading = false;
  showScrollTop = false;
  showSidebar = false;
  selectedUser!: UserResponseDto;
  showUserModal: boolean = false;

  constructor(private fb: FormBuilder, private userService: UserService) {}

  ngOnInit(): void {
    this.filterForm = this.fb.group({
      userEmail: [''],
      userName: [''],
      filterBy: [''],
      sortBy: [''],
      ascending: ['true']
    });

    this.fetchUsers();

    this.filterForm.valueChanges.pipe(debounceTime(300)).subscribe(() => {
      this.pageNumber = 1;
      this.users = [];
      this.fetchUsers();
    });
  }

  fetchUsers(): void {
    if (this.loading) return;
    const values = this.filterForm.value;
    this.loading = true;
    this.userService
      .GetAllUsers(
        values.userEmail,
        values.userName,
        values.filterBy,
        values.sortBy,
        values.ascending === 'true',
        this.pageNumber,
        9
      )
      .subscribe((res: any) => {
        if (res) {
          console.log(res);
          this.totalPages = res.data.totalPages;
          this.users = [...this.users, ...(res.data.items?.$values || [])];
        }
        this.loading = false;
      });
  }

  @HostListener('window:scroll', [])
  onWindowScroll() {
    const threshold = 10;
    this.showScrollTop = window.scrollY > threshold;

    if (window.innerHeight + window.scrollY >= document.body.offsetHeight - 100 && !this.loading && this.pageNumber < this.totalPages) {
      this.pageNumber++;
      this.fetchUsers();
    }
  }

  scrollToTop(): void {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  toggleSidebar(): void {
    this.showSidebar = !this.showSidebar;
  }

  clearControl(controlName: string): void {
    this.filterForm.get(controlName)?.setValue('');
  }

  clearFilters(){
    this.filterForm.patchValue({
      userEmail: '',
      userName: '',
      filterBy: '',
      sortBy: '',
      ascending: 'true'
    });
  }


  openUserDetails(email:string): void {
    console.log("clicked");
    this.userService.GetUserDetails(email).subscribe((res:any)=>{
      if(res){
        console.log(res);
        this.selectedUser = new UserResponseDto(res.id,res.email,res.username,res.role,res.registeredAt,res.updatedAt,res.uploadedDocuments.$values.length);
        console.log(this.selectedUser);
      }
    })
    this.showUserModal = true;
  }

}

// import { Component } from '@angular/core';
// import { UserService } from '../../services/user.service';
// import { UserResponseDto } from '../../models/userResponseDto';
// import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
// import { CommonModule } from '@angular/common';
// import { debounceTime } from 'rxjs';

// @Component({
//   selector: 'app-people-component',
//   imports: [CommonModule,FormsModule,ReactiveFormsModule],
//   templateUrl: './people-component.html',
//   styleUrl: './people-component.css'
// })
// export class PeopleComponent {
//   users: UserResponseDto[] | null = null;
//   filterForm!:FormGroup;
//   ascending:boolean = true;
//   pageNumber:number = 1;
//   totalPages:number = 0;
//   sortBy:string = "";

//   constructor(private fb:FormBuilder,private userService:UserService){}

//   ngOnInit():void{
//     this.filterForm = this.fb.group({
//       userEmail:[''],
//       userName: [''],
//       filterBy:[''],
//       sortBy: [''],
//       ascending:[true]
//     })

//     this.userService.users$.subscribe((user:any) =>{
//       if(user){
//         this.totalPages = user.totalPages;
//         this.users = user.items.$values;
//       }
//     })

//     const values = this.filterForm.value;
//     this.userService
//       .GetAllUsers(values.userEmail,values.userName,values.filterBy,values.sortBy,values.ascending,this.pageNumber,6)
//       .subscribe();

//     this.filterForm.valueChanges
//       .pipe(debounceTime(300))
//       .subscribe(values => {
//         this.pageNumber = 1;
//         this.userService
//           .GetAllUsers(values.userEmail,values.userName,values.filterBy,values.sortBy,values.ascending,this.pageNumber,6)
//           .subscribe();
//       });
//   }

//   fetchUsers(){
//     const values = this.filterForm.value;
//     this.userService
//         .GetAllUsers(values.userEmail,values.userName,values.filterBy,values.sortBy,values.ascending,this.pageNumber,6)
//         .subscribe();
//   }

//   nextPage(){
//     this.pageNumber++;
//     this.fetchUsers();
//   }

//   prevPage(){
//     if(this.pageNumber > 1){
//       this.pageNumber--;
//       this.fetchUsers();
//     }
//   }

//   clearFilters() {
//     this.filterForm.setValue({
//       userEmail: '',
//       userName: '',
//       filterBy:'',
//       sortBy: '',
//       ascending: true
//     });
//     this.ascending = true;
//     this.sortBy = '';
//     this.pageNumber = 1;
//   }

//   onSortby(event:Event){
//     const value = (event.currentTarget as HTMLElement).id;


//     this.filterForm.get('sortBy')?.setValue(value);
//     this.sortBy = value;

//     this.ascending = !this.ascending;
//     this.filterForm.get('ascending')?.setValue(this.ascending);

//   }

// }

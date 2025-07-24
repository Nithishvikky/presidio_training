import { Component, HostListener, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { debounceTime } from 'rxjs';
import { UserService } from '../../services/user.service';
import { UserResponseDto } from '../../models/userResponseDto';
import { FormsModule } from '@angular/forms';
import { UserModalComponent } from '../user-modal-component/user-modal-component';
import { NotificationService } from '../../services/notification.service';
import { DocumentService } from '../../services/document.service';
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
  inactiveUsers: UserResponseDto[] = [];
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
  showInactiveUsers: boolean = false;
  inactiveUserIds: string[] = [];

  constructor(
    private fb: FormBuilder, 
    private userService: UserService,
    private notificationService: NotificationService,
    private documentService: DocumentService,
    private cdr: ChangeDetectorRef
  ) {}

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

  getInactiveUsers(): void {
    this.loading = true;
    console.log('Getting inactive users...');
    this.userService.GetInactiveUsers(30).subscribe((res: any) => {
      console.log('Response received:', res);
      if (res && res.data && res.data.$values) {
        console.log('Processing inactive users data...');
        // Map the response to UserResponseDto objects
        this.inactiveUsers = res.data.$values.map((user: any) => 
          new UserResponseDto(
            user.id,
            user.email,
            user.username,
            user.role,
            user.registeredAt,
            user.updatedAt,
            user.uploadedDocuments ? user.uploadedDocuments.$values?.length || 0 : 0
          )
        );
        this.inactiveUserIds = this.inactiveUsers.map(user => user.userId);
        this.showInactiveUsers = true;
        console.log('Inactive users mapped:', this.inactiveUsers);
        console.log('Inactive user IDs:', this.inactiveUserIds);
        console.log('showInactiveUsers set to:', this.showInactiveUsers);
        this.cdr.detectChanges(); // Force change detection
      } else {
        console.log('No inactive users data found in response');
      }
      this.loading = false;
    });
  }

  notifyInactiveUsers(): void {
    this.loading = true;
    this.notificationService.NotifyInactiveUsers().subscribe((res: any) => {
      if (res) {
        console.log('Notifications sent to inactive users:', res);
        alert('Notifications sent to inactive users successfully!');
      }
      this.loading = false;
    });
  }

  archiveInactiveUserFiles(): void {
    if (this.inactiveUserIds.length === 0) {
      alert('No inactive users to archive files for.');
      return;
    }

    if (confirm(`Are you sure you want to archive files for ${this.inactiveUserIds.length} inactive users?`)) {
      this.loading = true;
      this.documentService.ArchiveUserFiles(this.inactiveUserIds).subscribe((res: any) => {
        if (res) {
          console.log('Files archived for inactive users:', res);
          alert('Files archived for inactive users successfully!');
        }
        this.loading = false;
      });
    }
  }

  backToAllUsers(): void {
    this.showInactiveUsers = false;
    this.inactiveUsers = [];
    this.inactiveUserIds = [];
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

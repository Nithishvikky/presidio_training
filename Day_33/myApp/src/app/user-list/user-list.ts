import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { User } from '../models/User';
import { Store } from '@ngrx/store';
import { AsyncPipe, NgFor, NgIf } from '@angular/common';
import { selectAllUsers,selectUserError, selectUserLoading } from '../ngrx/user.selector';
import { AddUser } from '../add-user/add-user';

@Component({
  selector: 'app-user-list',
  imports: [NgIf, NgFor, AsyncPipe, AddUser],
  templateUrl: './user-list.html',
  styleUrl: './user-list.css'
})
export class UserList {
users$:Observable<User[]> ;
loading$:Observable<boolean>;
error$:Observable<string | null>;

  constructor(private store:Store){
    this.users$ = this.store.select(selectAllUsers);
    this.loading$ = this.store.select(selectUserLoading);
    this.error$ = this.store.select(selectUserError);

  }
  ngOnInit(): void {
    this.store.dispatch({ type: '[Users] Load Users' });
  }
}

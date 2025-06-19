import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { UserList } from "./user-list/user-list";
import { MenuComponent } from "./menu-component/menu-component";

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, UserList, MenuComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'UserManagement';
}

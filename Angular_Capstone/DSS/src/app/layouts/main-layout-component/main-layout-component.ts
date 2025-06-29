import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { MenuComponent } from "../../pages/menu-component/menu-component";

@Component({
  selector: 'app-main-layout-component',
  imports: [RouterModule, MenuComponent],
  templateUrl: './main-layout-component.html',
  styleUrl: './main-layout-component.css'
})
export class MainLayoutComponent {

}

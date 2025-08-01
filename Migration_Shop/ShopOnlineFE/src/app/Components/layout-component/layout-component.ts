import { Component } from '@angular/core';
import { RouterLink, RouterModule, RouterOutlet } from '@angular/router';
import { CategorySidebar } from '../category-sidebar/category-sidebar';
import { ColorComponent } from "../color-component/color-component";

@Component({
  selector: 'app-layout-component',
  imports: [RouterOutlet, CategorySidebar,RouterLink],
  templateUrl: './layout-component.html',
  styleUrl: './layout-component.css'
})
export class LayoutComponent {
  currentYear = new Date().getFullYear();
}

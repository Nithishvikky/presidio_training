import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { LayoutComponent } from "./Components/layout-component/layout-component";
import { ColorComponent } from "./Components/color-component/color-component";

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, LayoutComponent, ColorComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'ShopOnlineFE';
}

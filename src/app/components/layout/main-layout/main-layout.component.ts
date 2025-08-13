import { Component } from '@angular/core';
import { HeaderComponent } from "./header/header.component";
import { NavbarComponent } from './navbar/navbar.component';

@Component({
  selector: 'app-main-layout',
  imports: [HeaderComponent, NavbarComponent],
  templateUrl: './main-layout.component.html',
  styleUrl: './main-layout.component.css'
})
export class MainLayoutComponent {

}

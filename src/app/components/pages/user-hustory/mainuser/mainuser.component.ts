import { Component } from '@angular/core';

import { MainLayoutComponent } from '../../../layout/main-layout/main-layout.component';
import { Footer } from '../../../layout/footer/footer';
import { SliderComponent } from '../slider/slider.component';

@Component({
  selector: 'app-mainuser',
  imports: [MainLayoutComponent, Footer, SliderComponent],
  templateUrl: './mainuser.component.html',
  styleUrl: './mainuser.component.css',
})
export class MainuserComponent {}

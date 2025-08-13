import { Component } from '@angular/core';
import { MainLayoutComponent } from '../../layout/main-layout/main-layout.component';
import { FoodHomeComponent } from "../homeComponents/food-home/food-home.component";

import { PartnerHomeComponent } from "../homeComponents/partner-home/partner-home.component";



import { CountHomeComponent } from "../homeComponents/count-home/count-home.component";

import { OrderHomeComponent } from "../homeComponents/order-home/order-home.component";
import { Order2HomeComponent } from "../homeComponents/order2-home/order2-home.component";
import { FooterComponent } from '../../layout/footer/footer.component';

@Component({
  selector: 'app-home',
  imports: [MainLayoutComponent, FoodHomeComponent,  PartnerHomeComponent, FooterComponent,  CountHomeComponent,  OrderHomeComponent, Order2HomeComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {

}

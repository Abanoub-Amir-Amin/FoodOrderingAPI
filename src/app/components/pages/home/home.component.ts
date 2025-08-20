import { Component } from '@angular/core';
import { MainLayoutComponent } from '../../layout/main-layout/main-layout.component';
import { FoodHomeComponent } from '../homeComponents/food-home/food-home.component';

import { PartnerHomeComponent } from '../homeComponents/partner-home/partner-home.component';

import { CountHomeComponent } from '../homeComponents/count-home/count-home.component';

import { OrderHomeComponent } from '../homeComponents/order-home/order-home.component';
import { Order2HomeComponent } from '../homeComponents/order2-home/order2-home.component';
import { Footer } from '../../layout/footer/footer';
import { ChatBot } from '../../chat-bot/chat-bot';
import { AboutComponent } from '../homeComponents/about/about.component';

@Component({
  selector: 'app-home',
  imports: [
    MainLayoutComponent,
    FoodHomeComponent,
    PartnerHomeComponent,
    Footer,
    CountHomeComponent,
    OrderHomeComponent,
    Order2HomeComponent,
    ChatBot,
    AboutComponent,
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent {}

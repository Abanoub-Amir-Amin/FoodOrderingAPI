import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { LoginComponent } from './components/login/login';
import { RestaurantApply } from './components/Restaurant/restaurant-apply/restaurant-apply';
//import { MainLayoutComponent } from './components/Restaurant/MainLayout/MainLayout'; 
import { ActionPendingComponent } from './components/Restaurant/action-pending/action-pending';
import { RestaurantDashboard } from './components/Restaurant/dashboard/dashboard';
import { RestaurantProfileComponent } from './components/Restaurant/restaurant-profile/restaurant-profile';
import { DashboardAnalytics } from './components/Restaurant/dashboard-analytics/dashboard-analytics';
import { MenuManagement } from './components/Restaurant/menu-management/menu-management';
import { OrdersManagement } from './components/Restaurant/orders-management/orders-management';
import { AuthGuard } from './services/auth-guard';
import { DeliveryManRegister } from './components/pages/auth/delivery-man-register/delivery-man-register';
import { RegisterComponent } from './components/pages/auth/register/register.component';
import { AddressComponent } from './components/pages/user-hustory/address/address.component';
import { PaymentComponent } from './components/pages/user-hustory/payment/payment.component';
import { MainuserComponent } from './components/pages/user-hustory/mainuser/mainuser.component';


import { OrderHistoryComponent } from './components/pages/order-history/order-history.component';
import { BillingPageComponent } from './components/pages/billing-page/billing-page.component';
import { HomeComponent } from './components/pages/home/home.component';
import { Getallresturant } from './components/pages/getallresturant/getallresturant';


export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'login', component: LoginComponent },
  { path: 'action-pending', component: ActionPendingComponent },
  { path: 'restaurant-apply', component: RestaurantApply },
  // {
  //   path: '',
  //   component: MainLayoutComponent,
  //   canActivate: [AuthGuard],
  //   children: [
  //     { path: 'dashboard', component: RestaurantDashboard },
  //     { path: 'orders-management', component: OrdersManagement },
  //     { path: 'menu-management', component: MenuManagement },
  //     { path: 'dashboard-analytics', component: DashboardAnalytics },
  //     // { path: 'settings', component: SettingsComponent },
  //     { path: 'restaurant-profile', component: RestaurantProfileComponent },
  //     { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  //   ]
  // },

  {path:"getAllResturant",component:Getallresturant},
  { path: 'dashboard', component: RestaurantDashboard, canActivate: [AuthGuard] },
  { path: 'orders-management', component: OrdersManagement, canActivate: [AuthGuard] },
  { path: 'menu-management', component: MenuManagement, canActivate: [AuthGuard] },
  { path: 'dashboard-analytics', component: DashboardAnalytics, canActivate: [AuthGuard] },
  { path: 'restaurant-profile', component: RestaurantProfileComponent, canActivate: [AuthGuard] },
  // { path: 'settings', component: SettingsComponent },
  
{
    path: 'home',
    loadComponent: () =>
      import('./components/pages/home/home.component').then(
        (m) => m.HomeComponent
      ),
  },
{path:'home' ,component:HomeComponent},
  { path: '', redirectTo: '/home', pathMatch: 'full' },
 {path:"getAllResturant",component:Getallresturant},
  { path: 'billing', component: BillingPageComponent },
  { path: 'order', component: OrderHistoryComponent },
 

  {
    path: 'user',
    component: MainuserComponent,
    children: [
      { path: 'payment', component: PaymentComponent },
      { path: 'ddress', component: AddressComponent },
    ],
  },

  { path: 'login', component: LoginComponent },
  { path: 'customerRegister', component: RegisterComponent },

  { path: 'restaurant-apply', component: RestaurantApply },
  { path: 'action-pending', component: ActionPendingComponent },
 
  
  { path: 'DeliveryManRegister', component: DeliveryManRegister },


  { path: '**', redirectTo: '' },
];



@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}

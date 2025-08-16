import { inject, NgModule } from '@angular/core';
import { Router, RouterModule, Routes } from '@angular/router';

import { LoginComponent } from './components/login/login';
import { RestaurantApply } from './components/Restaurant/restaurant-apply/restaurant-apply';
import { MainLayoutComponent } from './components/Restaurant/MainLayout/MainLayout';
import { ActionPendingComponent } from './components/Restaurant/action-pending/action-pending';
import { RestaurantDashboardComponent } from './components/Restaurant/dashboard/dashboard';
import { RestaurantProfileComponent } from './components/Restaurant/restaurant-profile/restaurant-profile';
import { DashboardAnalytics } from './components/Restaurant/dashboard-analytics/dashboard-analytics';
import { MenuManagement } from './components/Restaurant/menu-management/menu-management';
import { OrdersManagement } from './components/Restaurant/orders-management/orders-management';
import { AuthGuard } from './services/auth-guard';
import { AuthService } from './services/auth';
import { DeliveryManRegister } from './components/pages/auth/delivery-man-register/delivery-man-register';
import { RegisterComponent } from './components/pages/auth/register/register.component';
import { AddressComponent } from './components/pages/user-hustory/address/address.component';
import { PaymentComponent } from './components/pages/user-hustory/payment/payment.component';
import { MainuserComponent } from './components/pages/user-hustory/mainuser/mainuser.component';
import { ResetPassword } from './components/reset-password/reset-password';

import { OrderHistoryComponent } from './components/pages/order-history/order-history.component';
import { BillingPageComponent } from './components/pages/billing-page/billing-page.component';
import { HomeComponent } from './components/pages/home/home.component';
import { Getallresturant } from './components/pages/getallresturant/getallresturant';
import { Container } from './components/deliveryManDashboard/container/container';
import { Profile } from './components/deliveryManDashboard/profile/profile';
import { AvailbilityStatus } from './components/deliveryManDashboard/availbility-status/availbility-status';
import { Orders } from './components/deliveryManDashboard/orders/orders';
import { RestaurantItems } from './components/pages/restaurant-items/restaurant-items';
import { ShoppingCart } from './components/Shopping-cart/shopping-cart/shopping-cart';
import { PlaceOrder } from './components/place-order/place-order';
import { CustomerProfile } from './components/CustomerDashboard/customer-profile/customer-profile';
import { CustomerOrders } from './components/CustomerDashboard/customer-orders/customer-orders';
import { title } from 'process';
import { CustomerContainer } from './components/CustomerDashboard/customer-container/customer-container';
import { CustomerAddresses } from './components/CustomerDashboard/customer-addresses/customer-addresses';
import { CustomerInbobox } from './components/CustomerDashboard/customer-inbobox/customer-inbobox';

// Guard Functions for Angular 20
export const authGuard = () => {
  const loginService = inject(AuthService);
  const router = inject(Router);

  if (loginService.isLoggedIn() && !loginService.isTokenExpired()) {
    return true;
  } else {
    router.navigate(['/login']);
    return false;
  }
};

export const roleGuard = (requiredRole: string) => {
  return () => {
    const loginService = inject(AuthService);
    const router = inject(Router);

    if (loginService.hasRole(requiredRole)) {
      return true;
    } else {
      router.navigate(['/login']); //we need new component
      return false;
    }
  };
};

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'login', component: LoginComponent },
  { path: 'action-pending', component: ActionPendingComponent },
  { path: 'restaurant-apply', component: RestaurantApply },
  {path:'shoppingcart', component: ShoppingCart,canActivate:[roleGuard('Customer')]},
  {path:'placeorder',component:PlaceOrder,canActivate:[roleGuard('Customer')]},
  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [AuthGuard],
    children: [
      {
        path: 'dashboard',
        component: RestaurantDashboardComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'orders-management',
        component: OrdersManagement,
        canActivate: [AuthGuard],
      },
      {
        path: 'menu-management',
        component: MenuManagement,
        canActivate: [AuthGuard],
      },
      {
        path: 'dashboard-analytics',
        component: DashboardAnalytics,
        canActivate: [AuthGuard],
      },
      {
        path: 'restaurant-profile',
        component: RestaurantProfileComponent,
        canActivate: [AuthGuard],
      },
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' }, // Default redirect
    ],
  },
  // {
  //   path:'shoppingcart',
  //   loadComponent: () =>
  //     import('./components/Shopping-cart/shopping-cart/shopping-cart').then(
  //       (m) => m.ShoppingCart
  //     ),
  // },
  {
    path: 'confirm-email',
    loadComponent: () =>
      import('./components/confirm-email/confirm-email').then(
        (m) => m.ConfirmEmail
      ),
  },
  { path: 'reset-password', component: ResetPassword },
  {
    path: 'new-password',
    loadComponent: () =>
      import('./components/newpassword/newpassword').then((m) => m.NewPassword),
  },
  { path: 'getAllResturant', component: Getallresturant },

  {
    path: 'home',
    loadComponent: () =>
      import('./components/pages/home/home.component').then(
        (m) => m.HomeComponent
      ),
  },
  { path: 'home', component: HomeComponent },
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: 'getAllResturant', component: Getallresturant },
  { path: 'billing', component: BillingPageComponent },
  { path: 'order', component: OrderHistoryComponent },

  { path: 'restaurant/:name/items', component: RestaurantItems },

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
  //my config
  {
    path: 'DeliveryManDashboard',
    component: Container,
    canActivate: [authGuard, roleGuard('DeliveryMan')],
    children: [
      {
        path: '',
        redirectTo: 'profile',
        pathMatch: 'full',
      },
      {
        path: 'profile',
        component: Profile,
        canActivate: [authGuard, roleGuard('DeliveryMan')],
      },
      {
        path: 'availability',
        component: AvailbilityStatus,
        canActivate: [authGuard, roleGuard('DeliveryMan')],
      },
      {
        path: 'order',
        component: Orders,
        canActivate: [authGuard, roleGuard('DeliveryMan')],
      },
    ],
    title: 'Dashboard',
  },
  {
    path: 'CustomerDashboard',
    component: CustomerContainer,
    canActivate: [authGuard, roleGuard('Customer')],
    children: [
      {
        path: '',
        redirectTo: 'customer-profile',
        pathMatch: 'full',
      },
      {
        path: 'customer-profile',
        component: CustomerProfile,
        canActivate: [authGuard, roleGuard('Customer')],
      },
      {
        path: 'customer-addresses',
        component: CustomerAddresses,
        canActivate: [authGuard, roleGuard('Customer')],
      },
      {
        path: 'customer-orders',
        component: CustomerOrders,
        canActivate: [authGuard, roleGuard('Customer')],
      },
      {
        path: 'customer-inbox',
        component: CustomerInbobox,
        canActivate: [authGuard, roleGuard('Customer')],
      }
    ],
      title: 'Customer Dashboard',
  },

  { path: '**', redirectTo: '' },
];


@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}

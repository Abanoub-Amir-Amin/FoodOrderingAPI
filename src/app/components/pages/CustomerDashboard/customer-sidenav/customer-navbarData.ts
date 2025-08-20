import { RouterLink } from '@angular/router';

export const customernavbarData = [
  {
    RouterLink: '/CustomerDashboard/customer-profile',
    icon: 'fa fa-user',
    label: 'Profile',
  },
  {
    RouterLink: '/CustomerDashboard/customer-addresses',
    icon: 'fa-solid fa-location-dot',
    label: 'Adresses',
  },
  {
    RouterLink: '/CustomerDashboard/customer-orders',
    icon: 'fa-solid fa-toggle-ona-solid fa-bell-concierge',
    label: 'Order',
  },
    {
    RouterLink: '/CustomerDashboard/customer-inbox',
    icon: 'fa-solid fa-inbox',
    label: 'Inbox',
  },
  {
    RouterLink: '/CustomerDashboard/logout',
    icon: 'fa-solid fa-arrow-right-from-bracket',
    label: 'logout',
  }

];

// src/app/models/restaurant.model.ts

export interface RegisterCustomerDTO {
  firstName: string;
  lastName: string;
  userName: string;
  email: string;
  phoneNumber: string; // ✅ تعديل هنا
  password: string;
  confirmPassword: string; // ✅ مطلوب
  Address: AddressDto;
}
export interface AddressDto {
  label: string;
  street: string;
  city: string;
  latitude: number;
  longitude: number;
}
export interface UserDto {
  userId?: string;
  userName: string;
  email: string;
  phoneNumber?: string;
  role?: string;
  createdAt?: string;
  password?: string;
}

export interface RestaurantDto {
  restaurantId?: string;
  restaurantName: string;
  location: string;
  openHours?: string;
  isActive?: boolean;
  isAvailable?: boolean;
  imageFile?: string;
  user: UserDto;
}

export interface RestaurantUpdateDto {
  restaurantId?: string;
  restaurantName: string;
  location: string;
  openHours?: string;
  isActive?: boolean;
  isAvailable?: boolean | null; // bool? in backend, nullable boolean
  logoFile?: File; // IFormFile in backend, use File in frontend
  imageUrl?: string;
  user: UserDto;
}

export interface ItemDto {
  itemID?: string;
  name: string;
  description?: string;
  price: number;
  isAvailable: boolean;
  category: string;
  imageFile?: File;
  imageUrl?: string;
  restaurantID?: string;
}

export interface ItemUpdateDto {
  itemID?: string;
  name: string;
  description?: string;
  price: number;
  isAvailable: boolean;
  category: string;
  imageFile?: File; // corresponds to IFormFile in backend
  imageUrl?: string;
  restaurantID?: string;
}

export interface DiscountDto {
  discountID?: number;
  itemID?: string;
  itemName?: string;
  percentage: number;
  startDate: string;
  endDate: string;
}

export interface PromoCodeDto {
  promoCodeID?: string; // Guid as string
  code: string;
  discountPercentage: number;
  isFreeDelivery: boolean;
  issuedByType?: string;
  issuedByID?: string;
  expiryDate: string; // ISO string date
  usageLimit: number;
}

export interface OrderDto {
  orderID?: string;
  addressID: string; // Guid as string
  restaurantID: string;
  deliveryManID?: string | null;
  status: string;
  orderDate: string; // ISO string date
  deliveredAt?: string | null; // ISO string date or null
  totalPrice: number;
  promoCodeID?: string | null; // Guid or null
  orderItems: OrderItemDto[];
  customer: CustomerDto;
}

export interface OrderItemDto {
  orderItemId?: string;
  orderID: string; // Guid as string
  itemName: string;
  quantity: number;
  preferences: string;
  imageUrl: string;
}

export interface CustomerDto {
  customerId?: string;
  firstName: string;
  lastName: string;
  userName: string;
  email: string;
  phone: string;
  gender?: string | null; // GenderEnum? nullable in backend, string enum in frontend
  addresses?: string[]; // Assuming addresses as array of string addresses
  loyaltyPoints?: number;
  totalOrders?: number;
  totalDeliveredOrders?: number;
  totalCancelledOrders?: number;
  inProcessOrders?: OrderDto[];
  rewards?: string[];
  totalRewardspoints?: number;
}

export interface OrderStatusUpdateDto {
  orderID: string;
  status: string;
}

export interface DashboardSummaryDto {
  deliveredOrders: number;
  inProcessOrders: number;
  cancelledOrders: number;
}

export interface LoginResponseDto {
  token: string;
  role: string;
  userId: string;
}

export interface LoginRequestDto {
  username: string;
  password: string;
}

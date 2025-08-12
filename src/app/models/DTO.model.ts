// src/app/models/restaurant.model.ts

 export interface RegisterCustomerDTO {
 firstName: string;
  lastName: string;
  userName: string;
  email: string;
  phoneNumber: string; // ✅ تعديل هنا
  password: string;
  confirmPassword: string; // ✅ مطلوب
 Address:AddressDto;
}
export interface AddressDto{
  label: string;     
  street: string;  
  city: string;       
  latitude: number;   
  longitude: number;  
}
export interface UserDto {
  UserId?: string
  UserName: string;
  Email: string;
  Phone?: string;
  Password?: string;
}

export interface RestaurantDto {
  RestaurantId?: string
  RestaurantName: string;
  Location: string;
  OpenHours?: string;
  IsActive?: boolean;
  IsAvailable?: boolean;
  LogoUrl?: string;
  User: UserDto;
}

export interface RestaurantUpdateDto {
  RestaurantId?: string
  RestaurantName: string;
  Location: string;
  OpenHours?: string;
  IsActive?: boolean;
  IsAvailable?: boolean | null; // bool? in backend, nullable boolean
  LogoFile?: File; // IFormFile in backend, use File in frontend
  User: UserDto;
}

export interface ItemDto {
  ItemID?: string
  Name: string;
  Description?: string;
  Price: number;
  IsAvailable: boolean;
  Category: string;
  ImageFile?: File; // corresponds to IFormFile in backend
}

export interface DiscountDto {
  DiscountId?: number
  ItemID: string;       // Guid as string
  RestaurantName?: string;
  ItemName?: string;
  Percentage: number;
  StartDate: string;    // ISO string date
  EndDate: string;      // ISO string date
}

export interface PromoCodeDto {
  PromoCodeID: string;  // Guid as string
  Code: string;
  DiscountPercentage: number;
  IsFreeDelivery: boolean;
  IssuedByType?: string;
  IssuedByID?: number;
  ExpiryDate: string;   // ISO string date
  UsageLimit: number;
}

export interface OrderDto {
  OrderID?: string
  AddressID: string;           // Guid as string
  RestaurantID: string;
  DeliveryManID?: string | null;
  Status: string;
  OrderDate: string;           // ISO string date
  DeliveredAt?: string | null; // ISO string date or null
  TotalPrice: number;
  PromoCodeID?: string | null; // Guid or null
  OrderItems: OrderItemDto[];
  Customer: CustomerDto
}

export interface OrderItemDto {
  OrderItemId?: string
  OrderID: string;       // Guid as string
  ItemName: string;
  Quantity: number;
  Preferences: string;
  ImageUrl: string;
}

export interface CustomerDto {
  CustomerId?: string
  FirstName: string;
  LastName: string;
  UserName: string;
  Email: string;
  Phone: string;
  Gender?: string | null;          // GenderEnum? nullable in backend, string enum in frontend
  Addresses?: string[];            // Assuming addresses as array of string addresses
  LoyaltyPoints?: number;
  TotalOrders?: number;
  TotalDeliveredOrders?: number;
  TotalCancelledOrders?: number;
  InProcessOrders?: OrderDto[];
  Rewards?: string[];
  TotalRewardspoints?: number;
}

export interface OrderStatusUpdateDto {
  OrderID: string;  
  Status: string;
}

export interface DashboardSummaryDto {
  DeliveredOrders: number;
  InProcessOrders: number;
  CancelledOrders: number;
}

export interface LoginResponseDto {
  Token: string;
  Role: string;
  UserId: string;
}

export interface LoginRequestDto {
  Username: string;
  Password: string;
}


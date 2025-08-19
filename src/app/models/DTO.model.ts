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
export interface ShoppingCartDto {
  cartID: string; // Guid
  restaurantID?: string; // nullable
  restaurantName?: string; // nullable
  updatedAt: string; // DateTime => string (ISO format)
  subTotal: number;
  delivaryPrice: number;
  totalAfterDiscount: number; // من الـ API أو بتحسبه
  shoppingCartItems: {
    $id:string;
    $values: ShoppingCartItemDto[];
  };
}
export interface ShoppingCartItemDto {
  shoppingCartItemId: string;
  imageFile:string;
  itemName: string;
  quantity: number;
  preferences: string;
  totalPrice: number;
}
export interface ShoppingCartItemAddedDTO {
  cartID: string;       // Guid → string
  itemID: string;       // Guid → string
  preferences?: string; // nullable → optional
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
  restaurantID?: string;
  restaurantName: string;
  location: string;
  openHours?: string;
  isActive?: boolean;
  isAvailable?: boolean;
  imageFile?: string;
  latitude?: number;
  longitude?: number;
  delivaryPrice?: number;
  orderTime?: string;
  user: UserDto;
}

export interface RestaurantUpdateDto {
  restaurantID?: string;
  restaurantName: string;
  location: string;
  openHours?: string;
  isAvailable?: boolean | null; // bool? in backend, nullable boolean
  logoFile?: File; // IFormFile in backend, use File in frontend
  latitude?: number;
  longitude?: number;
  delivaryPrice?: number;
  orderTime?: string;
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
export interface ItemUpdateDto {
  itemID?: string;
  name: string;
  description?: string;
  price: number;
  isAvailable: boolean;
  category: string;
  imageFile?: File;  // corresponds to IFormFile in backend
  imageUrl?: string;
  restaurantID?: string;
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
  orderItems: {$id:string,$value:OrderItemDto[]};
  customer: CustomerDto;
}

export interface OrderItemDto {
  orderItemId?: string;
  orderID: string; // Guid as string
  itemName: string;
  quantity: number;
  preferences: string;
  imageUrl: string;
  totalPrice: number;
}

export interface CustomerDto {
  customerId?: string;
  firstName: string;
  lastName: string;
  userName: string;
  email: string;
  phone: string;
  gender?: GenderEnum | null; // GenderEnum? nullable in backend, string enum in frontend
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

// export interface AddressDTO {
//     label: string; // MaxLength(50)
//     street: string; // MaxLength(255)
//     city: string; // MaxLength(100)
//     latitude: number; // Range(-90, 90)
//     longitude: number; // Range(-180, 180)
// }

export interface AddressViewDto {
    addressID: string; // Guid
    customerID?: string | null;
    label: string; // MaxLength(50)
    street: string; // MaxLength(255)
    city: string; // MaxLength(100)
    isDefault: boolean;
    latitude: number;
    longitude: number;
}

export interface CheckoutViewDTO {
    restaurantName: string;
    items: ShoppingCartItemDto[];
    phoneNumber: string;
    address: AddressViewDto;
    subTotal: number;
    delivaryPrice: number;
    discountAmount: number;
    totalPrice: number; // بيجي من الـ API أو ممكن نحسبه في الفرونت
    paymentLink: string;
    // paymentMethod?: string;
}
export interface OrderDetailDTO {
    orderNumber: number;
    orderDate: string; // ISO Date string
    status: StatusEnum;

    items: {$id:string,$values:OrderItemDto[];}

    restaurantName: string;
    restaurantLocation: string;
    restaurantPhone: string;

    delivaryName: string;
    orderTimeToComplete: string; // TimeSpan -> ISO string or duration format
    address: string;

    subTotal: number;
    delivaryPrice: number;
    discountAmount: number;
    totalPrice: number;
}

export interface OrderViewDTO {
    orderID: string; // Guid
    orderNumber: number;
    status: StatusEnum;
    restaurantName: string;
    itemNames: {$id:string,$values:string[]};
    orderDate: string; // ISO Date string
    totalPrice: number;
}
export interface UpdateCustomerDTO {
    FirstName: string;
    LastName: string;
    PhoneNumber?: string | null;
    Gender?: GenderEnum | null;
}

// تعريف GenderEnum (لازم يكون مطابق للـ C# Enum)
export enum GenderEnum {
    Male = 0,
    Female = 1
}

// لو فيه Enums أو Types مستخدمة
export enum StatusEnum {
    All = 0,
    WaitingToConfirm = 1,
    Preparing = 2,
    Out_for_Delivery = 3,
    Delivered = 4,
    // Cancelled = 5
}


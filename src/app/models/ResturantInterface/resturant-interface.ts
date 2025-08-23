export interface ResturantInterface {
  id: string;
  restaurantName: string;
  location: string;
  openHours: string;
  delivaryPrice:string;
  rating: number | null;
  imageFile: string;
}
export interface RestaurantItem {
  itemID: string;
  name: string;
  imageFile: string;
  description: string;
  price: number;
  category: string;
  discountedPrice: number;
}
export interface ItemDto {
  itemID: string;             // Guid => string في Angular
  restaurantID: string;

  name: string;
  imageFile?: string | null;  // nullable في C# => optional في TS
  description: string;

  price: number;
  discountedPrice: number;

  isAvailable: boolean;
  category: string;

  stripePriceId: string;
  stripeProductId: string;

  // لو هتحتاجي تعرض بيانات المطعم مع المنتج
 ResturantInterface?: any; 

  // لو هتجيبي خصومات أو علاقات
  discounts?: any[];
  orderItems?: any[];
  shoppingCartItems?: any[];
}

export interface ShoppingCart {
  getCart(customerId: string): unknown;
  cartID: string;
  restaurantID?: string;
  restaurantName?: string;
  updatedAt: string;
  subTotal: number;
  discountAmount: number;
  delivaryPrice: number;
  totalAfterDiscount: number;
  shoppingCartItems: any[];
}
export interface AddItemDto {
  cartID: string;
  itemID: string;
  quantity: number;
}
export interface ShoppingCartResponse {
  cartID: string;
  restaurantID?: string;
  restaurantName?: string;
  updatedAt: string;
  subTotal: number;
  discountAmount: number;
  delivaryPrice: number;
  totalAfterDiscount: number;
  shoppingCartItems: any[];
}

export interface ResturantInterface {
  id: string;
  restaurantName: string;
  location: string;
  openHours: string;
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

export interface OrderItemDto {
  orderID: string;
  itemName: string;
  quantity: number;
  preferences: string;
  imageFile: string;
  totalPrice: number;
}

export interface DeliveryOrderDTO {
  orderID: string;
  orderNumber: number;
  customerName: string;
  customerAddress: string;
  customerPhone: string;
  restaurantName: string;
  restaurantAddress: string;
  restaurantPhone?: string;
  items: OrderItemDto[];
  orderDate: string;
  totalPrice: number;
  status?: OrderStatus;
  deliveredAt?: string;
}

export enum OrderStatus {
  Preparing = 2,
  Out_for_Delivery = 3,
  Delivered = 4,
}

export interface UpdateOrderStatusRequest {
  orderID: string;
  status: OrderStatus;
  deliveryManId: string;
}

export interface NotificationDTO {
  userId: string;
  message: string;
}

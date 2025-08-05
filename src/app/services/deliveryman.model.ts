export interface DeliveryManRegistration {
  userName: string;
  password: string;
  email: string;
  phoneNumber: string;
  agreeTerms: boolean;
  latitude: number;
  longitude: number;
}

export interface ApiResponse {
  success: boolean;
  message: string;
  data?: any;
}

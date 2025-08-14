export interface Ilogin {
  username: string;
  password: string;
}

export interface loginResponse {
  $id: string;
  role: string;
  token: string;
  userId: string;
}

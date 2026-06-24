export interface Room {
  id: string;
  name: string;
  capacity: number;
}

export interface BookingResponse {
  id: string;
  roomId: string;
  startTime: string;
  endTime: string;
}

export interface CreateBookingDto {
  startTime: string;
  endTime: string;
}

export interface CreateRoomDto {
  name: string;
  capacity: number;
}

export interface RegisterDto {
  email: string;
  username: string;
  password: string;
  fullName?: string;
}

export interface LoginDto {
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  userId: string;
  email: string;
  username: string;
  fullName?: string;
}

export interface ProblemDetails {
  status: number;
  title: string;
  detail: string;
  errors?: Record<string, string[]>;
}

export interface User {
  userId: string;
  email: string;
  username: string;
  fullName?: string;
}

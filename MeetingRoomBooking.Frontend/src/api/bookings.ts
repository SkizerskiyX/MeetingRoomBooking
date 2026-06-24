import client from './client';
import type { BookingResponse, CreateBookingDto } from '../types';

export const bookingsApi = {
  getByRoom: (roomId: string) =>
    client
      .get<BookingResponse[]>(`/api/rooms/${roomId}/bookings`)
      .then((r) => r.data),

  getById: (roomId: string, id: string) =>
    client
      .get<BookingResponse>(`/api/rooms/${roomId}/bookings/${id}`)
      .then((r) => r.data),

  create: (roomId: string, data: CreateBookingDto) =>
    client
      .post<BookingResponse>(`/api/rooms/${roomId}/bookings`, data)
      .then((r) => r.data),

  delete: (roomId: string, bookingId: string) =>
    client.delete(`/api/rooms/${roomId}/bookings/${bookingId}`),
};

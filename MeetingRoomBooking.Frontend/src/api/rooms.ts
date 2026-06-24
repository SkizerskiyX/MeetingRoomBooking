import client from './client';
import type { Room, CreateRoomDto } from '../types';

export const roomsApi = {
  getAll: () => client.get<Room[]>('/api/rooms').then((r) => r.data),

  getById: (id: string) =>
    client.get<Room>(`/api/rooms/${id}`).then((r) => r.data),

  create: (data: CreateRoomDto) =>
    client.post<Room>('/api/rooms', data).then((r) => r.data),

  update: (id: string, data: CreateRoomDto) =>
    client.put(`/api/rooms/${id}`, data),

  delete: (id: string) => client.delete(`/api/rooms/${id}`),
};

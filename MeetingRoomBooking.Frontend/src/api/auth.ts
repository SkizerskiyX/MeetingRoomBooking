import client from './client';
import type { RegisterDto, LoginDto, AuthResponse } from '../types';

export const authApi = {
  register: (data: RegisterDto) =>
    client.post<AuthResponse>('/api/auth/register', data).then((r) => r.data),

  login: (data: LoginDto) =>
    client.post<AuthResponse>('/api/auth/login', data).then((r) => r.data),
};

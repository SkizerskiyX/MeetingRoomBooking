import axios from 'axios';

const API_URL = import.meta.env.VITE_API_URL || '';

const client = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor to attach token
client.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('auth_token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Response interceptor for global 401 handling
client.interceptors.response.use(
  (response) => response,
  (error) => {
    // Don't auto-clear token on 401 to avoid unexpected logout during operations like delete.
    // Let callers handle 401 and show appropriate UI messages. Keep token in storage.
    return Promise.reject(error);
  }
);

export default client;

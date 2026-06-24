import {
  createContext,
  useContext,
  useState,
  useEffect,
  useCallback,
  type ReactNode,
} from 'react';
import type { User, LoginDto, RegisterDto } from '../types';
import { authApi } from '../api/auth';

interface AuthContextType {
  user: User | null;
  token: string | null;
  isLoading: boolean;
  login: (data: LoginDto) => Promise<void>;
  register: (data: RegisterDto) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const storedToken = localStorage.getItem('auth_token');
    const storedUser = localStorage.getItem('auth_user');
    if (storedToken && storedUser) {
      setToken(storedToken);
      setUser(JSON.parse(storedUser));
    }
    // In development, if no token is present, try to auto-login with demo account
    async function tryAutoLogin() {
      if (!storedToken && import.meta.env.DEV) {
        try {
          const response = await authApi.login({ email: 'demo@local', password: 'Password123!' });
          const userData: User = {
            userId: response.userId,
            email: response.email,
            username: response.username,
            fullName: response.fullName,
          };
          localStorage.setItem('auth_token', response.token);
          localStorage.setItem('auth_user', JSON.stringify(userData));
          setToken(response.token);
          setUser(userData);
        } catch {
          // ignore auto-login failures
        }
      }
      setIsLoading(false);
    }

    void tryAutoLogin();
  }, []);

  const login = useCallback(async (data: LoginDto) => {
    const response = await authApi.login(data);
    const userData: User = {
      userId: response.userId,
      email: response.email,
      username: response.username,
      fullName: response.fullName,
    };
    localStorage.setItem('auth_token', response.token);
    localStorage.setItem('auth_user', JSON.stringify(userData));
    setToken(response.token);
    setUser(userData);
  }, []);

  const register = useCallback(async (data: RegisterDto) => {
    const response = await authApi.register(data);
    const userData: User = {
      userId: response.userId,
      email: response.email,
      username: response.username,
      fullName: response.fullName,
    };
    localStorage.setItem('auth_token', response.token);
    localStorage.setItem('auth_user', JSON.stringify(userData));
    setToken(response.token);
    setUser(userData);
  }, []);

  const logout = useCallback(() => {
    localStorage.removeItem('auth_token');
    localStorage.removeItem('auth_user');
    setToken(null);
    setUser(null);
  }, []);

  return (
    <AuthContext.Provider
      value={{ user, token, isLoading, login, register, logout }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}

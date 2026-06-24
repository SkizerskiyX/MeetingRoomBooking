import { AxiosError } from 'axios';
import type { ProblemDetails } from '../types';

export function isAxiosError(error: unknown): error is AxiosError<ProblemDetails> {
  return error instanceof AxiosError;
}

export function getErrorMessage(error: unknown): string {
  if (isAxiosError(error)) {
    const data = error.response?.data;
    if (data?.detail) return data.detail;
    if (data?.title) return data.title;
    if (error.response?.status === 401) return 'Please login to continue';
    if (error.response?.status === 403) return 'You do not have permission';
    if (error.response?.status === 404) return 'Resource not found';
    if (error.response?.status === 409) return data?.detail || 'Conflict occurred';
    if (error.message === 'Network Error') return 'Unable to connect to server';
    return error.message || 'An unexpected error occurred';
  }
  if (error instanceof Error) return error.message;
  return 'An unexpected error occurred';
}

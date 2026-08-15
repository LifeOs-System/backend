import { apiClient } from './api-client';
import type { AxiosRequestConfig } from 'axios';

export async function apiRequest<T>(
  method: 'get' | 'post' | 'put' | 'delete',
  url: string,
  body?: unknown,
  config?: AxiosRequestConfig
): Promise<ApiResponse<T>> {
  const finalConfig = { ...config };

  if (body instanceof FormData) {
    delete finalConfig.headers?.['Content-Type'];
  }

  try {
    let response;
    if (method === 'get' || method === 'delete') {
      
      response = await apiClient[method](url, finalConfig);
      console.log(response)
    } else {
      response = await apiClient[method](url, body, finalConfig);
    }
    return mapToApiResponse<T>(response.data);
    
  } catch  {

    return {
      isSuccess: false,
      isFailure: true,
      message:  'Error de conexión con el servidor',
      errors: [{
        code: 'NETWORK_ERROR',
        message:'No se pudo conectar con el servidor',
        httpCode: 500,
      }],
      value: undefined as T,
     };
  }
}

function mapToApiResponse<T>(backendData:  ApiResponse<T>): ApiResponse<T> {
  return {
    isSuccess: backendData.isSuccess ?? false,
    isFailure: backendData.isFailure ?? true,
    message: backendData.message ?? '',
    errors: backendData.errors ?? [],
    value: backendData.value as T, 
  };
}
import axios, { InternalAxiosRequestConfig } from 'axios';
import https from 'https';

const token=process.env.TOKEN_BEARER
// Configuración de rate limiting
const rateLimitConfig = {
  maxRequests: 10,       
  perMilliseconds: 60000,
  requests: [] as number[] 
};

async function checkRateLimit(config: InternalAxiosRequestConfig) {
  const now = Date.now();
  
  // Limpiar peticiones antiguas (fuera de la ventana de tiempo)
  rateLimitConfig.requests = rateLimitConfig.requests.filter(
    timestamp => now - timestamp < rateLimitConfig.perMilliseconds
  );
  
  // Verificar si excede el límite
  if (rateLimitConfig.requests.length >= rateLimitConfig.maxRequests) {
    const oldestRequest = rateLimitConfig.requests[0];
    const waitTime = rateLimitConfig.perMilliseconds - (now - oldestRequest);

    console.warn(`Rate limit excedido. Esperando ${Math.ceil(waitTime / 1000)} segundos...`);
    
    // Esperar antes de continuar
    await new Promise(resolve => setTimeout(resolve, waitTime));
  }
  
  // Registrar esta petición
  rateLimitConfig.requests.push(now);
  
  return config;
}

export const apiClient = axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_URL,
  timeout: 15000, 
  // Solo en desarrollo: aceptar certificados autofirmados
  httpsAgent: process.env.NODE_ENV === 'production' 
    ? undefined 
    : new https.Agent({ rejectUnauthorized: false }),
    validateStatus: (status) => (status >= 200 && status < 300) || (status >= 400 && status < 500),
});

apiClient.interceptors.request.use(checkRateLimit);

// Interceptor: Inyecta Bearer si existe cookie
apiClient.interceptors.request.use(async (config) => { 
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
  return config;
});

// Interceptor: Formatea errores de red/timeout para React Query
apiClient.interceptors.response.use(
  (res) => res,
  (err) => {
    if (err.code === 'ECONNABORTED') {
      return Promise.reject(new Error('La petición tardó demasiado. Intenta de nuevo.'));
    }
    return Promise.reject(err);
  }
);
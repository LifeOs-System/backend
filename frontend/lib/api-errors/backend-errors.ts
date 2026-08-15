const ERROR_CODE_MAP: Record<string, string> = {
  'UNAUTHORIZED': 'No autorizado',
  'NOT_FOUND': 'notFound',
  'VALIDATION_ERROR': 'validationError',
  'SERVER_ERROR': 'serverError',
  'BAD_REQUEST': 'badRequest',
  'FORBIDDEN': 'forbidden',
  'INTERNAL_SERVER_ERROR': 'internalServerError',
  'USER_ALREADY_EXISTS': 'userAlreadyExists',
  'INVALID_CREDENTIALS': 'invalidCredentials',
  'TOKEN_EXPIRED': 'tokenExpired',
  'RATE_LIMIT_EXCEEDED': 'rateLimitExceeded',
  'DATABASE_ERROR': 'databaseError',
  'EMAIL_ALREADY_IN_USE': 'emailAlreadyInUse',
  'WEAK_PASSWORD': 'weakPassword',
  'INVALID_EMAIL_FORMAT': 'invalidEmailFormat',
  'RESOURCE_NOT_FOUND': 'resourceNotFound',
  'ACCESS_DENIED': 'accessDenied',
  'DUPLICATE_ENTRY': 'duplicateEntry'
};

export const useBackendError = () => {
  
  const getErrorMessage = (errorCode: string): string => {
    const translationKey = ERROR_CODE_MAP[errorCode];
    
    if (translationKey) {
      return translationKey;
    }
    
    return "Error desconocido";
  };
  
  return { getErrorMessage };
};
type ApiResponse<T = unknown> ={
  value: T | null;
  message: string;
  isSuccess: boolean;
  isFailure: boolean;
  errors:ApiError[];
}

type ApiError = {
    code:string;
    message:string;
    httpCode:number;
}

type ReviewsPendingAdmin = {
    totalPendingCount: number,
    pendingBusinessCount: number,
    pendingDishGroupCount: number,
    pendingDishCount: number
}

type useHookApi<T> = {
  onSuccess?: (data: T) => void;
  onError?: (errorMessage: string) => void;
}
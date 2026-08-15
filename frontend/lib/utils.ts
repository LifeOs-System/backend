import { clsx, type ClassValue } from "clsx"
import { twMerge } from "tailwind-merge"
import axios from "axios";

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs))
}

export function createApiResponse<T>(
    response: ApiResponse<T>
): ApiResponse<T> {
    if (response.isFailure===true) {
        return {
            isSuccess: false,
            isFailure: true,
            message: response.message,
            errors: response.errors || [],
            value: null,
        };
    }

    return {
        isSuccess: true,
        isFailure: false,
        message: response.message,
        errors: [],
        value: response.value!,
    };
}

export const getInitials = (name: string): string => {
  return name
    .split(" ")
    .slice(0, 2)      
    .map((word) => word[0])
    .join("")
    .toUpperCase();
};
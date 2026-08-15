import { apiRequest } from "../api-request";
import { createApiResponse } from "@/lib/utils";

export async function fetchSummaryPending(
): Promise<ApiResponse<ReviewsPendingAdmin>> {
    const response = await apiRequest<ReviewsPendingAdmin>(
        'get',
        'admin/summary/pending',
    );

   return createApiResponse(response);
}

export async function createBusiness({formData}:{formData:FormData}
): Promise<ApiResponse<string>> {
    const response = await apiRequest<string>(
        'post',
        `business-manager/businesses/create`,
        formData
    );

   return createApiResponse(response);
}

import { useBackendError } from "@/lib/api-errors/backend-errors";
import { createBusiness } from "@/lib/api/example/example";
import { useMutation, useQueryClient } from "@tanstack/react-query";

//post
export function useCreateBusiness({
    onSuccess,
    onError,
}: useHookApi<string>) {
    
    const { getErrorMessage } = useBackendError()
    
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (formData: FormData) => createBusiness({ formData }),
        onSuccess: (result) => {
            if (result.isSuccess) {
                queryClient.invalidateQueries({ queryKey: ["businesses"] });
                onSuccess?.(result.value || "Negocio creado exitosamente");
            } else {
                onError?.(getErrorMessage(result.errors[0].code) || "Error desconocido");
            }
        },
        onError: (error: Error) => {
            onError?.(error?.message || "Error al crear el negocio");
        },
    });
}
import { fetchSummaryPending } from "@/lib/api/example/example";
import { useQuery } from "@tanstack/react-query";

//get
export function useSummaryPendings() {
  const {
    data: pendings,
    isLoading: isLoading,
    isError: isError,
    error:error,
  } = useQuery({
    queryKey: ["admin"],
    queryFn: () => fetchSummaryPending().then((res) => res.value),
  });

  return { pendings: pendings, error,isLoading, isError };
}
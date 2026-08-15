"use client";

import { useSummaryPendings } from "@/hooks/reviews/use-reviews";

export default function HomePage() {
  const { isLoading, pendings, isError,error } = useSummaryPendings();

  if (isError && !isLoading) {
    return <div>{error?.message}</div>;
  }

  return (
    <div className="p-4">
      <h1>Home</h1>
    </div>
  );
}

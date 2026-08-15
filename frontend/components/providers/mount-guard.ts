"use client"

import { useMounted } from "@/hooks/use-mounted";

export default function MountGuard({ children }: { children: React.ReactNode }) {
    const mounted = useMounted();
    if (!mounted) {
        return null;
    }
    return children;
}
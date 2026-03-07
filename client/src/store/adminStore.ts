import { create } from "zustand";

interface AdminState {
  isAdmin: boolean;
  toggleAdmin: () => void;
}

export const useAdminStore = create<AdminState>((set) => ({
  isAdmin: false,
  toggleAdmin: () => set((state) => ({ isAdmin: !state.isAdmin })),
}));

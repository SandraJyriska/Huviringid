import type { User } from "~/types/user";
import type { NitroFetchOptions, NitroFetchRequest } from "nitropack";
import { useUserStore } from "~/stores/userStore";

export const useAuth = () => {
  const userStore = useUserStore();

  const api = useApi();
  const router = useRouter();
  const isAuthenticated = computed(() =>  userStore.isAuthenticated);

  const initialize = async () => {
    if (typeof window !== "undefined") {
      await userStore.initializeUser();
    }
  };

  const logIn = async (userData: User) => {
    try {
      const response = await api.customFetch<{ token: string, role: string, userId: number }>("Users/Login", {
        method: "POST",
        body: userData,
      });
  
      if (response.token && response.token.trim().length > 0) {
        userStore.setUserInfo(response.role, response.token, response.userId);
        return true;  
      } else {
        return false; 
      }
    } catch (error) {
      // Handle fetch errors (like 401 Unauthorized)
      console.error("Login failed:", error);
      return false; 
    }
  };
  
  const logOut = async() => {
    userStore.clearUser();
    router.push('/');
  };

  const fetchWithToken = async <T>(
    url: string,
    options?: NitroFetchOptions<NitroFetchRequest>
  ) => {
    return await api.customFetch<T>(url, {
        ...options, 
        headers: {
            Authorization: 'Bearer ' + userStore.token,
            ...options?.headers
        }
    });
  };

  return { logIn, logOut, fetchWithToken, initialize, isAuthenticated };
};

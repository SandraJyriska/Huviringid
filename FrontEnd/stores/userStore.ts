export const useUserStore = defineStore('user', () => {
    const token = ref<string | null>(null);  
    const role = ref<string | null>(null);
    const userId = ref<number | null>(null);
  
    const isAuthenticated = computed(() => !!token.value);
    const api = useApi();
  
    const initializeUser = async () => {
      if (typeof window !== 'undefined') {
        const storedRole = localStorage.getItem("role");
        const storedToken = localStorage.getItem("token");
        const storedUserId = localStorage.getItem("userId");     

        if (storedRole && storedToken && storedUserId) {
          role.value = storedRole;
          token.value = storedToken;
          userId.value = parseInt(storedUserId, 10);
        }
      }
    };
  
    const setUserInfo = (roleValue: string, tokenValue: string, userIdValue: number) => {
      role.value = roleValue;
      token.value = tokenValue;
      userId.value = userIdValue;
  
      if (typeof window !== 'undefined') {
        localStorage.setItem("role", roleValue);
        localStorage.setItem("token", tokenValue);
        localStorage.setItem("userId", userIdValue.toString());
      }
    };
  
    const clearUser = () => {
      role.value = null;
      token.value = null;
      userId.value = null;
  
      if (typeof window !== 'undefined') {
        localStorage.removeItem("role");
        localStorage.removeItem("token");
        localStorage.removeItem("userId");
      }
    };

    const isUsernameExisting = async (username: string) => {
      const res = await api.customFetch<boolean>(`Users/Username-Check/${username}`);
      return res;
    }

    const addUser = async (user: User): Promise<User> => {
      const res = await api.customFetch<User>('Users', {
        method: 'POST',
        body: user,
      });
      return res;
    }
  
    return { token, role, userId, isAuthenticated, clearUser, initializeUser, setUserInfo, isUsernameExisting, addUser };
  });
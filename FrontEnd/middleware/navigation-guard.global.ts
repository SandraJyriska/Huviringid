import { useAuth } from "~/composables/useAuth";

export default defineNuxtRouteMiddleware((to, from) => {
    const auth = useAuth();
    
    if (import.meta.client) {
      if (to.path !== '/' && !auth.isAuthenticated.value && to.path !== '/register-user-form') {
        return navigateTo('/');
      }
    }
  })

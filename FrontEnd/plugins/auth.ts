import { useAuth } from '~/composables/useAuth';

export default defineNuxtPlugin(async nuxtApp => {
  const { initialize, logOut } = useAuth();
  const route = useRouter();
  
  // Ensure the initialization runs on the client side (browser)
  if (import.meta.client) {
    await initialize();

    const token = localStorage.getItem('token');
    
    if (token === null) {
      logOut(); 
      return;
    }

    const decodedToken = JSON.parse(atob(token.split('.')[1])); 
    const expiryDate = decodedToken.exp * 1000; 
    if (expiryDate < Date.now()) {
      logOut(); 
    }
  }
});
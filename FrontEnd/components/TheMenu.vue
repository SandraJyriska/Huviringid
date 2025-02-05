<template>
  <div class="navbar">
    <UHorizontalNavigation
      :links="filteredLinks"
      class="custom-navigation"
    />
    <div class="user-info">
      <div v-if="person" class="user-menu">
        <span class="user-icon" @click="toggleMenu">
          <span class="user-name">{{ getFullName(person) }}</span>

          <!-- Kasutaja ikoon -->
          <svg 
            xmlns="http://www.w3.org/2000/svg" 
            fill="none" 
            viewBox="0 0 24 24" 
            stroke-width="1.0" 
            stroke="currentColor" 
            class="icon">
            <path 
              stroke-linecap="round" 
              stroke-linejoin="round" 
              d="M15.75 6a3.75 3.75 0 1 1-7.5 0 3.75 3.75 0 0 1 7.5 0ZM4.501 20.118a7.5 7.5 0 0 1 14.998 0A17.933 17.933 0 0 1 12 21.75c-2.676 0-5.216-.584-7.499-1.632Z" />
          </svg>
        </span>

        <div v-if="menuOpen" class="dropdown-menu" @click.self="closeModal">
          <ul>
            <li @click="goToProfile">Profiil</li>
            <li @click="logOut" class="logout-item">

              <!-- Välja logimise ikoon -->
              <svg xmlns="http://www.w3.org/2000/svg" 
                fill="none" 
                viewBox="0 0 30 30" 
                stroke-width="1.0" 
                stroke="currentColor" 
                class="logout-icon">
                <path 
                  stroke-linecap="round" 
                  stroke-linejoin="round" 
                  d="M15.75 9V5.25A2.25 2.25 0 0 0 13.5 3h-6a2.25 2.25 0 0 0-2.25 2.25v13.5A2.25 2.25 0 0 0 7.5 21h6a2.25 2.25 0 0 0 2.25-2.25V15m3 0 3-3m0 0-3-3m3 3H9" />
              </svg>
              Logi välja 
            </li>
          </ul>
        </div>
      </div>
    </div>
  </div>
</template>
  
  <script setup lang="ts">
  import { useUserStore } from '~/stores/userStore';
  const userStore = useUserStore();
  const auth = useAuth();
  const role = ref<string | null>(null);
  const userId = ref<number | null>(null);
  const studentStore = useStudentStore();
  const teacherStore = useTeacherStore();
  const extracurricularStore = useExtracurricularStore();
  const person = ref<any>(null);
  const router = useRouter();
  const menuOpen = ref(false); // Dropdown menüü avatud/seisund
  const emit = defineEmits(['close']);

  
  const links = [
    {
      label: "Vaata saabuvaid tunde",
      to: "/upcoming-lessons",
    },
    {
      label: "Registreeri huviringi",
      to: "/register-student-form",
    },
    {
      label: "Õpilaste nimekiri",
      to: "/registered-student-list",
    },
    {
      label: "Kinnita õpilased",
      to: "/student-request-list",
    },
    {
      label: "Registreeri huviring",
      to: "/register-extracurricular-form",
    }
  ];

  const filteredLinks = computed(() => {
    if (role.value === null) {
      return links.filter(link => link.to === "/" || link.to === "/register-user-form"
      );
    }

    if (role.value === "Student") {
      return links.filter(link => 
      link.to === "/upcoming-lessons" || link.to === "/register-student-form" 
      );
    }

    if (role.value === "Teacher") {
      return links.filter(link => 
      link.to === "/registered-student-list" || link.to === "/student-request-list" || link.to === "/register-extracurricular-form"
      );
    }

    return links.filter(link => link.to === "/");
  });

  const handleOutsideClick = (event: MouseEvent) => {
    const dropdown = document.querySelector('.dropdown-menu');
    const userIcon = document.querySelector('.user-icon');
    if (
      dropdown &&
      !dropdown.contains(event.target as Node) &&
      !userIcon?.contains(event.target as Node)
    ) {
      closeModal();
    }
  };

  const getPersonByRoleAndUserId = async () => {   
    if (userId.value !== null) {
      if (role.value === 'Student') {
        person.value = await studentStore.getStudentByUserId(userId.value);
      } else if (role.value === 'Teacher') {
        person.value = await teacherStore.getTeacherByUserId(userId.value);
        if (person.value) {
      }
      } else {
        person.value = null; // If no role matches
      }
    } else {
      person.value = null; // If userId is null
    }
  };
  
  const getFullName = (person: any) => {
    if (role.value === 'Student') {
      return `${person.firstName} ${person.lastName}`;
    } else if (role.value === 'Teacher') {
      return person.name;
    }
    return 'No name available';
  };

  const toggleMenu = () => {
    menuOpen.value = !menuOpen.value; // Menüü avamine/sulgemine
    console.log("Rippmenüü olek ", menuOpen.value)
    if (menuOpen.value) {
      document.addEventListener('click', handleOutsideClick);
    } else {
      closeModal();
    }

  };

  const goToProfile = () => {
    menuOpen.value = false;
    router.push("/profile"); // Suunab profiili lehele
    toggleMenu();
    closeModal();

  };


  const logOut = async () => {
    await auth.logOut();
    person.value = null;
    role.value = null;
    userId.value = null;
    closeModal();
    router.push('/');
  };

  function closeModal() {
    menuOpen.value = false;
    document.removeEventListener('click', handleOutsideClick);
  };

  onMounted(async () => {   
    await auth.initialize();
    role.value = userStore.role;
    userId.value = userStore.userId;

    if (userId.value !== null) {
      await getPersonByRoleAndUserId();
    }
    if (person.value === null) {
      router.push('/');
     }  
  });

  // Watch for changes in userStore and update person/reactively
  watch(
    () => userStore.userId,
    async (newUserId) => {
      userId.value = newUserId;
      role.value = userStore.role; 
      await getPersonByRoleAndUserId();
    },
    { immediate: true } // Run immediately to catch the initial state
  );

  watch(
    () => userStore.role,
    async (newRole) => {
      role.value = newRole;
      await getPersonByRoleAndUserId();
    },
    { immediate: true } // Run immediately to catch the initial state
  );
  defineProps({});
  provide('getFullName', getFullName);
  </script>

<style scoped>
.navbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  background-color: #E6CCB2;
  color: #E6CCB2;
  border-bottom: #000;
}

.user-info {
  color: #000;
  font-size: 12px;
  margin-right:50px;
  width: 100px;
}

.user-icon {
  display: flex; /* Kuvab lapseelemendid horisontaalselt */
  align-items: center; /* Tsentreerib elemendid vertikaalselt */
  cursor: pointer; /* Käeikoon, mis näitab, et element on klikitav */
}

.user-name {
  width: 100px;
  text-align: right;
  margin-right: 8px; /* Ruumi tekitamine nime ja ikooni vahele */
  font-size: 12px; /* Võimalik kohandada teksti suurust */
}

.icon {
  width: 24px; /* Ikooni suurus */
  height: 24px;
  color: #000; /* Määra ikooni värv */
}

.user-menu {
  display: flex;
  align-items: center;
  cursor: pointer;
}

.logout-icon {
  align-items:normal;
  width: 20px;
  height: 20px;
}

.logout-item {
  display: flex; /* Kuvab ikooni ja teksti samal real */
  align-items: center; /* Tsentreerib vertikaalselt */
  gap: 0.1rem; /* Ruumi ikooni ja teksti vahel */
  padding: 10px 20px;
  cursor: pointer;
  transition: background-color 0.2s;
}

.dropdown-menu {
  position:absolute;
  top: 40px;
  right: 0;
  background-color: #fff;
  border: 1px solid #ccc;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  z-index: 1000;
  width: 120px;
}

.dropdown-menu ul {
  list-style: none;
  padding: 0;
  margin: 0;
}

.dropdown-menu li {
  padding: 10px 20px;
  cursor: pointer;
  transition: background-color 0.2s;
}

.dropdown-menu li:hover {
  background-color: #f0f0f0;
}
</style>

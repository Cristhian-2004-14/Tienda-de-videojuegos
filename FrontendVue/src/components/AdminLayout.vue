<script setup>
import { computed, ref, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useAuthStore } from '../stores/auth';
import { useNotificacionesStore } from '../stores/notificaciones';

// Componente reutilizable: sidebar + top bar compartidos por todas las
// vistas del panel administrativo (Dashboard, Productos, Clientes, etc.).
defineProps({
  titulo: { type: String, required: true },
  buscadorPlaceholder: { type: String, default: 'Buscar...' },
});

const route = useRoute();
const router = useRouter();
const authStore = useAuthStore();
const notificaciones = useNotificacionesStore();
const mostrarAviso = ref(false);
let temporizadorAviso;
watch(() => notificaciones.id, () => {
  mostrarAviso.value = true;
  clearTimeout(temporizadorAviso);
  temporizadorAviso = setTimeout(() => mostrarAviso.value = false, 3500);
});

const navItems = [
  { label: 'Dashboard', icon: 'dashboard', to: '/admin', permiso: 'dashboard' },
  { label: 'Clientes', icon: 'group', to: '/admin/clientes', permiso: 'clientes' },
  { label: 'Productos', icon: 'inventory_2', to: '/admin/productos', permiso: 'productos' },
  { label: 'Ventas', icon: 'payments', to: '/admin/ventas', permiso: 'ventas' },
  { label: 'Servicios', icon: 'settings_applications', to: '/admin/servicios', permiso: 'servicios' },
  { label: 'Dispositivos', icon: 'devices', to: '/admin/dispositivos', permiso: 'servicios' },
  { label: 'Compras', icon: 'local_shipping', to: '/admin/compras', permiso: 'compras' },
  { label: 'Proveedores', icon: 'warehouse', to: '/admin/proveedores', permiso: 'compras' },
  { label: 'Personal', icon: 'badge', to: '/admin/personal', permiso: 'personal' },
  { label: 'Reportes', icon: 'monitoring', to: '/admin/reportes', permiso: 'reportes' },
  { label: 'Roles', icon: 'admin_panel_settings', to: '/admin/roles', permiso: 'roles' },
];
const navPermitida = computed(() => navItems.filter(item => authStore.tienePermiso(item.permiso)));

function esActivo(to) {
  return route.path === to || (to !== '/admin' && route.path.startsWith(to));
}

function cerrarSesion() {
  authStore.cerrarSesion();
  router.push('/login');
}
</script>

<template>
  <div class="admin-shell">
    <aside class="sidebar">
      <div class="sidebar-brand">
        <h1>Kinetic Console</h1>
        <p class="mono">Admin Terminal</p>
      </div>

      <nav class="sidebar-nav">
        <router-link
          v-for="item in navPermitida"
          :key="item.to"
          :to="item.to"
          class="nav-item"
          :class="{ active: esActivo(item.to) }"
        >
          <span class="material-symbols-outlined">{{ item.icon }}</span>
          <span>{{ item.label }}</span>
        </router-link>
      </nav>

      <div class="sidebar-footer">
        <button class="nav-item nav-item-btn" @click="cerrarSesion">
          <span class="material-symbols-outlined">logout</span>
          <span>Cerrar sesión</span>
        </button>
      </div>
    </aside>

    <div class="admin-main">
      <header class="topbar">
        <div class="search-box">
          <span class="material-symbols-outlined">search</span>
          <input type="text" :placeholder="buscadorPlaceholder" />
        </div>
        <div class="topbar-actions">
          <div class="user-chip">
            <div class="avatar">
              {{ (authStore.usuarioActual?.username || 'AD').slice(0, 2).toUpperCase() }}
            </div>
            <span class="mono">{{ authStore.usuarioActual?.rol || 'Invitado' }}</span>
          </div>
        </div>
      </header>

      <div class="content">
        <slot name="header">
          <h2 class="page-title">{{ titulo }}</h2>
        </slot>
        <slot />
      </div>
      <transition name="aviso">
        <div v-if="mostrarAviso && notificaciones.mensaje" class="notificacion" :class="notificaciones.tipo" role="status" aria-live="polite">
          <span class="material-symbols-outlined">{{ notificaciones.tipo === 'error' ? 'error' : 'check_circle' }}</span>
          <strong>{{ notificaciones.mensaje }}</strong>
          <button aria-label="Cerrar aviso" @click="mostrarAviso=false">×</button>
        </div>
      </transition>
    </div>
  </div>
</template>

<style scoped>
.admin-shell {
  display: flex;
  min-height: 100%;
}

.sidebar {
  width: 260px;
  flex-shrink: 0;
  background: var(--color-surface-container-lowest);
  border-right: 1px solid var(--color-surface-container-low);
  display: flex;
  flex-direction: column;
  padding: var(--space-md) 0;
  position: fixed;
  top: 0;
  left: 0;
  height: 100vh;
}

.sidebar-brand {
  padding: 0 var(--space-md) var(--space-md);
}

.sidebar-brand h1 {
  font-size: 20px;
  font-weight: 700;
  color: var(--color-primary);
}

.sidebar-brand p {
  font-size: 12px;
  color: var(--color-on-surface-variant);
  opacity: 0.6;
}

.sidebar-nav {
  flex-grow: 1;
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.nav-item {
  display: flex;
  align-items: center;
  gap: var(--space-sm);
  padding: 12px var(--space-md);
  color: var(--color-on-surface-variant);
  font-weight: 500;
  border-left: 4px solid transparent;
  transition: background-color 0.2s, color 0.2s;
  background: none;
  border-top: none;
  border-right: none;
  border-bottom: none;
  width: 100%;
  text-align: left;
  font-size: 14px;
}

.nav-item:hover {
  background: var(--color-surface-container-high);
  color: var(--color-on-surface);
}

.nav-item.active {
  color: var(--color-primary);
  font-weight: 700;
  border-left-color: var(--color-primary);
  background: var(--color-surface-container);
}

.nav-item-btn {
  cursor: pointer;
}

.sidebar-footer {
  padding-top: var(--space-md);
  border-top: 1px solid var(--color-surface-container-high);
}

.admin-main {
  margin-left: 260px;
  flex-grow: 1;
  display: flex;
  flex-direction: column;
  min-height: 100vh;
}

.topbar {
  height: 64px;
  position: sticky;
  top: 0;
  z-index: 10;
  background: var(--color-surface-container-lowest);
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 var(--space-lg);
}

.search-box {
  display: flex;
  align-items: center;
  gap: var(--space-sm);
  background: var(--color-surface-container-low);
  border-radius: var(--radius);
  padding: 8px 16px;
  width: 320px;
  border: 1px solid rgba(255, 255, 255, 0.05);
}

.search-box .material-symbols-outlined {
  color: var(--color-on-surface-variant);
  font-size: 18px;
}

.search-box input {
  background: transparent;
  border: none;
  outline: none;
  color: var(--color-on-surface);
  width: 100%;
  font-size: 14px;
}

.topbar-actions {
  display: flex;
  align-items: center;
  gap: var(--space-md);
}

.icon-btn {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  border: none;
  background: transparent;
  color: var(--color-on-surface-variant);
  display: flex;
  align-items: center;
  justify-content: center;
  transition: background-color 0.2s;
}

.icon-btn:hover {
  background: var(--color-surface-container-high);
}

.user-chip {
  display: flex;
  align-items: center;
  gap: var(--space-sm);
  padding-left: var(--space-md);
  border-left: 1px solid var(--color-surface-container-high);
  font-size: 12px;
}

.avatar {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  background: var(--color-secondary-container);
  color: var(--color-on-surface);
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
  font-size: 12px;
}

.content {
  padding: var(--space-lg);
  max-width: 1440px;
  width: 100%;
  margin: 0 auto;
  display: flex;
  flex-direction: column;
  gap: var(--space-lg);
}

.page-title {
  font-size: 24px;
  font-weight: 700;
}
.notificacion{position:fixed;right:24px;bottom:24px;z-index:100;display:flex;align-items:center;gap:12px;max-width:430px;padding:15px 18px;background:#182519;border:1px solid #79dd68;border-radius:9px;box-shadow:0 16px 45px #0009}.notificacion span{color:#79dd68}.notificacion.error{background:#321819;border-color:#ffb4ab}.notificacion.error span{color:#ffb4ab}.notificacion button{margin-left:auto;border:0;background:transparent;color:inherit;font-size:20px}.aviso-enter-active,.aviso-leave-active{transition:.22s}.aviso-enter-from,.aviso-leave-to{opacity:0;transform:translateY(12px)}
</style>

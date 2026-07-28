import { createRouter, createWebHistory } from 'vue-router';
import { useAuthStore } from '../stores/auth';

const routes = [
  { path: '/', redirect: '/tienda' },
  {
    path: '/login',
    name: 'login',
    component: () => import('../views/LoginView.vue'),
  },

  // --- Panel administrativo ---
  {
    path: '/admin',
    name: 'admin-dashboard',
    component: () => import('../views/admin/DashboardView.vue'),
  },
  {
    path: '/admin/productos',
    name: 'admin-productos',
    component: () => import('../views/admin/ProductosListView.vue'),
  },
  {
    path: '/admin/productos/nuevo',
    name: 'admin-productos-nuevo',
    component: () => import('../views/admin/ProductoFormView.vue'),
  },
  {
    path: '/admin/productos/:id/editar',
    name: 'admin-productos-editar',
    component: () => import('../views/admin/ProductoFormView.vue'),
    props: true,
  },
  {
    path: '/admin/clientes',
    name: 'admin-clientes',
    component: () => import('../views/admin/ClientesListView.vue'),
  },
  {
    path: '/admin/clientes/nuevo',
    name: 'admin-clientes-nuevo',
    component: () => import('../views/admin/ClienteFormView.vue'),
  },
  {
    path: '/admin/clientes/:id/editar',
    name: 'admin-clientes-editar',
    component: () => import('../views/admin/ClienteFormView.vue'),
    props: true,
  },
  { path: '/admin/clientes/:id', name: 'admin-cliente-detalle', component: () => import('../views/admin/ClienteDetalleView.vue'), props: true },
  {
    path: '/admin/ventas/nueva',
    name: 'admin-ventas-nueva',
    component: () => import('../views/admin/VentaPosView.vue'),
  },
  { path: '/admin/ventas', name: 'admin-ventas', component: () => import('../views/admin/VentasListView.vue') },
  { path: '/admin/ventas/:id', name: 'admin-venta-detalle', component: () => import('../views/admin/VentaDetalleView.vue'), props: true },
  { path: '/admin/ventas/:id/pago', name: 'admin-pago', component: () => import('../views/admin/PagoView.vue'), props: true },
  { path: '/admin/compras', name: 'admin-compras', component: () => import('../views/admin/ComprasView.vue') },
  { path: '/admin/compras/:id', name: 'admin-compra-detalle', component: () => import('../views/admin/CompraDetalleView.vue'), props: true },
  { path: '/admin/proveedores', name: 'admin-proveedores', component: () => import('../views/admin/ProveedoresView.vue') },
  { path: '/admin/dispositivos', name: 'admin-dispositivos', component: () => import('../views/admin/DispositivosView.vue') },
  { path: '/admin/personal', name: 'admin-personal', component: () => import('../views/admin/PersonalView.vue') },
  { path: '/admin/roles', name: 'admin-roles', component: () => import('../views/admin/RolesView.vue') },
  { path: '/admin/reportes', name: 'admin-reportes', component: () => import('../views/admin/ReportesView.vue') },
  {
    path: '/admin/servicios',
    name: 'admin-servicios',
    component: () => import('../views/admin/ServiciosListView.vue'),
  },
  { path: '/admin/servicios/nuevo', name: 'admin-servicios-nuevo', component: () => import('../views/admin/ServicioFormView.vue') },
  {
    path: '/admin/servicios/:id',
    name: 'admin-servicios-detalle',
    component: () => import('../views/admin/ServicioDetalleView.vue'),
    props: true,
  },

  // --- Tienda online ---
  {
    path: '/tienda',
    name: 'tienda-catalogo',
    component: () => import('../views/tienda/CatalogoView.vue'),
  },
  {
    path: '/tienda/producto/:id',
    name: 'tienda-producto-detalle',
    component: () => import('../views/tienda/ProductoDetalleView.vue'),
    props: true,
  },
  {
    path: '/tienda/carrito',
    name: 'tienda-carrito',
    component: () => import('../views/tienda/CarritoView.vue'),
  },
  {
    path: '/tienda/login',
    name: 'tienda-login',
    component: () => import('../views/tienda/ClienteLoginView.vue'),
  },
  {
    path: '/tienda/servicio',
    name: 'tienda-servicio-consulta',
    component: () => import('../views/tienda/ConsultaServicioView.vue'),
  },
  {
    path: '/tienda/servicio/:id',
    name: 'tienda-servicio-seguimiento',
    component: () => import('../views/tienda/SeguimientoServicioView.vue'),
    props: true,
  },
  { path: '/acceso-denegado', name: 'acceso-denegado', component: () => import('../views/AccesoDenegadoView.vue') },
  { path: '/:pathMatch(.*)*', name: 'no-encontrado', component: () => import('../views/NotFoundView.vue') },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

const permisosPorModulo = {
  productos: 'productos', clientes: 'clientes', ventas: 'ventas', servicios: 'servicios',
  dispositivos: 'servicios', compras: 'compras', proveedores: 'compras', personal: 'personal',
  reportes: 'reportes', roles: 'roles',
};

router.beforeEach((to) => {
  const auth = useAuthStore();
  if (to.path === '/login' && auth.estaAutenticado) return '/admin';
  if (!to.path.startsWith('/admin')) return true;
  if (!auth.estaAutenticado) return { path: '/login', query: { redirect: to.fullPath } };
  const modulo = to.path.split('/')[2] || 'dashboard';
  const permiso = permisosPorModulo[modulo] || modulo;
  if (auth.tienePermiso(permiso)) return true;
  if (modulo === 'dashboard') {
    const destino = Object.entries(permisosPorModulo)
      .find(([, permisoModulo]) => auth.tienePermiso(permisoModulo))?.[0];
    return destino ? `/admin/${destino}` : '/acceso-denegado';
  }
  return '/acceso-denegado';
});

export default router;

import { createRouter, createWebHistory } from 'vue-router';

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
  {
    path: '/admin/ventas/nueva',
    name: 'admin-ventas-nueva',
    component: () => import('../views/admin/VentaPosView.vue'),
  },
  {
    path: '/admin/servicios',
    name: 'admin-servicios',
    component: () => import('../views/admin/ServiciosListView.vue'),
  },
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
    path: '/tienda/servicio/:id',
    name: 'tienda-servicio-seguimiento',
    component: () => import('../views/tienda/SeguimientoServicioView.vue'),
    props: true,
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;

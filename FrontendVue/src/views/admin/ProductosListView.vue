<script setup>
import { computed, onMounted, ref } from 'vue';
import { storeToRefs } from 'pinia';
import AdminLayout from '../../components/AdminLayout.vue';
// Patrón Repository: los productos se obtienen de src/data/productos.js;
// cuando exista backend, solo se reemplaza este import por una llamada a la API.
import { useDatosApiStore } from '../../stores/datosApi';
import { productosApi } from '../../services/recursosApi';

const filtroTexto = ref('');
const datosStore = useDatosApiStore();
const { productos } = storeToRefs(datosStore);
onMounted(() => datosStore.cargarRecurso('productos', productosApi));

const productosFiltrados = computed(() => {
  const texto = filtroTexto.value.trim().toLowerCase();
  if (!texto) return productos.value;
  return productos.value.filter(
    (p) =>
      p.nombre.toLowerCase().includes(texto) ||
      p.categoria.toLowerCase().includes(texto) ||
      p.marca.toLowerCase().includes(texto)
  );
});

function estadoStock(stock) {
  if (stock === 0) return { texto: 'Sin stock', clase: 'sin-stock' };
  if (stock <= 5) return { texto: `Stock bajo (${stock})`, clase: 'stock-bajo' };
  return { texto: `En stock (${stock})`, clase: 'en-stock' };
}
async function darDeBaja(producto) {
  if (!producto.activo || !window.confirm(`¿Dar de baja ${producto.nombre}?`)) return;
  await productosApi.eliminar(producto.id);
  await datosStore.cargarRecurso('productos', productosApi);
}
</script>

<template>
  <AdminLayout titulo="Productos">
    <template #header>
      <div class="page-header">
        <div>
          <h2 class="page-title">Productos</h2>
          <p class="page-subtitle">
            Administra tu inventario, precios y niveles de stock.
          </p>
        </div>
        <div class="page-actions">
          <router-link to="/admin/productos/nuevo" class="btn-primary">
            <span class="material-symbols-outlined">add</span>
            Agregar producto
          </router-link>
        </div>
      </div>
    </template>

    <div class="table-toolbar">
      <div class="search-box">
        <span class="material-symbols-outlined">search</span>
        <input
          v-model="filtroTexto"
          type="text"
          placeholder="Buscar por nombre, categoría o marca..."
        />
      </div>
    </div>

    <div class="table-card">
      <table>
        <thead>
          <tr>
            <th>Producto</th>
            <th>Categoría</th>
            <th>Marca</th>
            <th>Precio</th>
            <th>Stock</th>
            <th class="col-acciones">Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="producto in productosFiltrados" :key="producto.id">
            <td>
              <div class="producto-cell">
                <div class="producto-thumb">
                  <span class="material-symbols-outlined">sports_esports</span>
                </div>
                <div>
                  <p class="producto-nombre">{{ producto.nombre }}</p>
                  <p class="producto-id mono">ID-{{ String(producto.id).padStart(3, '0') }}</p>
                </div>
              </div>
            </td>
            <td><span class="chip-categoria">{{ producto.categoria }}</span></td>
            <td>{{ producto.marca }}</td>
            <td class="precio">${{ producto.precioVenta.toFixed(2) }}</td>
            <td>
              <div class="stock-indicator" :class="estadoStock(producto.stock).clase">
                <span class="dot"></span>
                {{ estadoStock(producto.stock).texto }}
              </div>
            </td>
            <td class="col-acciones">
              <div class="acciones">
                <router-link :to="`/admin/productos/${producto.id}/editar`" class="icon-btn">
                  <span class="material-symbols-outlined">edit</span>
                </router-link>
                <button class="icon-btn icon-btn-danger" :disabled="!producto.activo" :title="producto.activo?'Dar de baja':'Producto inactivo'" @click="darDeBaja(producto)">
                  <span class="material-symbols-outlined">{{producto.activo?'visibility_off':'block'}}</span>
                </button>
              </div>
            </td>
          </tr>
          <tr v-if="productosFiltrados.length === 0">
            <td colspan="6" class="sin-resultados">No se encontraron productos.</td>
          </tr>
        </tbody>
      </table>

      <div class="table-footer">
        <p>
          Mostrando <strong>{{ productosFiltrados.length }}</strong> de
          <strong>{{ productos.length }}</strong> productos
        </p>
      </div>
    </div>
  </AdminLayout>
</template>

<style scoped>
.page-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-end;
  flex-wrap: wrap;
  gap: var(--space-md);
}

.page-title {
  font-size: 28px;
  font-weight: 800;
  letter-spacing: -0.01em;
}

.page-subtitle {
  color: var(--color-on-surface-variant);
  font-size: 14px;
  margin-top: 4px;
}

.page-actions {
  display: flex;
  gap: var(--space-sm);
}

.btn-secondary,
.btn-primary {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: 10px var(--space-md);
  border-radius: var(--radius);
  font-size: 14px;
  font-weight: 600;
  border: none;
  transition: opacity 0.2s, background-color 0.2s;
}

.btn-secondary {
  background: var(--color-surface-container-high);
  color: var(--color-on-surface);
}

.btn-secondary:hover {
  background: var(--color-surface-container-highest);
}

.btn-primary {
  background: var(--color-primary-container);
  color: #fff;
}

.btn-primary:hover {
  opacity: 0.9;
}

.table-toolbar {
  display: flex;
}

.search-box {
  display: flex;
  align-items: center;
  gap: var(--space-sm);
  background: var(--color-surface-container-low);
  border-radius: var(--radius);
  padding: 8px 16px;
  width: 100%;
  max-width: 420px;
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

.table-card {
  background: var(--color-surface-container-low);
  border-radius: var(--radius-md);
  overflow: hidden;
  border: 1px solid rgba(255, 255, 255, 0.05);
}

table {
  width: 100%;
  border-collapse: collapse;
  text-align: left;
}

thead {
  background: rgba(255, 255, 255, 0.03);
}

th {
  padding: var(--space-sm) var(--space-md);
  font-size: 11px;
  font-weight: 700;
  letter-spacing: 0.05em;
  text-transform: uppercase;
  color: var(--color-on-surface-variant);
  opacity: 0.8;
}

td {
  padding: var(--space-sm) var(--space-md);
  border-top: 1px solid var(--color-surface-container-high);
  font-size: 14px;
}

tbody tr:hover {
  background: var(--color-surface-container);
}

.producto-cell {
  display: flex;
  align-items: center;
  gap: var(--space-md);
}

.producto-thumb {
  width: 48px;
  height: 48px;
  border-radius: var(--radius);
  background: var(--color-surface-container-high);
  color: var(--color-on-surface-variant);
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.producto-nombre {
  font-weight: 700;
}

.producto-id {
  font-size: 11px;
  color: var(--color-on-surface-variant);
  text-transform: uppercase;
}

.chip-categoria {
  background: var(--color-surface-container-high);
  color: var(--color-secondary);
  font-size: 11px;
  font-weight: 700;
  padding: 4px 8px;
  border-radius: var(--radius-sm);
}

.precio {
  font-weight: 700;
}

.stock-indicator {
  display: flex;
  align-items: center;
  gap: 8px;
  font-weight: 500;
}

.stock-indicator .dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
}

.stock-indicator.en-stock {
  color: var(--color-primary);
}
.stock-indicator.en-stock .dot {
  background: var(--color-primary);
}

.stock-indicator.stock-bajo {
  color: var(--color-tertiary);
}
.stock-indicator.stock-bajo .dot {
  background: var(--color-tertiary);
}

.stock-indicator.sin-stock {
  color: var(--color-error);
}
.stock-indicator.sin-stock .dot {
  background: var(--color-error);
}

.col-acciones {
  text-align: right;
}

.acciones {
  display: flex;
  justify-content: flex-end;
  gap: 8px;
}

.icon-btn {
  width: 36px;
  height: 36px;
  border-radius: var(--radius);
  border: none;
  background: transparent;
  color: var(--color-on-surface-variant);
  display: flex;
  align-items: center;
  justify-content: center;
  transition: background-color 0.2s, color 0.2s;
}

.icon-btn:hover {
  background: rgba(121, 221, 104, 0.1);
  color: var(--color-primary);
}

.icon-btn-danger:hover {
  background: rgba(255, 180, 171, 0.1);
  color: var(--color-error);
}

.sin-resultados {
  text-align: center;
  color: var(--color-on-surface-variant);
  padding: var(--space-lg);
}

.table-footer {
  padding: var(--space-sm) var(--space-md);
  border-top: 1px solid var(--color-surface-container-high);
  color: var(--color-on-surface-variant);
  font-size: 14px;
}

.table-footer strong {
  color: var(--color-on-surface);
}
</style>

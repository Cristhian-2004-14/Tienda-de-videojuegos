<script setup>
import { computed, onMounted } from 'vue';
import { storeToRefs } from 'pinia';
import AdminLayout from '../../components/AdminLayout.vue';
import StatCard from '../../components/StatCard.vue';
// Patrón Repository: esta vista no sabe de dónde vienen los datos, solo los
// importa desde la capa src/data/ — el día que haya backend, solo esa capa
// cambia (por fetch a la API) y esta vista sigue funcionando igual.
import { useDatosApiStore } from '../../stores/datosApi';

const datosStore = useDatosApiStore();
const { ventas, servicios, productos, clientes } = storeToRefs(datosStore);
onMounted(() => datosStore.cargarTodo());
const totalVentas = computed(() =>
  ventas.value.reduce((acc, v) => acc + v.total, 0)
);

const productosEnStock = computed(() =>
  productos.value.reduce((acc, p) => acc + p.stock, 0)
);

const serviciosPendientes = computed(
  () => servicios.value.filter((s) => s.estado !== 'Completado').length
);

function formatoMoneda(valor) {
  return `$${valor.toFixed(2)}`;
}
</script>

<template>
  <AdminLayout titulo="Overview" buscadorPlaceholder="Buscar órdenes, tickets...">
    <section class="stats-grid">
      <StatCard
        etiqueta="Ventas totales"
        :valor="formatoMoneda(totalVentas)"
        icono="payments"
        nota="+12.5% vs mes anterior"
      />
      <StatCard
        etiqueta="Clientes registrados"
        :valor="clientes.length"
        icono="group"
        nota="+3.2% desde ayer"
      />
      <StatCard
        etiqueta="Productos en stock"
        :valor="productosEnStock"
        icono="inventory_2"
        nota="Capacidad de inventario"
        notaTipo="neutra"
      />
      <StatCard
        etiqueta="Servicios pendientes"
        :valor="serviciosPendientes"
        icono="settings_applications"
        nota="Requieren seguimiento"
        notaTipo="negativa"
      />
    </section>

    <section class="detail-grid">
      <div class="panel">
        <div class="panel-header">
          <h3>Últimas ventas</h3>
          <router-link to="/admin/ventas" class="ver-todo">Ver todas</router-link>
        </div>
        <div class="panel-list">
          <div v-for="venta in ventas" :key="venta.id" class="panel-row">
            <div class="row-left">
              <div class="row-avatar">
                <span class="material-symbols-outlined">person</span>
              </div>
              <div class="row-info">
                <span class="row-title">{{ venta.cliente }}</span>
                <span class="row-subtitle">{{ venta.fecha }} · {{ venta.estado }}</span>
              </div>
            </div>
            <span class="row-value mono">{{ formatoMoneda(venta.total) }}</span>
          </div>
        </div>
      </div>

      <div class="panel">
        <div class="panel-header">
          <h3>Servicios en proceso</h3>
          <router-link to="/admin/servicios" class="ver-todo">Gestionar</router-link>
        </div>
        <div class="panel-list">
          <div v-for="servicio in servicios" :key="servicio.id" class="panel-row">
            <div class="row-info">
              <span class="row-title mono">#SRV-{{ String(servicio.id).padStart(4, '0') }}</span>
              <span class="row-subtitle">{{ servicio.diagnostico }}</span>
            </div>
            <div class="row-right">
              <span class="chip">{{ servicio.estado }}</span>
              <span class="row-subtitle">{{ servicio.dispositivo }}</span>
            </div>
          </div>
        </div>
      </div>
    </section>
  </AdminLayout>
</template>

<style scoped>
.stats-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: var(--space-md);
}

@media (max-width: 1024px) {
  .stats-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

.detail-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: var(--space-lg);
}

@media (max-width: 900px) {
  .detail-grid {
    grid-template-columns: 1fr;
  }
}

.panel {
  background: var(--color-surface-container-low);
  border-radius: var(--radius);
  overflow: hidden;
}

.panel-header {
  padding: var(--space-md);
  border-bottom: 1px solid var(--color-surface-container-high);
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.panel-header h3 {
  font-size: 16px;
  font-weight: 700;
}

.ver-todo {
  color: var(--color-primary);
  font-size: 12px;
  font-weight: 700;
}

.ver-todo:hover {
  text-decoration: underline;
}

.panel-list {
  display: flex;
  flex-direction: column;
}

.panel-row {
  padding: var(--space-sm) var(--space-md);
  display: flex;
  align-items: center;
  justify-content: space-between;
  border-bottom: 1px solid var(--color-surface-container-high);
  transition: background-color 0.2s;
}

.panel-row:last-child {
  border-bottom: none;
}

.panel-row:hover {
  background: var(--color-surface-container);
}

.row-left {
  display: flex;
  align-items: center;
  gap: var(--space-md);
}

.row-avatar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  background: var(--color-surface-container-high);
  color: var(--color-on-surface-variant);
  display: flex;
  align-items: center;
  justify-content: center;
}

.row-info {
  display: flex;
  flex-direction: column;
}

.row-right {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: 4px;
}

.row-title {
  font-weight: 600;
  font-size: 14px;
}

.row-subtitle {
  font-size: 12px;
  color: var(--color-on-surface-variant);
}

.row-value {
  color: var(--color-primary);
  font-weight: 700;
}

.chip {
  font-size: 10px;
  font-weight: 700;
  text-transform: uppercase;
  color: var(--color-primary);
  background: rgba(121, 221, 104, 0.1);
  padding: 2px 8px;
  border-radius: var(--radius-sm);
}
</style>

<script setup>
import { computed, onMounted } from 'vue';
import { storeToRefs } from 'pinia';
import AdminLayout from '../../components/AdminLayout.vue';
import { useDatosApiStore } from '../../stores/datosApi';
import { ventasApi } from '../../services/recursosApi';

const datosStore = useDatosApiStore();
const { ventas } = storeToRefs(datosStore);
const pendientes = computed(() => ventas.value.filter((venta) => venta.estado === 'Pendiente'));
onMounted(() => datosStore.cargarRecurso('ventas', ventasApi));
</script>

<template>
  <AdminLayout titulo="Ventas" buscador-placeholder="Buscar por cliente o número...">
    <template #header><div class="modulo-header"><div><p class="eyebrow mono">COMERCIAL / VENTAS</p><h2>Historial de ventas</h2><p>Registra ventas y deja constancia de cómo fueron canceladas.</p></div><router-link class="btn-primary" to="/admin/ventas/nueva">Nueva venta</router-link></div></template>
    <section class="resumen-casos"><article><small>PENDIENTES DE PAGO</small><strong>{{ pendientes.length }}</strong></article><article><small>VENTAS REGISTRADAS</small><strong>{{ ventas.length }}</strong></article></section>
    <section class="panel-caso">
      <table class="tabla-caso"><thead><tr><th>Venta</th><th>Cliente</th><th>Fecha</th><th>Total</th><th>Estado</th><th>Acción</th></tr></thead>
      <tbody><tr v-for="venta in ventas" :key="venta.id"><td><router-link class="accion-caso mono" :to="`/admin/ventas/${venta.id}`">#V-{{ String(venta.id).padStart(4,'0') }}</router-link></td><td>{{ venta.cliente }}</td><td>{{ new Date(venta.fecha).toLocaleString('es-BO') }}</td><td>${{ Number(venta.total).toFixed(2) }}</td><td><span class="estado-caso">{{ venta.estado }}</span></td><td><div class="acciones-fila"><router-link class="accion-caso" :to="`/admin/ventas/${venta.id}`">Ver detalle</router-link><router-link v-if="venta.estado === 'Pendiente'" class="accion-caso" :to="`/admin/ventas/${venta.id}/pago`">Registrar pago</router-link></div></td></tr><tr v-if="!ventas.length"><td colspan="6" class="vacio">Todavía no hay ventas registradas.</td></tr></tbody></table>
    </section>
  </AdminLayout>
</template>

<style scoped>
.acciones-fila{display:flex;gap:14px;flex-wrap:wrap}.vacio{text-align:center!important;padding:38px!important;color:#899287!important}
</style>

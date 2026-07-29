<script setup>
import { computed, onMounted, ref } from 'vue';
import { storeToRefs } from 'pinia';
import AdminLayout from '../../components/AdminLayout.vue';
import AdminPageHeader from '../../components/common/AdminPageHeader.vue';
import DataTable from '../../components/common/DataTable.vue';
import StatusBadge from '../../components/common/StatusBadge.vue';
import StatCard from '../../components/StatCard.vue';
import PaginacionRegistros from '../../components/common/PaginacionRegistros.vue';
import { formatearCodigo, formatearDinero, formatearFechaHora } from '../../composables/useFormatters';
import { useDatosApiStore } from '../../stores/datosApi';
import { ventasApi } from '../../services/recursosApi';

const store = useDatosApiStore();
const { ventas } = storeToRefs(store);
const pendientes = computed(() => ventas.value.filter(venta => venta.estado === 'Pendiente'));
const pagina = ref(1);
const porPagina = 10;
const ventasPagina = computed(() => ventas.value.slice(
  (pagina.value - 1) * porPagina,
  pagina.value * porPagina,
));
onMounted(() => store.cargarRecurso('ventas', ventasApi));
</script>

<template>
  <AdminLayout titulo="Ventas">
    <template #header>
      <AdminPageHeader eyebrow="COMERCIAL / VENTAS" title="Historial de ventas" description="Registra ventas y deja constancia de cómo fueron canceladas.">
        <router-link class="btn-primary" to="/admin/ventas/nueva">Nueva venta</router-link>
      </AdminPageHeader>
    </template>
    <section class="stats">
      <StatCard etiqueta="Pendientes de pago" :valor="pendientes.length" icono="pending_actions" />
      <StatCard etiqueta="Ventas registradas" :valor="ventas.length" icono="receipt_long" />
    </section>
    <section class="panel-caso">
      <DataTable :empty="!ventas.length" empty-text="Todavía no hay ventas registradas" :columns="6">
        <template #header><thead><tr><th>Venta</th><th>Cliente</th><th>Fecha</th><th>Total</th><th>Estado</th><th>Acción</th></tr></thead></template>
        <tr v-for="venta in ventasPagina" :key="venta.id">
          <td><router-link class="accion-caso mono" :to="`/admin/ventas/${venta.id}`">{{formatearCodigo('V',venta.id)}}</router-link></td>
          <td>{{venta.cliente}}</td><td>{{formatearFechaHora(venta.fecha)}}</td><td>{{formatearDinero(venta.total)}}</td>
          <td><StatusBadge :status="venta.estado"/></td>
          <td><div class="actions"><router-link class="accion-caso" :to="`/admin/ventas/${venta.id}`">Ver detalle</router-link><router-link v-if="venta.estado==='Pendiente'" class="accion-caso" :to="`/admin/ventas/${venta.id}/pago`">Registrar pago</router-link></div></td>
        </tr>
      </DataTable>
      <PaginacionRegistros v-model:pagina="pagina" :total="ventas.length" :por-pagina="porPagina" />
    </section>
  </AdminLayout>
</template>

<style scoped>
.stats{display:grid;grid-template-columns:repeat(2,minmax(0,1fr));gap:16px}.actions{display:flex;gap:14px;flex-wrap:wrap}@media(max-width:600px){.stats{grid-template-columns:1fr}}
</style>

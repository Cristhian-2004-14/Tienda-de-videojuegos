<script setup>
import { computed, onMounted, ref } from 'vue';
import { storeToRefs } from 'pinia';
import AdminLayout from '../../components/AdminLayout.vue';
import AdminPageHeader from '../../components/common/AdminPageHeader.vue';
import DataTable from '../../components/common/DataTable.vue';
import StatusBadge from '../../components/common/StatusBadge.vue';
import StatCard from '../../components/StatCard.vue';
import { formatearCodigo, formatearFecha } from '../../composables/useFormatters';
import { serviciosApi } from '../../services/recursosApi';
import { useDatosApiStore } from '../../stores/datosApi';

const store=useDatosApiStore(),filtro=ref('Todos');
const {servicios}=storeToRefs(store);
const activos=computed(()=>servicios.value.filter(s=>!['Entregado','Cancelado'].includes(s.estado)));
const reparacion=computed(()=>servicios.value.filter(s=>s.estado==='En reparación'));
const listos=computed(()=>servicios.value.filter(s=>s.estado==='Listo para entrega'));
const filtrados=computed(()=>filtro.value==='Todos'?servicios.value:servicios.value.filter(s=>s.estado===filtro.value));
const estados=['Todos','Recibido','En diagnóstico','En reparación','En pruebas','Listo para entrega','Entregado'];
onMounted(()=>store.cargarRecurso('servicios',serviciosApi));
</script>

<template>
  <AdminLayout titulo="Servicio técnico">
    <template #header><AdminPageHeader eyebrow="OPERACIONES / TALLER" title="Órdenes de servicio" description="Recepción, diagnóstico, reparación, pruebas y entrega."><router-link class="btn-primary" to="/admin/servicios/nuevo"><span class="material-symbols-outlined">add</span>Nueva orden</router-link></AdminPageHeader></template>
    <section class="stats"><StatCard etiqueta="Servicios activos" :valor="activos.length" icono="pending_actions"/><StatCard etiqueta="En reparación" :valor="reparacion.length" icono="handyman"/><StatCard etiqueta="Listos para entrega" :valor="listos.length" icono="verified"/></section>
    <section class="panel-caso">
      <div class="filters"><button v-for="estado in estados" :key="estado" :class="{active:filtro===estado}" @click="filtro=estado">{{estado}}</button></div>
      <DataTable :empty="!filtrados.length" empty-text="No hay servicios para este filtro" :columns="6" min-width="850px">
        <template #header><thead><tr><th>Orden</th><th>Cliente</th><th>Dispositivo</th><th>Diagnóstico</th><th>Estado</th><th></th></tr></thead></template>
        <tr v-for="servicio in filtrados" :key="servicio.id"><td class="mono code">{{formatearCodigo('SRV',servicio.id)}}</td><td><strong>{{servicio.cliente}}</strong><small>{{formatearFecha(servicio.fechaIngreso)}}</small></td><td>{{servicio.dispositivo}}</td><td>{{servicio.diagnostico||'Pendiente de diagnóstico'}}</td><td><StatusBadge :status="servicio.estado"/></td><td><router-link class="accion-caso" :to="`/admin/servicios/${servicio.id}`">Abrir</router-link></td></tr>
      </DataTable>
    </section>
  </AdminLayout>
</template>

<style scoped>
.stats{display:grid;grid-template-columns:repeat(3,1fr);gap:16px}.filters{display:flex;gap:7px;overflow:auto;padding-bottom:18px}.filters button{white-space:nowrap;padding:8px 11px;border:1px solid #363636;background:#171717;color:#aaa;border-radius:6px}.filters .active{background:#107c10;border-color:#107c10;color:#fff}.code{color:#79dd68}.tabla-caso small{display:block;color:#818981;margin-top:4px}@media(max-width:750px){.stats{grid-template-columns:1fr}}
</style>

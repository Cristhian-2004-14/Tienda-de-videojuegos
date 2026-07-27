<script setup>
import { onMounted } from 'vue';
import { storeToRefs } from 'pinia';
import AdminLayout from '../../components/AdminLayout.vue';
import { useDatosApiStore } from '../../stores/datosApi';
import { serviciosApi } from '../../services/recursosApi';

const datosStore = useDatosApiStore();
const { servicios } = storeToRefs(datosStore);
onMounted(() => datosStore.cargarRecurso('servicios', serviciosApi));

const clasesEstado = { 'En proceso': 'verde', 'En pruebas': 'azul', 'Esperando repuestos': 'naranja', Asignado: 'gris', Pendiente: 'gris' };
</script>

<template>
  <AdminLayout titulo="Servicio técnico" buscador-placeholder="Buscar tickets de servicio...">
    <template #header><div class="header"><div><p class="eyebrow mono">OPERACIONES / TALLER</p><h2>Panel de servicio</h2><p>Supervisa reparaciones, diagnósticos y entregas.</p></div><button class="btn-primary"><span class="material-symbols-outlined">add</span>Nuevo servicio</button></div></template>
    <section class="stats">
      <article><span class="material-symbols-outlined">pending_actions</span><div><small>Servicios activos</small><strong>12</strong></div></article>
      <article><span class="material-symbols-outlined">handyman</span><div><small>En reparación</small><strong>5</strong></div></article>
      <article><span class="material-symbols-outlined">verified</span><div><small>Listos para entrega</small><strong>3</strong></div></article>
    </section>
    <section class="panel">
      <div class="panel-top"><h3>Servicios activos</h3><div><button class="filtro activo">Todos</button><button class="filtro">Pendientes</button><button class="filtro">En proceso</button></div></div>
      <div class="tabla-wrap"><table><thead><tr><th>Ticket</th><th>Cliente</th><th>Dispositivo</th><th>Diagnóstico</th><th>Estado</th><th></th></tr></thead>
      <tbody><tr v-for="servicio in servicios" :key="servicio.id"><td class="mono ticket">#SRV-{{ 90209 + servicio.id }}</td><td><strong>{{ servicio.cliente }}</strong><small>Recibido hace {{ servicio.id + 1 }} días</small></td><td>{{ servicio.dispositivo }}</td><td>{{ servicio.diagnostico }}</td><td><span class="estado" :class="clasesEstado[servicio.estado]">{{ servicio.estado }}</span></td><td><router-link :to="`/admin/servicios/${servicio.id}`" class="ver"><span class="material-symbols-outlined">arrow_forward</span></router-link></td></tr></tbody></table></div>
    </section>
  </AdminLayout>
</template>

<style scoped>
.header{display:flex;justify-content:space-between;align-items:end}.header h2{font-size:32px}.header p:last-child{color:#9ba497;margin-top:8px}.eyebrow{font-size:11px;color:#79dd68!important;margin:0 0 8px!important}.btn-primary{display:flex;align-items:center;gap:8px}.stats{display:grid;grid-template-columns:repeat(3,1fr);gap:16px}.stats article{background:#1b1b1b;border-radius:8px;padding:22px;display:flex;align-items:center;gap:16px}.stats .material-symbols-outlined{width:44px;height:44px;display:grid;place-items:center;background:#0f3812;color:#79dd68;border-radius:6px}.stats small{display:block;color:#9ba497}.stats strong{display:block;font-size:27px;margin-top:3px}.panel{background:#1a1a1a;border-radius:8px;overflow:hidden}.panel-top{display:flex;justify-content:space-between;align-items:center;padding:22px 24px;border-bottom:1px solid #303030}.panel-top h3{font-size:19px}.filtro{border:0;background:transparent;color:#949494;padding:8px 12px}.filtro.activo{background:#303030;color:#fff;border-radius:5px}.tabla-wrap{overflow:auto}table{border-collapse:collapse;width:100%;min-width:900px}th{text-align:left;padding:13px 20px;color:#899482;font-size:10px;text-transform:uppercase;letter-spacing:.08em;background:#141414}td{padding:18px 20px;border-bottom:1px solid #2a2a2a;color:#c8c8c8;font-size:13px}td small{display:block;color:#7d7d7d;margin-top:5px}.ticket{color:#79dd68}.estado{padding:6px 9px;border-radius:4px;font-size:11px;font-weight:700}.verde{background:#123b17;color:#79dd68}.azul{background:#18334b;color:#7fc4ff}.naranja{background:#493015;color:#ffc36c}.gris{background:#333;color:#ccc}.ver{color:#79dd68}
@media(max-width:800px){.stats{grid-template-columns:1fr}.header{align-items:start;flex-direction:column;gap:20px}}
</style>

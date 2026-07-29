<script setup>
import { computed, onMounted, ref } from 'vue';
import { storeToRefs } from 'pinia';
import AdminLayout from '../../components/AdminLayout.vue';
import AdminPageHeader from '../../components/common/AdminPageHeader.vue';
import { useDatosApiStore } from '../../stores/datosApi';
import { clientesApi } from '../../services/recursosApi';

// Patrón Repository: los clientes se consumen desde la capa de datos mock.
const busqueda = ref('');
const datosStore = useDatosApiStore();
const { clientes: listado } = storeToRefs(datosStore);
onMounted(() => datosStore.cargarRecurso('clientes', clientesApi));
const filtrados = computed(() => listado.value.filter((cliente) => `${cliente.nombre} ${cliente.apellido} ${cliente.email}`.toLowerCase().includes(busqueda.value.toLowerCase())));
async function eliminar(id) {
  if (window.confirm('¿Eliminar este cliente?')) {
    await datosStore.eliminarCliente(id);
  }
}
</script>

<template>
  <AdminLayout titulo="Clientes" buscador-placeholder="Buscar clientes...">
    <template #header><AdminPageHeader title="Clientes" description="Administra y consulta tu base de clientes."><router-link class="btn-primary" to="/admin/clientes/nuevo"><span class="material-symbols-outlined">person_add</span>Agregar cliente</router-link></AdminPageHeader></template>
    <div class="stats"><article><small>Total clientes</small><strong>{{ listado.length }}</strong><span>↑ Datos de ejemplo</span></article><article><small>Con correo</small><strong>{{ listado.filter(c=>c.email).length }}</strong><span>Contactables</span></article><article><small>Nuevos este mes</small><strong>2</strong><span>Actividad reciente</span></article></div>
    <div class="search-local"><span class="material-symbols-outlined">search</span><input v-model="busqueda" placeholder="Filtrar por nombre o correo..." /></div>
    <section class="tabla-wrap"><table><thead><tr><th>Cliente</th><th>Teléfono</th><th>Correo electrónico</th><th>ID</th><th>Acciones</th></tr></thead><tbody><tr v-for="cliente in filtrados" :key="cliente.id"><td><div class="cliente"><span>{{ cliente.nombre[0] }}{{ cliente.apellido[0] }}</span><strong>{{ cliente.nombre }} {{ cliente.apellido }}</strong></div></td><td class="mono">{{ cliente.telefono }}</td><td>{{ cliente.email }}</td><td class="mono">CLI-{{ String(cliente.id).padStart(4,'0') }}</td><td><router-link :to="`/admin/clientes/${cliente.id}/editar`" class="icon"><span class="material-symbols-outlined">edit</span></router-link><button class="icon" @click="eliminar(cliente.id)"><span class="material-symbols-outlined">delete</span></button></td></tr></tbody></table><div v-if="!filtrados.length" class="vacio">No se encontraron clientes.</div></section>
  </AdminLayout>
</template>

<style scoped>
.stats{display:grid;grid-template-columns:repeat(3,1fr);gap:22px}.stats article{padding:25px;background:#1b1b1b;border-radius:10px}.stats small{display:block;color:#c7d0c2;font-weight:700}.stats strong{display:block;font-size:35px;margin:10px 0;color:var(--color-primary)}.stats span{font-size:12px;color:#8d9988}.search-local{display:flex;gap:10px;align-items:center;background:#171717;border:1px solid #303030;border-radius:8px;padding:11px 14px;max-width:420px}.search-local input{border:0;outline:0;background:transparent;color:white;width:100%}.tabla-wrap{overflow:auto;background:#191919;border:1px solid #292929;border-radius:10px}table{width:100%;border-collapse:collapse;min-width:800px}th{text-align:left;background:#111;padding:17px 20px;color:#bfc8bb;text-transform:uppercase;font-size:11px;letter-spacing:.06em}td{padding:19px 20px;border-bottom:1px solid #252525;color:#d5d5d5}.cliente{display:flex;align-items:center;gap:12px}.cliente>span{width:38px;height:38px;display:grid;place-items:center;background:#263226;color:var(--color-primary);border-radius:8px;font-weight:800}.icon{border:0;background:none;color:#cbd4c8;padding:6px}.icon:hover{color:var(--color-primary)}.vacio{padding:40px;text-align:center;color:#999}@media(max-width:800px){.stats{grid-template-columns:1fr}}
</style>

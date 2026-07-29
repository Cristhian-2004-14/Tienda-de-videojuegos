<script setup>
import { onMounted, ref } from 'vue';
import AdminLayout from '../../components/AdminLayout.vue';
import DataTable from '../../components/common/DataTable.vue';
import StatusBadge from '../../components/common/StatusBadge.vue';
import { comprasApi } from '../../services/recursosApi';
const props=defineProps({id:{type:String,required:true}});
const compra=ref(null), error=ref('');
onMounted(async()=>{try{compra.value=await comprasApi.obtenerPorId(props.id)}catch{error.value='No se pudo cargar la compra.'}});
</script>
<template>
  <AdminLayout titulo="Detalle de compra">
    <p v-if="error" class="error">{{error}}</p>
    <div v-else-if="compra" class="detalle">
      <section class="panel-caso cabecera"><div><p class="eyebrow mono">COMPRA #C-{{compra.id}}</p><h2>{{compra.proveedor}}</h2><p>{{new Date(compra.fecha).toLocaleString('es-BO')}} · {{compra.empleado||'Sin empleado asignado'}}</p></div><StatusBadge :status="compra.estado"/></section>
      <section class="panel-caso"><DataTable :empty="!compra.detalles?.length" empty-text="Esta compra no tiene productos" :columns="4"><template #header><thead><tr><th>Producto</th><th>Cantidad</th><th>Costo unitario</th><th>Subtotal</th></tr></thead></template><tr v-for="d in compra.detalles" :key="d.productoId"><td>{{d.producto}}</td><td>{{d.cantidad}}</td><td>${{Number(d.precioUnitario).toFixed(2)}}</td><td>${{Number(d.subtotal).toFixed(2)}}</td></tr><template #footer><tfoot v-if="compra.detalles?.length"><tr><th colspan="3">Total</th><th>${{Number(compra.total).toFixed(2)}}</th></tr></tfoot></template></DataTable></section>
      <router-link class="btn-secondary volver" to="/admin/compras">Volver a compras</router-link>
    </div>
    <p v-else>Cargando compra...</p>
  </AdminLayout>
</template>
<style scoped>
.detalle{display:grid;gap:20px}.cabecera{display:flex;justify-content:space-between;align-items:center}.cabecera h2{font-size:30px;margin:7px 0}.cabecera p:last-child{color:#929a90}.volver{justify-self:start}.error{color:#ffb4ab}
</style>

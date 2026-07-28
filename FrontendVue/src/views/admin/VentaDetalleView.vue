<script setup>
import { computed, onMounted, ref } from 'vue';
import { useRouter } from 'vue-router';
import AdminLayout from '../../components/AdminLayout.vue';
import { ventasApi, anularVentaApi } from '../../services/recursosApi';
const props=defineProps({id:{type:String,required:true}}), router=useRouter();
const venta=ref(null), cargando=ref(true), error=ref('');
const pagado=computed(()=>venta.value?.pagos?.reduce((s,p)=>s+p.monto,0)||0);
const saldo=computed(()=>Math.max(0,(venta.value?.total||0)-pagado.value));
async function cargar(){try{venta.value=await ventasApi.obtenerPorId(props.id)}catch{error.value='No se encontró la venta.'}finally{cargando.value=false}}
async function anular(){if(!window.confirm('¿Anular esta venta y devolver el stock?'))return;venta.value=await anularVentaApi(props.id)}
function imprimir(){ window.print(); }
onMounted(cargar);
</script>
<template><AdminLayout titulo="Detalle de venta"><p v-if="cargando">Cargando venta...</p><p v-else-if="error" class="error">{{error}}</p><div v-else-if="venta" class="venta-shell">
<section class="cabecera panel-caso"><div><p class="eyebrow mono">VENTA #V-{{String(venta.id).padStart(4,'0')}}</p><h2>{{venta.cliente}}</h2><p>{{new Date(venta.fecha).toLocaleString('es-BO')}} · {{venta.empleado||'Sin vendedor asignado'}}</p></div><span class="estado-caso">{{venta.estado}}</span></section>
<section class="panel-caso"><table class="tabla-caso"><thead><tr><th>Producto</th><th>Edición</th><th>Cantidad</th><th>Precio</th><th>Subtotal</th></tr></thead><tbody><tr v-for="d in venta.detalles" :key="`${d.productoId}-${d.edicion}`"><td>{{d.producto}}</td><td>{{d.edicion}}</td><td>{{d.cantidad}}</td><td>${{d.precioUnitario.toFixed(2)}}</td><td>${{d.subtotal.toFixed(2)}}</td></tr></tbody></table></section>
<div class="columnas"><section class="panel-caso"><h3>Pagos registrados</h3><div v-if="venta.pagos?.length" class="pagos"><article v-for="p in venta.pagos" :key="p.id"><div><strong>{{p.metodoPago}}</strong><small>{{new Date(p.fecha).toLocaleString('es-BO')}} {{p.referencia?`· ${p.referencia}`:''}}</small></div><b>${{p.monto.toFixed(2)}}</b></article></div><p v-else class="muted">Todavía no se registraron pagos.</p></section><section class="panel-caso totales"><p><span>Total</span><strong>${{venta.total.toFixed(2)}}</strong></p><p><span>Pagado</span><strong>${{pagado.toFixed(2)}}</strong></p><p class="saldo"><span>Saldo pendiente</span><strong>${{saldo.toFixed(2)}}</strong></p></section></div>
<div class="acciones"><button class="btn-secondary" @click="imprimir">Imprimir comprobante</button><router-link v-if="saldo>0&&venta.estado!=='Anulada'" class="btn-primary" :to="`/admin/ventas/${venta.id}/pago`">Registrar pago</router-link><button v-if="venta.estado!=='Anulada'&&!pagado" class="peligro" @click="anular">Anular venta</button><button class="btn-secondary" @click="router.push('/admin/ventas')">Volver</button></div>
</div></AdminLayout></template>
<style scoped>
.venta-shell{display:grid;gap:20px}.cabecera{display:flex;justify-content:space-between;align-items:center}.cabecera h2{font-size:30px;margin:7px 0}.cabecera p:last-child{color:#929a90}.columnas{display:grid;grid-template-columns:1.5fr .8fr;gap:20px}.panel-caso h3{margin-bottom:16px}.pagos article,.totales p{display:flex;justify-content:space-between;padding:12px 0;border-bottom:1px solid #333}.pagos div{display:flex;flex-direction:column}.pagos small{color:#888;margin-top:4px}.totales .saldo{font-size:20px;color:#79dd68}.acciones{display:flex;gap:10px;flex-wrap:wrap}.acciones a{display:inline-flex;align-items:center}.peligro{border:1px solid #ffb4ab;background:#321819;color:#ffb4ab;padding:12px 18px;border-radius:8px}.error{color:#ffb4ab}@media(max-width:750px){.columnas{grid-template-columns:1fr}}@media print{.acciones{display:none}.venta-shell{color:#000}.panel-caso{background:#fff;border-color:#ccc}.estado-caso{color:#000}}
</style>

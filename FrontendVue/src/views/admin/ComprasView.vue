<script setup>
import { computed, onMounted, reactive, ref } from 'vue';
import AdminLayout from '../../components/AdminLayout.vue';
import AdminPageHeader from '../../components/common/AdminPageHeader.vue';
import DataTable from '../../components/common/DataTable.vue';
import StatusBadge from '../../components/common/StatusBadge.vue';
import LineItemEditor from '../../components/inventory/LineItemEditor.vue';
import { formatearCodigo, formatearDinero, formatearFechaHora } from '../../composables/useFormatters';
import { comprasApi, productosApi, proveedoresApi } from '../../services/recursosApi';
import { useAuthStore } from '../../stores/auth';
import { useNotificacionesStore } from '../../stores/notificaciones';

const auth=useAuthStore(),avisos=useNotificacionesStore();
const compras=ref([]),proveedores=ref([]),productos=ref([]);
const mostrarFormulario=ref(false),guardando=ref(false),error=ref('');
const compra=reactive({proveedorId:'',detalles:[{productoId:'',cantidad:1,precioUnitario:0}]});
const total=computed(()=>compra.detalles.reduce((suma,linea)=>suma+Number(linea.cantidad||0)*Number(linea.precioUnitario||0),0));

async function cargar(){
  try{[compras.value,proveedores.value,productos.value]=await Promise.all([comprasApi.obtenerTodos(),proveedoresApi.obtenerTodos(),productosApi.obtenerTodos()])}
  catch{error.value='No se pudieron cargar los datos de compras.'}
}
async function guardar(){
  error.value='';
  if(compra.detalles.some(linea=>!linea.productoId||linea.cantidad<1||linea.precioUnitario<=0)){error.value='Completa correctamente todos los productos.';return}
  guardando.value=true;
  try{
    const creada=await comprasApi.crear({proveedorId:Number(compra.proveedorId),empleadoId:auth.usuarioActual?.empleadoId||1,empleado:auth.usuarioActual?.username||'admin',detalles:compra.detalles});
    avisos.mostrar(`Compra #${creada.id} registrada. El inventario aumentó correctamente.`);
    compra.proveedorId='';compra.detalles=[{productoId:'',cantidad:1,precioUnitario:0}];mostrarFormulario.value=false;await cargar();
  }catch(excepcion){error.value=excepcion.response?.data?.message||'No se pudo registrar la compra.'}
  finally{guardando.value=false}
}
onMounted(cargar);
</script>

<template><AdminLayout titulo="Compras">
  <template #header><AdminPageHeader eyebrow="INVENTARIO / ABASTECIMIENTO" title="Compras a proveedores" description="Cada compra recibida incrementa automáticamente el stock."><router-link class="btn-secondary" to="/admin/proveedores">Proveedores</router-link><button class="btn-primary" @click="mostrarFormulario=!mostrarFormulario">Registrar compra</button></AdminPageHeader></template>
  <form v-if="mostrarFormulario" class="panel-caso form" @submit.prevent="guardar"><div class="campo"><label>Proveedor</label><select v-model="compra.proveedorId" required><option value="">Seleccionar</option><option v-for="proveedor in proveedores.filter(item=>item.activo)" :key="proveedor.id" :value="proveedor.id">{{proveedor.razonSocial}}</option></select></div><LineItemEditor v-model="compra.detalles" :products="productos" price-label="Costo unitario"/><div class="footer"><p>Total: <strong>{{formatearDinero(total)}}</strong></p><button class="btn-primary" :disabled="guardando">{{guardando?'Registrando...':'Confirmar ingreso y actualizar stock'}}</button></div><p v-if="error" class="error">{{error}}</p></form>
  <p v-else-if="error" class="error">{{error}}</p>
  <section class="panel-caso"><DataTable :empty="!compras.length" empty-text="Todavía no hay compras registradas" :columns="6"><template #header><thead><tr><th>Compra</th><th>Proveedor</th><th>Fecha</th><th>Productos</th><th>Total</th><th>Estado</th></tr></thead></template><tr v-for="item in compras" :key="item.id"><td><router-link class="accion-caso mono" :to="`/admin/compras/${item.id}`">{{formatearCodigo('C',item.id)}}</router-link></td><td>{{item.proveedor}}</td><td>{{formatearFechaHora(item.fecha)}}</td><td>{{item.detalles?.reduce((suma,detalle)=>suma+detalle.cantidad,0)||0}}</td><td>{{formatearDinero(item.total)}}</td><td><StatusBadge :status="item.estado"/></td></tr></DataTable></section>
</AdminLayout></template>

<style scoped>.form{display:grid;gap:16px}.footer{display:flex;justify-content:flex-end;align-items:center;gap:16px;border-top:1px solid #333;padding-top:16px}.footer p{font-size:20px}.error{color:#ffb4ab}@media(max-width:650px){.footer{align-items:stretch;flex-direction:column}}</style>

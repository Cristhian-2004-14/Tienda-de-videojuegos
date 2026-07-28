<script setup>
import { computed, onMounted, reactive, ref } from 'vue';
import AdminLayout from '../../components/AdminLayout.vue';
import { comprasApi, productosApi, proveedoresApi } from '../../services/recursosApi';
import { useAuthStore } from '../../stores/auth';
import { useNotificacionesStore } from '../../stores/notificaciones';

const auth = useAuthStore();
const avisos = useNotificacionesStore();
const compras = ref([]), proveedores = ref([]), productos = ref([]);
const mostrarFormulario = ref(false), guardando = ref(false), error = ref('');
const compra = reactive({ proveedorId: '', detalles: [] });
const total = computed(() => compra.detalles.reduce((suma, linea) => suma + Number(linea.cantidad || 0) * Number(linea.precioUnitario || 0), 0));
function agregarLinea(){ compra.detalles.push({ productoId: '', cantidad: 1, precioUnitario: 0 }); }
function quitarLinea(indice){ compra.detalles.splice(indice, 1); if (!compra.detalles.length) agregarLinea(); }
async function cargar(){ try { [compras.value, proveedores.value, productos.value] = await Promise.all([comprasApi.obtenerTodos(), proveedoresApi.obtenerTodos(), productosApi.obtenerTodos()]); } catch { error.value='No se pudieron cargar los datos de compras.'; } }
async function guardar(){
  error.value='';
  if (compra.detalles.some(d=>!d.productoId||d.cantidad<1||d.precioUnitario<=0)){error.value='Completa correctamente todos los productos.';return}
  guardando.value=true;
  try{
    const creada = await comprasApi.crear({ proveedorId:Number(compra.proveedorId), empleadoId:auth.usuarioActual?.empleadoId||1, empleado:auth.usuarioActual?.username||'admin', detalles:compra.detalles.map(d=>({...d,productoId:Number(d.productoId)})) });
    avisos.mostrar(`Compra #${creada.id} registrada. El inventario aumentó correctamente.`);
    compra.proveedorId=''; compra.detalles=[]; agregarLinea(); mostrarFormulario.value=false; await cargar();
  }catch(excepcion){error.value=excepcion.response?.data?.message||'No se pudo registrar la compra.'}finally{guardando.value=false}
}
onMounted(async()=>{agregarLinea();await cargar()});
</script>
<template>
  <AdminLayout titulo="Compras">
    <template #header><div class="modulo-header"><div><p class="eyebrow mono">INVENTARIO / ABASTECIMIENTO</p><h2>Compras a proveedores</h2><p>Cada compra recibida incrementa automáticamente el stock.</p></div><div class="header-acciones"><router-link class="btn-secondary" to="/admin/proveedores">Proveedores</router-link><button class="btn-primary" @click="mostrarFormulario=!mostrarFormulario">Registrar compra</button></div></div></template>
    <form v-if="mostrarFormulario" class="panel-caso formulario" @submit.prevent="guardar">
      <div class="campo"><label>Proveedor</label><select v-model="compra.proveedorId" required><option value="">Seleccionar</option><option v-for="p in proveedores.filter(p=>p.activo)" :key="p.id" :value="p.id">{{p.razonSocial}}</option></select></div>
      <div class="lineas"><article v-for="(linea,indice) in compra.detalles" :key="indice" class="linea"><div class="campo producto"><label>Producto</label><select v-model="linea.productoId" required><option value="">Seleccionar</option><option v-for="p in productos" :key="p.id" :value="p.id">{{p.nombre}} (stock: {{p.stock}})</option></select></div><div class="campo"><label>Cantidad</label><input v-model.number="linea.cantidad" type="number" min="1" required></div><div class="campo"><label>Costo unitario</label><input v-model.number="linea.precioUnitario" type="number" min=".01" step=".01" required></div><button class="quitar" type="button" aria-label="Quitar producto" @click="quitarLinea(indice)">×</button></article></div>
      <button class="accion-caso agregar" type="button" @click="agregarLinea">+ Agregar otro producto</button>
      <div class="pie"><p>Total: <strong>${{total.toFixed(2)}}</strong></p><button class="btn-primary" :disabled="guardando">{{guardando?'Registrando...':'Confirmar ingreso y actualizar stock'}}</button></div><p v-if="error" class="error">{{error}}</p>
    </form>
    <p v-else-if="error" class="error">{{error}}</p>
    <section class="panel-caso"><table class="tabla-caso"><thead><tr><th>Compra</th><th>Proveedor</th><th>Fecha</th><th>Productos</th><th>Total</th><th>Estado</th></tr></thead><tbody><tr v-for="item in compras" :key="item.id"><td><router-link class="accion-caso mono" :to="`/admin/compras/${item.id}`">#C-{{item.id}}</router-link></td><td>{{item.proveedor}}</td><td>{{new Date(item.fecha).toLocaleString('es-BO')}}</td><td>{{item.detalles?.reduce((s,d)=>s+d.cantidad,0)||0}}</td><td>${{Number(item.total).toFixed(2)}}</td><td><span class="estado-caso">{{item.estado}}</span></td></tr></tbody></table></section>
  </AdminLayout>
</template>
<style scoped>
.header-acciones,.pie{display:flex;gap:10px;align-items:center}.formulario,.lineas{display:grid;gap:16px}.linea{display:grid;grid-template-columns:minmax(220px,2fr) 1fr 1fr auto;gap:12px;align-items:end;padding:14px;background:#171717;border:1px solid #333;border-radius:8px}.quitar{height:42px;width:42px;border:1px solid #5c3535;background:#291919;color:#ffb4ab;border-radius:7px;font-size:22px}.agregar{justify-self:start;background:none;border:0;cursor:pointer}.pie{justify-content:flex-end;border-top:1px solid #333;padding-top:16px}.pie p{font-size:20px}.error{color:#ffb4ab}@media(max-width:750px){.linea{grid-template-columns:1fr 1fr}.producto{grid-column:1/-1}.header-acciones,.pie{align-items:stretch;flex-direction:column}}
</style>

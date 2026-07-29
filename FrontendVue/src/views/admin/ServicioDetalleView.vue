<script setup>
import { computed, onMounted, reactive, ref } from 'vue';
import AdminLayout from '../../components/AdminLayout.vue';
import StatusBadge from '../../components/common/StatusBadge.vue';
import LineItemEditor from '../../components/inventory/LineItemEditor.vue';
import PaymentForm from '../../components/payments/PaymentForm.vue';
import PaymentHistory from '../../components/payments/PaymentHistory.vue';
import PaymentSummary from '../../components/payments/PaymentSummary.vue';
import { formatearCodigo, formatearFechaHora } from '../../composables/useFormatters';
import {
  actualizarSeguimientoServicioApi,
  productosApi,
  registrarPagoServicioApi,
  serviciosApi,
} from '../../services/recursosApi';

const props=defineProps({id:{type:String,required:true}});
const servicio=ref(null),productos=ref([]),error=ref(''),mensaje=ref(''),guardando=ref(false),guardandoPago=ref(false);
const estados=['Recibido','En diagnóstico','En reparación','En pruebas','Listo para entrega','Entregado','Cancelado'];
const seguimiento=reactive({estado:'Recibido',diagnostico:'',costoManoObra:0,detalles:[]});
const total=computed(()=>Number(seguimiento.costoManoObra||0)+seguimiento.detalles.reduce((s,d)=>s+Number(d.cantidad||0)*Number(d.precioUnitario||0),0));
const pagado=computed(()=>(servicio.value?.pagos||[]).reduce((s,p)=>s+Number(p.monto||0),0));
const saldo=computed(()=>Math.max(0,total.value-pagado.value));
const datosSeguimiento=()=>({...seguimiento,detalles:seguimiento.detalles.map(d=>({...d,productoId:Number(d.productoId)}))});

async function cargar(){
  try{
    [servicio.value,productos.value]=await Promise.all([serviciosApi.obtenerPorId(props.id),productosApi.obtenerTodos()]);
    Object.assign(seguimiento,{estado:servicio.value.estado,diagnostico:servicio.value.diagnostico,costoManoObra:servicio.value.costoManoObra,detalles:servicio.value.detalles?.map(d=>({...d}))||[]});
  }catch{error.value='No se pudo cargar la orden.'}
}
async function guardar(){
  guardando.value=true;error.value='';mensaje.value='';
  try{
    servicio.value=await actualizarSeguimientoServicioApi(props.id,datosSeguimiento());
    mensaje.value='Seguimiento actualizado y stock sincronizado.';
    await cargar();
  }catch(e){error.value=e.response?.data?.message||'No se pudo actualizar el servicio.'}
  finally{guardando.value=false}
}
async function registrarPago(pago){
  guardandoPago.value=true;error.value='';mensaje.value='';
  try{
    servicio.value=await actualizarSeguimientoServicioApi(props.id,datosSeguimiento());
    servicio.value=await registrarPagoServicioApi(props.id,pago);
    await cargar();
    mensaje.value='Servicio actualizado y pago registrado correctamente.';
  }
  catch(e){error.value=e.response?.data?.message||'No se pudo registrar el pago.'}
  finally{guardandoPago.value=false}
}
onMounted(cargar);
</script>

<template><AdminLayout titulo="Detalle de servicio">
  <p v-if="!servicio&&!error">Cargando orden...</p><p v-if="error&&!servicio" class="error">{{error}}</p>
  <div v-if="servicio" class="page">
    <header class="panel-caso detail-header"><div><router-link to="/admin/servicios" class="accion-caso">← Volver</router-link><p class="eyebrow mono">ORDEN {{formatearCodigo('SRV',servicio.id)}}</p><h2>{{servicio.dispositivo}}</h2><p>{{servicio.cliente}} · {{formatearFechaHora(servicio.fechaIngreso)}}</p></div><StatusBadge :status="servicio.estado"/></header>
    <div class="columns">
      <form class="panel-caso form" @submit.prevent="guardar">
        <h3>Seguimiento técnico</h3>
        <div class="campo"><label>Estado actual</label><select v-model="seguimiento.estado"><option v-for="estado in estados" :key="estado">{{estado}}</option></select></div>
        <div class="campo"><label>Diagnóstico y trabajo realizado</label><textarea v-model.trim="seguimiento.diagnostico" rows="5" required></textarea></div>
        <div class="campo"><label>Costo de mano de obra</label><input v-model.number="seguimiento.costoManoObra" type="number" min="0" step=".01"></div>
        <h3>Repuestos utilizados</h3>
        <LineItemEditor v-model="seguimiento.detalles" :products="productos" price-label="Precio aplicado" auto-price allow-empty/>
        <button class="btn-primary" :disabled="guardando">{{guardando?'Guardando...':'Actualizar servicio'}}</button>
      </form>
      <aside>
        <PaymentSummary :total="total" :paid="pagado" :balance="saldo"/>
        <PaymentForm v-if="saldo>0" :max="saldo" :loading="guardandoPago" submit-text="Registrar abono" @submit="registrarPago"/>
        <PaymentHistory :payments="servicio.pagos"/>
      </aside>
    </div>
    <p v-if="error" class="error">{{error}}</p><p v-if="mensaje" class="success">{{mensaje}}</p>
  </div>
</AdminLayout></template>

<style scoped>.page{display:grid;gap:20px}.detail-header{display:flex;justify-content:space-between;align-items:center}.detail-header h2{font-size:29px;margin:7px 0}.detail-header p:last-child{color:#929a90}.columns{display:grid;grid-template-columns:minmax(0,1.5fr) minmax(300px,.75fr);gap:20px}.form,aside{display:grid;align-content:start;gap:20px}.error{color:#ffb4ab}.success{color:#79dd68}@media(max-width:950px){.columns{grid-template-columns:1fr}}</style>

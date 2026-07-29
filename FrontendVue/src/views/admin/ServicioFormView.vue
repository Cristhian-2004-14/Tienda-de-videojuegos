<script setup>
import { computed, onMounted, reactive, ref } from 'vue';
import { useRouter } from 'vue-router';
import AdminLayout from '../../components/AdminLayout.vue';
import SelectorCliente from '../../components/common/SelectorCliente.vue';
import { clientesApi, dispositivosApi, empleadosApi, serviciosApi } from '../../services/recursosApi';
import { useAuthStore } from '../../stores/auth';
import { useNotificacionesStore } from '../../stores/notificaciones';
const router=useRouter(),auth=useAuthStore();
const avisos=useNotificacionesStore();
const clientes=ref([]),dispositivos=ref([]),empleados=ref([]),error=ref(''),guardando=ref(false);
const form=reactive({clienteId:'',dispositivoId:'',empleadoId:auth.usuarioActual?.empleadoId||'',diagnostico:''});
const equipos=computed(()=>dispositivos.value.filter(d=>d.clienteId===Number(form.clienteId)));
async function cargar(){try{[clientes.value,dispositivos.value,empleados.value]=await Promise.all([clientesApi.obtenerTodos(),dispositivosApi.obtenerTodos(),empleadosApi.obtenerTodos()])}catch{error.value='No se pudieron cargar los datos del taller.'}}
async function guardar(){error.value='';if(!form.clienteId){error.value='Selecciona un cliente.';return}if(!form.dispositivoId){error.value='Selecciona un dispositivo.';return}guardando.value=true;try{const empleado=empleados.value.find(e=>e.id===Number(form.empleadoId));const creado=await serviciosApi.crear({...form,clienteId:Number(form.clienteId),dispositivoId:Number(form.dispositivoId),empleadoId:Number(form.empleadoId)||0,empleado:empleado?`${empleado.nombre} ${empleado.apellido}`:'',costoManoObra:0,detalles:[],pagos:[]});avisos.mostrar(`Orden #SRV-${String(creado.id).padStart(4,'0')} creada correctamente.`);router.push(`/admin/servicios/${creado.id}`)}catch(e){error.value=e.response?.data?.message||'No se pudo crear la orden.'}finally{guardando.value=false}}
onMounted(cargar);
</script>
<template><AdminLayout titulo="Nueva orden de servicio"><div class="form-shell"><div><p class="eyebrow mono">TALLER / RECEPCIÓN</p><h2>Abrir orden de trabajo</h2><p class="muted">Selecciona un dispositivo registrado. La orden iniciará como “Recibido”.</p></div><form class="panel-caso formulario-grid" @submit.prevent="guardar"><div class="ancho"><SelectorCliente v-model="form.clienteId" :clientes="clientes" label="Cliente" @change="form.dispositivoId=''" /></div><div class="campo ancho"><label>Dispositivo</label><select v-model="form.dispositivoId" required :disabled="!form.clienteId"><option value="">{{equipos.length?'Seleccionar equipo':'Este cliente no tiene dispositivos'}}</option><option v-for="d in equipos" :key="d.id" :value="d.id">{{d.marca}} {{d.modelo}} · {{d.numeroSerie||'sin serie'}}</option></select><router-link class="ayuda-link" to="/admin/dispositivos">Registrar un dispositivo nuevo</router-link></div><div class="campo"><label>Técnico responsable</label><select v-model="form.empleadoId"><option value="">Sin asignar</option><option v-for="e in empleados.filter(e=>e.activo)" :key="e.id" :value="e.id">{{e.nombre}} {{e.apellido}} · {{e.cargo}}</option></select></div><div class="campo ancho"><label>Falla reportada</label><textarea v-model.trim="form.diagnostico" rows="4" required></textarea></div><p v-if="error" class="error ancho">{{error}}</p><button class="btn-primary" :disabled="guardando">{{guardando?'Generando...':'Generar orden de trabajo'}}</button></form></div></AdminLayout></template>
<style scoped>.form-shell{max-width:900px;display:grid;gap:24px}.form-shell h2{font-size:32px;margin:8px 0}.ancho{grid-column:1/-1}.ayuda-link{color:#79dd68;font-size:12px}.error{color:#ffb4ab}@media(max-width:650px){.ancho{grid-column:auto}}</style>

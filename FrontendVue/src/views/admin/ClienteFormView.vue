<script setup>
import { computed, onMounted, reactive, ref, watch } from 'vue';
import { storeToRefs } from 'pinia';
import { useRouter } from 'vue-router';
import AdminLayout from '../../components/AdminLayout.vue';
import { useDatosApiStore } from '../../stores/datosApi';
import { clientesApi } from '../../services/recursosApi';
import { useNotificacionesStore } from '../../stores/notificaciones';
const props=defineProps({id:{type:String,default:null}}),router=useRouter(),store=useDatosApiStore();
const avisos=useNotificacionesStore();
const {clientes}=storeToRefs(store),error=ref('');
const original=computed(()=>clientes.value.find(c=>c.id===Number(props.id))),editando=computed(()=>Boolean(original.value));
const form=reactive({nombre:'',apellido:'',ci:'',telefono:'',email:'',direccion:''});
watch(original,c=>{if(c)Object.assign(form,c)},{immediate:true});
async function guardar(){error.value='';try{await store.guardarCliente({...form,id:original.value?.id||0});avisos.mostrar(`Cliente ${editando.value?'actualizado':'registrado'} correctamente.`);router.push('/admin/clientes')}catch(e){error.value=e.response?.data?.message||'No se pudo guardar el cliente.'}}
onMounted(()=>store.cargarRecurso('clientes',clientesApi));
</script>
<template><AdminLayout :titulo="editando?'Editar cliente':'Agregar cliente'"><template #header><div><p class="eyebrow mono">CLIENTES / FICHA</p><h2>{{editando?'Editar cliente':'Agregar cliente'}}</h2><p class="muted">El CI no puede repetirse entre clientes.</p></div></template><form class="panel-caso formulario-grid ficha" @submit.prevent="guardar"><div class="campo"><label>Nombre</label><input v-model.trim="form.nombre" required></div><div class="campo"><label>Apellido</label><input v-model.trim="form.apellido" required></div><div class="campo"><label>CI</label><input v-model.trim="form.ci"></div><div class="campo"><label>Teléfono</label><input v-model.trim="form.telefono" required></div><div class="campo ancho"><label>Correo electrónico</label><input v-model.trim="form.email" type="email"></div><div class="campo ancho"><label>Dirección</label><textarea v-model.trim="form.direccion" rows="3"></textarea></div><p v-if="error" class="error ancho">{{error}}</p><div class="acciones ancho"><button type="button" class="btn-secondary" @click="router.back()">Cancelar</button><button class="btn-primary">Guardar cliente</button></div></form></AdminLayout></template>
<style scoped>.ficha{max-width:850px}.ancho{grid-column:1/-1}.acciones{display:flex;justify-content:flex-end;gap:10px}.error{color:#ffb4ab}@media(max-width:650px){.ancho{grid-column:auto}}</style>

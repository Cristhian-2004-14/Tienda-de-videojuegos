<script setup>
import { onMounted, reactive, ref } from 'vue';
import AdminLayout from '../../components/AdminLayout.vue';
import { rolesApi } from '../../services/recursosApi';
const roles=ref([]),abierto=ref(false),editando=ref(null),error=ref('');
const modulos=['dashboard','productos','ventas','servicios','clientes','compras','personal','reportes','roles'];
const form=reactive({nombre:'',descripcion:'',permisos:[],protegido:false});
async function cargar(){try{roles.value=await rolesApi.obtenerTodos()}catch{error.value='No se pudieron cargar los roles.'}}
function limpiar(){editando.value=null;abierto.value=false;Object.assign(form,{nombre:'',descripcion:'',permisos:[],protegido:false})}
function editar(r){editando.value=r.id;abierto.value=true;Object.assign(form,{...r,permisos:[...r.permisos]})}
async function guardar(){error.value='';try{if(editando.value)await rolesApi.actualizar(editando.value,{...form,id:editando.value});else await rolesApi.crear({...form,permisos:[...form.permisos]});limpiar();await cargar()}catch{error.value='No se pudo guardar el rol.'}}
onMounted(cargar);
</script>
<template><AdminLayout titulo="Roles y permisos"><template #header><div class="modulo-header"><div><p class="eyebrow mono">SEGURIDAD / ACCESO</p><h2>Roles y permisos</h2><p>Los permisos controlan las rutas y opciones visibles de cada usuario.</p></div><button class="btn-primary" @click="abierto=!abierto">Nuevo rol</button></div></template>
<form v-if="abierto" class="panel-caso formulario-caso" @submit.prevent="guardar"><div class="campo"><label>Nombre</label><input v-model.trim="form.nombre" required></div><div class="campo"><label>Descripción</label><input v-model.trim="form.descripcion" required></div><fieldset><legend>Módulos permitidos</legend><label v-for="m in modulos" :key="m" class="check"><input v-model="form.permisos" type="checkbox" :value="m">{{m}}</label></fieldset><div class="acciones"><button class="btn-primary">{{editando?'Actualizar rol':'Guardar rol'}}</button><button class="btn-secondary" type="button" @click="limpiar">Cancelar</button></div></form><p v-if="error" class="error">{{error}}</p>
<div class="tarjetas-caso"><article v-for="r in roles" :key="r.id" class="panel-caso"><div class="rol-head"><span class="material-symbols-outlined">shield_person</span><span v-if="r.protegido" class="estado-caso">Protegido</span></div><h3>{{r.nombre}}</h3><p class="muted">{{r.descripcion}}</p><div class="chips"><span v-for="p in r.permisos" :key="p">{{p}}</span></div><button class="accion-caso editar" @click="editar(r)">Editar permisos</button></article></div></AdminLayout></template>
<style scoped>fieldset{border:1px solid #343434;padding:16px;display:flex;flex-wrap:wrap;gap:14px}legend{padding:0 8px}.check{display:flex;gap:7px;text-transform:capitalize}.acciones{display:flex;gap:10px}.rol-head{display:flex;justify-content:space-between;color:#79dd68}.panel-caso h3{margin:14px 0 7px}.chips{display:flex;flex-wrap:wrap;gap:6px;margin-top:18px}.chips span{background:#292929;padding:5px 8px;border-radius:4px;font-size:10px;text-transform:uppercase}.editar{margin-top:18px;background:none;border:0;cursor:pointer}.error{color:#ffb4ab}</style>

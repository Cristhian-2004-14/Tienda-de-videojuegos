<script setup>
import { computed } from 'vue';
const props=defineProps({status:{type:String,required:true}});
const tipo=computed(()=>({
  Completada:'success',Entregado:'success','Listo para entrega':'success',Activo:'success',Recibida:'success',
  Pendiente:'warning','En diagnóstico':'warning',Recibido:'neutral',
  'En reparación':'info','En pruebas':'info',
  Anulada:'danger',Cancelado:'danger',Inactivo:'danger',
}[props.status]||'neutral'));
</script>
<template><span class="badge" :class="tipo">{{status}}</span></template>
<style scoped>.badge{display:inline-flex;padding:6px 9px;border-radius:5px;font-size:11px;font-weight:700;white-space:nowrap}.success{background:#153c18;color:#79dd68}.warning{background:#493015;color:#ffc36c}.info{background:#18334b;color:#7fc4ff}.danger{background:#431d1d;color:#ffb4ab}.neutral{background:#333;color:#d2d2d2}</style>

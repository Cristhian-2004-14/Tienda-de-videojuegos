<script setup>
import { reactive, watch } from 'vue';
const props=defineProps({max:{type:Number,required:true},loading:{type:Boolean,default:false},submitText:{type:String,default:'Registrar pago'}});
const emit=defineEmits(['submit']);
const form=reactive({metodoPago:'Efectivo',monto:props.max,referencia:''});
watch(()=>props.max,valor=>form.monto=Number(valor.toFixed(2)));
function submit(){emit('submit',{...form})}
</script>
<template><form class="panel-caso formulario-caso" @submit.prevent="submit"><div class="methods"><label v-for="method in ['Efectivo','QR','Tarjeta','Transferencia']" :key="method" :class="{selected:form.metodoPago===method}"><input v-model="form.metodoPago" type="radio" :value="method"><span class="material-symbols-outlined">{{method==='Efectivo'?'payments':method==='QR'?'qr_code_2':method==='Tarjeta'?'credit_card':'account_balance'}}</span>{{method}}</label></div><div class="campo"><label>Monto a registrar</label><input v-model.number="form.monto" type="number" min=".01" :max="max" step=".01" required></div><div class="campo"><label>Referencia o nota (opcional)</label><input v-model.trim="form.referencia" placeholder="Ej. comprobante 88421"></div><button class="btn-primary" :disabled="loading">{{loading?'Registrando...':submitText}}</button></form></template>
<style scoped>.methods{display:grid;grid-template-columns:repeat(4,1fr);gap:10px}.methods label{display:flex;flex-direction:column;gap:8px;padding:17px;border:1px solid #353535;border-radius:8px;background:#171717;cursor:pointer}.methods input{position:absolute;opacity:0}.methods .selected{border-color:#79dd68;background:#132714}@media(max-width:700px){.methods{grid-template-columns:repeat(2,1fr)}}</style>

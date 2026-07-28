<script setup>
import { formatearDinero, formatearFechaHora } from '../../composables/useFormatters';
defineProps({payments:{type:Array,default:()=>[]}});
</script>
<template><section class="panel-caso"><h3>Historial de pagos</h3><EmptyState v-if="!payments.length" title="Sin pagos registrados" icon="payments"/><article v-for="pago in payments" :key="pago.id" class="payment"><div><strong>{{pago.metodoPago}}</strong><small>{{formatearFechaHora(pago.fecha)}} {{pago.referencia?`· ${pago.referencia}`:''}}</small></div><b>{{formatearDinero(pago.monto)}}</b></article></section></template>
<script>
import EmptyState from '../common/EmptyState.vue';
export default { components: { EmptyState } };
</script>
<style scoped>.panel-caso h3{margin-bottom:12px}.payment{display:flex;justify-content:space-between;padding:13px 0;border-bottom:1px solid #333}.payment div{display:flex;flex-direction:column;gap:4px}.payment small{color:#929a90}</style>

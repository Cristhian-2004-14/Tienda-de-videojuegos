<script setup>
import { formatearDinero } from '../../composables/useFormatters';
defineProps({total:{type:Number,default:0},paid:{type:Number,default:0},balance:{type:Number,default:0}});
</script>
<template><section class="summary"><article><small>TOTAL</small><strong>{{formatearDinero(total)}}</strong></article><article><small>PAGADO</small><strong>{{formatearDinero(paid)}}</strong></article><article class="balance"><small>SALDO</small><strong>{{formatearDinero(balance)}}</strong></article></section></template>
<style scoped>.summary{display:grid;grid-template-columns:repeat(3,1fr);gap:12px}.summary article{display:grid;gap:5px;padding:18px;background:#171717;border:1px solid #333;border-radius:8px}.summary small{color:#929a90}.summary strong{font-size:24px}.summary .balance{border-color:#79dd68}@media(max-width:600px){.summary{grid-template-columns:1fr}}</style>

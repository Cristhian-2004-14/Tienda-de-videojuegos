<script setup>
defineProps({
  empty: { type: Boolean, default: false },
  emptyText: { type: String, default: 'No existen registros.' },
  columns: { type: Number, default: 1 },
  minWidth: { type: String, default: '760px' },
});
</script>
<template><div class="table-wrap"><table class="tabla-caso" :style="{minWidth}"><slot name="header"/><tbody><slot v-if="!empty"/><tr v-else><td :colspan="columns"><EmptyState :title="emptyText"/></td></tr></tbody><slot name="footer"/></table></div></template>
<script>
import EmptyState from './EmptyState.vue';
export default { components: { EmptyState } };
</script>
<style scoped>.table-wrap{overflow:auto}.table-wrap :deep(td:has(.empty)){padding:0}</style>

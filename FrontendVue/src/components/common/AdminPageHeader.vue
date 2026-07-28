<script setup>
defineProps({
  eyebrow: { type: String, default: '' },
  title: { type: String, required: true },
  description: { type: String, default: '' },
});
</script>

<template>
  <div class="page-header">
    <div>
      <p v-if="eyebrow" class="eyebrow mono">{{ eyebrow }}</p>
      <h2>{{ title }}</h2>
      <p v-if="description" class="description">{{ description }}</p>
    </div>
    <div v-if="$slots.default" class="actions"><slot /></div>
  </div>
</template>

<style scoped>
.page-header{display:flex;justify-content:space-between;align-items:flex-end;gap:20px}.eyebrow{font-size:11px;color:#79dd68;margin-bottom:7px}.page-header h2{font-size:32px}.description{color:#9ba497;margin-top:7px}.actions{display:flex;gap:10px;align-items:center}@media(max-width:700px){.page-header{align-items:flex-start;flex-direction:column}.actions{width:100%;flex-wrap:wrap}}
</style>

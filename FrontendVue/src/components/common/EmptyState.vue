<script setup>
defineProps({
  title: { type: String, default: 'Sin resultados' },
  description: { type: String, default: '' },
  icon: { type: String, default: 'inbox' },
});
</script>
<template><div class="empty"><span class="material-symbols-outlined">{{icon}}</span><strong>{{title}}</strong><p v-if="description">{{description}}</p><slot /></div></template>
<style scoped>.empty{display:grid;justify-items:center;text-align:center;gap:8px;padding:42px 20px;color:#929a90}.empty>span{font-size:42px;color:#79dd68}.empty strong{color:#d8ddd6;font-size:17px}.empty p{max-width:480px;line-height:1.5}</style>

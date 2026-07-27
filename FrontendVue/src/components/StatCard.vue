<script setup>
defineProps({
  etiqueta: { type: String, required: true },
  valor: { type: [String, Number], required: true },
  icono: { type: String, required: true },
  nota: { type: String, default: '' },
  notaTipo: { type: String, default: 'positiva' }, // 'positiva' | 'negativa' | 'neutra'
});
</script>

<template>
  <div class="stat-card">
    <div class="stat-header">
      <span class="stat-label">{{ etiqueta }}</span>
      <span class="material-symbols-outlined stat-icon">{{ icono }}</span>
    </div>
    <div>
      <h2 class="stat-valor">{{ valor }}</h2>
      <p v-if="nota" class="stat-nota mono" :class="notaTipo">{{ nota }}</p>
    </div>
  </div>
</template>

<style scoped>
.stat-card {
  background: var(--color-surface-container-low);
  border-radius: var(--radius);
  padding: var(--space-md);
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  transition: background-color 0.2s;
}

.stat-card:hover {
  background: var(--color-surface-container-high);
}

.stat-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: var(--space-md);
}

.stat-label {
  font-size: 11px;
  font-weight: 700;
  letter-spacing: 0.05em;
  text-transform: uppercase;
  color: var(--color-on-surface-variant);
}

.stat-icon {
  color: var(--color-primary);
}

.stat-valor {
  font-size: 32px;
  font-weight: 700;
  letter-spacing: -0.01em;
}

.stat-nota {
  font-size: 10px;
  margin-top: 4px;
}

.stat-nota.positiva {
  color: var(--color-primary);
}

.stat-nota.negativa {
  color: var(--color-error);
}

.stat-nota.neutra {
  color: var(--color-on-surface-variant);
}
</style>

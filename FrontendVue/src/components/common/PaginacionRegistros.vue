<script setup>
import { computed } from 'vue';

const props = defineProps({
  pagina: { type: Number, default: 1 },
  total: { type: Number, default: 0 },
  porPagina: { type: Number, default: 10 },
});
const emit = defineEmits(['update:pagina']);

const totalPaginas = computed(() => Math.max(1, Math.ceil(props.total / props.porPagina)));
const inicio = computed(() => props.total ? ((props.pagina - 1) * props.porPagina) + 1 : 0);
const fin = computed(() => Math.min(props.pagina * props.porPagina, props.total));
const paginas = computed(() => {
  const cantidad = Math.min(5, totalPaginas.value);
  const primera = Math.min(
    Math.max(1, props.pagina - Math.floor(cantidad / 2)),
    Math.max(1, totalPaginas.value - cantidad + 1),
  );
  return Array.from({ length: cantidad }, (_, indice) => primera + indice);
});

function irA(pagina) {
  emit('update:pagina', Math.min(Math.max(1, pagina), totalPaginas.value));
}
</script>

<template>
  <nav v-if="total > porPagina" class="paginacion" aria-label="Paginación de registros">
    <p>Mostrando <strong>{{ inicio }}–{{ fin }}</strong> de <strong>{{ total }}</strong></p>
    <div>
      <button type="button" :disabled="pagina <= 1" aria-label="Página anterior" @click="irA(pagina - 1)">
        <span class="material-symbols-outlined">chevron_left</span>
      </button>
      <button
        v-for="numero in paginas"
        :key="numero"
        type="button"
        :class="{ activa: numero === pagina }"
        :aria-current="numero === pagina ? 'page' : undefined"
        :aria-label="`Página ${numero}`"
        @click="irA(numero)"
      >{{ numero }}</button>
      <button type="button" :disabled="pagina >= totalPaginas" aria-label="Página siguiente" @click="irA(pagina + 1)">
        <span class="material-symbols-outlined">chevron_right</span>
      </button>
    </div>
  </nav>
</template>

<style scoped>
.paginacion{display:flex;align-items:center;justify-content:space-between;gap:16px;padding-top:18px}.paginacion p{color:#879184;font-size:11px}.paginacion p strong{color:#cbd3c8;font-variant-numeric:tabular-nums}.paginacion>div{display:flex;gap:6px}.paginacion button{min-width:38px;height:38px;display:grid;place-items:center;border:1px solid #343934;border-radius:6px;background:#191c19;color:#aeb7aa;font-size:11px;cursor:pointer}.paginacion button:hover:not(:disabled){border-color:var(--color-primary);color:#fff}.paginacion button.activa{border-color:#107c10;background:#107c10;color:#fff;font-weight:800}.paginacion button:disabled{opacity:.35;cursor:not-allowed}.paginacion .material-symbols-outlined{font-size:18px}@media(max-width:560px){.paginacion{align-items:flex-start;flex-direction:column}.paginacion>div{width:100%;justify-content:space-between}.paginacion button{min-width:36px}}
</style>

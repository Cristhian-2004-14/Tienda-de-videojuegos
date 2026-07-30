<script setup>
import { computed } from 'vue';

const props = defineProps({
  producto: { type: Object, default: null },
  tipo: { type: String, default: '' },
  grande: { type: Boolean, default: false },
});

const iconos = {
  Consolas: 'stadia_controller',
  Accesorios: 'gamepad',
  Videojuegos: 'sports_esports',
};

const visualProducto = computed(() => props.producto || {
  id: 0,
  categoria: props.tipo || 'Otros',
  marca: props.tipo || 'X-Store',
});
const imagenPrincipal = computed(() => visualProducto.value.imagenes?.[0]?.url
  || visualProducto.value.imagenUrl
  || '');
</script>

<template>
  <div class="visual" :class="[`visual-${visualProducto.id}`, { grande }]">
    <img
      v-if="imagenPrincipal"
      :src="imagenPrincipal"
      :alt="visualProducto.nombre || 'Imagen del producto'"
      loading="lazy"
      decoding="async"
    />
    <span class="halo"></span>
    <span v-if="!imagenPrincipal" class="material-symbols-outlined">{{ iconos[visualProducto.categoria] || 'devices' }}</span>
    <small>{{ visualProducto.marca }}</small>
  </div>
</template>

<style scoped>
.visual { width:100%; height:100%; min-height:220px; position:relative; overflow:hidden; display:grid; place-items:center; background:radial-gradient(circle at 50% 75%,rgba(121,221,104,.28),transparent 34%),linear-gradient(145deg,#19231b,#070907 70%); }
.visual img { position:absolute; inset:0; width:100%; height:100%; object-fit:cover; object-position:center; z-index:1; transition:transform .45s cubic-bezier(.2,.8,.2,1); }
.visual:hover img { transform:scale(1.035); }
.visual::before { content:""; position:absolute; inset:18px; border:1px solid rgba(121,221,104,.13); transform:skew(-7deg); }
.visual .material-symbols-outlined { font-size:86px; color:#dce7da; filter:drop-shadow(0 16px 18px #000); font-variation-settings:'FILL' 1; z-index:2; }
.visual small { position:absolute; bottom:16px; left:18px; z-index:2; padding:5px 8px; border-radius:4px; background:rgba(5,7,5,.72); font:500 10px 'JetBrains Mono'; color:#d5ded3; letter-spacing:.12em; text-transform:uppercase; }
.halo { position:absolute; width:130px; height:18px; border-radius:50%; background:#1fa51c; filter:blur(16px); bottom:48px; }
.grande { width:100%; height:100%; min-height:0; }.grande .material-symbols-outlined { font-size:190px; }.grande .halo { width:300px; height:28px; bottom:130px; }
.visual-2 { background:radial-gradient(circle at 50% 70%,rgba(88,153,255,.22),transparent 35%),linear-gradient(145deg,#1c242d,#08090b 70%); }
.visual-3 { background:radial-gradient(circle at 50% 70%,rgba(255,176,205,.24),transparent 38%),linear-gradient(145deg,#2a1822,#090708 72%); }
.visual-4 { background:radial-gradient(circle at 50% 70%,rgba(255,190,60,.24),transparent 38%),linear-gradient(145deg,#2a2315,#090807 72%); }
.visual-5 { background:radial-gradient(circle at 50% 70%,rgba(137,148,130,.24),transparent 38%),linear-gradient(145deg,#252525,#080808 72%); }
@media(max-width:700px){.grande{min-height:360px}.grande .material-symbols-outlined{font-size:130px}}
</style>

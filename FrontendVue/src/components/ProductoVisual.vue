<script setup>
const props = defineProps({
  producto: { type: Object, required: true },
  grande: { type: Boolean, default: false },
});

const iconos = {
  Consolas: 'stadia_controller',
  Accesorios: 'gamepad',
  Videojuegos: 'sports_esports',
};
</script>

<template>
  <div class="visual" :class="[`visual-${producto.id}`, { grande }]">
    <span class="halo"></span>
    <span class="material-symbols-outlined">{{ iconos[producto.categoria] || 'devices' }}</span>
    <small>{{ producto.marca }}</small>
  </div>
</template>

<style scoped>
.visual { min-height: 220px; position: relative; overflow: hidden; display:grid; place-items:center; background: radial-gradient(circle at 50% 75%, rgba(121,221,104,.28), transparent 34%), linear-gradient(145deg,#19231b,#070907 70%); }
.visual::before { content:""; position:absolute; inset:18px; border:1px solid rgba(121,221,104,.13); transform:skew(-7deg); }
.visual .material-symbols-outlined { font-size:86px; color:#dce7da; filter:drop-shadow(0 16px 18px #000); font-variation-settings:'FILL' 1; z-index:2; }
.visual small { position:absolute; bottom:16px; left:18px; font:500 10px 'JetBrains Mono'; color:#8f9a8e; letter-spacing:.12em; text-transform:uppercase; }
.halo { position:absolute; width:130px; height:18px; border-radius:50%; background:#1fa51c; filter:blur(16px); bottom:48px; }
.grande { min-height:560px; }.grande .material-symbols-outlined { font-size:190px; }.grande .halo { width:300px; height:28px; bottom:130px; }
.visual-2 { background:radial-gradient(circle at 50% 70%,rgba(88,153,255,.22),transparent 35%),linear-gradient(145deg,#1c242d,#08090b 70%); }
.visual-3 { background:radial-gradient(circle at 50% 70%,rgba(255,176,205,.24),transparent 38%),linear-gradient(145deg,#2a1822,#090708 72%); }
.visual-4 { background:radial-gradient(circle at 50% 70%,rgba(255,190,60,.24),transparent 38%),linear-gradient(145deg,#2a2315,#090807 72%); }
.visual-5 { background:radial-gradient(circle at 50% 70%,rgba(137,148,130,.24),transparent 38%),linear-gradient(145deg,#252525,#080808 72%); }
@media(max-width:700px){.grande{min-height:360px}.grande .material-symbols-outlined{font-size:130px}}
</style>

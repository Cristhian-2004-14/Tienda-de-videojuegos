<script setup>
import { computed } from 'vue';
import { useCarritoStore } from '../stores/carrito';

defineProps({
  buscador: { type: Boolean, default: true },
});

const carritoStore = useCarritoStore();
const cantidad = computed(() => carritoStore.cantidadItems);
</script>

<template>
  <div class="tienda-shell">
    <header class="tienda-header">
      <router-link to="/tienda" class="tienda-logo">X-STORE</router-link>
      <nav class="tienda-nav">
        <router-link to="/tienda">Tienda</router-link>
        <a href="#productos">Juegos</a>
        <a href="#productos">Hardware</a>
        <router-link to="/tienda/servicio/1">Servicio técnico</router-link>
      </nav>
      <div v-if="buscador" class="tienda-search">
        <span class="material-symbols-outlined">search</span>
        <input aria-label="Buscar productos" placeholder="Buscar juegos, consolas, accesorios..." />
      </div>
      <div class="tienda-actions">
        <router-link to="/tienda/carrito" class="tienda-icon" aria-label="Carrito">
          <span class="material-symbols-outlined">shopping_cart</span>
          <strong v-if="cantidad">{{ cantidad }}</strong>
        </router-link>
        <router-link to="/tienda/login" class="tienda-icon" aria-label="Cuenta">
          <span class="material-symbols-outlined">account_circle</span>
        </router-link>
      </div>
    </header>
    <main><slot /></main>
    <footer class="tienda-footer">
      <div><strong>X-STORE KINETIC</strong><span>© 2026. Todos los derechos reservados.</span></div>
      <nav><a href="#">Privacidad</a><a href="#">Términos</a><a href="#">Soporte</a></nav>
    </footer>
  </div>
</template>

<style scoped>
.tienda-shell { min-height: 100vh; background: #101010; }
.tienda-header { height: 82px; padding: 0 clamp(20px, 4vw, 54px); display: flex; align-items: center; gap: 38px; border-bottom: 1px solid #1d1d1d; position: sticky; top: 0; z-index: 30; background: rgba(16,16,16,.96); backdrop-filter: blur(14px); }
.tienda-logo { color: var(--color-primary); font-size: 29px; font-weight: 900; letter-spacing: -1.5px; }
.tienda-nav { display: flex; gap: 28px; font-size: 14px; }
.tienda-nav a { color: #d2d2d2; }
.tienda-nav a:hover, .tienda-nav .router-link-active { color: var(--color-primary); }
.tienda-search { margin-left: auto; width: min(360px, 30vw); display: flex; align-items: center; gap: 10px; padding: 10px 15px; background: #171717; border: 1px solid #292929; border-radius: 8px; }
.tienda-search input { width: 100%; border: 0; outline: 0; background: transparent; color: white; }
.tienda-actions { display: flex; gap: 10px; }
.tienda-icon { width: 42px; height: 42px; display: grid; place-items: center; color: var(--color-primary); position: relative; border-radius: 8px; }
.tienda-icon:hover { background: #202020; }
.tienda-icon strong { position: absolute; top: -1px; right: -2px; min-width: 18px; height: 18px; padding: 0 4px; display: grid; place-items: center; border-radius: 9px; font-size: 10px; background: var(--color-primary); color: #062b05; }
.tienda-footer { min-height: 130px; margin-top: 70px; padding: 35px clamp(20px, 4vw, 54px); border-top: 1px solid #2b2b2b; display: flex; justify-content: space-between; align-items: center; color: #9b9b9b; font-size: 13px; }
.tienda-footer div { display: flex; flex-direction: column; gap: 8px; }.tienda-footer nav { display: flex; gap: 30px; }
@media (max-width: 900px) { .tienda-nav { display:none; }.tienda-search { width:auto; flex:1; }.tienda-header { gap:14px; }.tienda-logo { font-size:22px; } }
@media (max-width: 600px) { .tienda-search { display:none; }.tienda-actions { margin-left:auto; }.tienda-footer { align-items:flex-start; gap:24px; flex-direction:column; }.tienda-footer nav { flex-wrap:wrap; } }
</style>

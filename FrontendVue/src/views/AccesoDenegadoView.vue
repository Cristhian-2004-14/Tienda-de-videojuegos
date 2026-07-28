<script setup>
import { useAuthStore } from '../stores/auth';
const auth=useAuthStore();
</script>
<template><main class="estado-pagina"><span class="material-symbols-outlined">lock</span><p class="mono">ERROR 403</p><h1>Acceso denegado</h1><p>Tu rol <strong>{{auth.rolActual||'actual'}}</strong> no tiene permiso para abrir este módulo.</p><router-link class="btn-primary" to="/admin">Volver al panel permitido</router-link></main></template>
<style scoped>.estado-pagina{min-height:100vh;display:grid;place-content:center;justify-items:center;text-align:center;gap:14px;padding:25px}.material-symbols-outlined{font-size:70px;color:#ffb4ab}.mono{color:#ffb4ab}.estado-pagina h1{font-size:48px}.estado-pagina>p:not(.mono){color:#a7afa4}.btn-primary{margin-top:12px}</style>

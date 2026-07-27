<script setup>
import { computed, onMounted } from 'vue';
import { storeToRefs } from 'pinia';
import TiendaLayout from '../../components/TiendaLayout.vue';
import { useDatosApiStore } from '../../stores/datosApi';
import { serviciosApi } from '../../services/recursosApi';

const props = defineProps({ id: { type: String, default: '1' } });
const datosStore = useDatosApiStore();
const { servicios } = storeToRefs(datosStore);
const servicio = computed(() => servicios.value.find(item => item.id === Number(props.id)) || servicios.value[0]);
onMounted(() => datosStore.cargarRecurso('servicios', serviciosApi));
</script>

<template>
  <TiendaLayout>
    <main class="seguimiento">
      <section class="encabezado"><p class="mono">SOPORTE / SERVICIO TÉCNICO</p><h1>Seguimiento de servicio técnico</h1><span>Consulta el estado actual y cada avance de la reparación.</span></section>
      <section class="ticket">
        <div><small>NÚMERO DE SERVICIO</small><strong class="mono">#SRV-{{ 90209 + servicio.id }}</strong></div><div><small>ESTADO ACTUAL</small><strong class="estado"><i></i>{{ servicio.estado }}</strong></div><div><small>FECHA ESTIMADA</small><strong>29 de julio</strong></div>
      </section>
      <div class="contenido">
        <section>
          <article class="equipo"><div class="equipo-icon"><span class="material-symbols-outlined">sports_esports</span></div><div><small>DISPOSITIVO EN SERVICIO</small><h2>{{ servicio.dispositivo }}</h2><p>Serie <span class="mono">XBX-84A2-{{ servicio.id }}09</span></p></div></article>
          <article class="progreso"><h2>Progreso de la reparación</h2><div class="linea"></div><div class="pasos"><div class="completo"><i><span class="material-symbols-outlined">check</span></i><b>Recibido</b><small>18 jul</small></div><div class="completo"><i><span class="material-symbols-outlined">check</span></i><b>Diagnosticado</b><small>19 jul</small></div><div class="actual"><i><span class="material-symbols-outlined">handyman</span></i><b>En reparación</b><small>En curso</small></div><div><i><span class="material-symbols-outlined">science</span></i><b>Pruebas</b><small>Pendiente</small></div><div><i><span class="material-symbols-outlined">inventory_2</span></i><b>Listo</b><small>Pendiente</small></div></div></article>
          <article class="actualizacion"><div class="fecha"><b>HOY</b><span>09:45</span></div><div><h3>Reparación iniciada</h3><p>{{ servicio.diagnostico }}. El técnico asignado está realizando el mantenimiento y las pruebas de estabilidad.</p><span class="tecnico"><span class="material-symbols-outlined">engineering</span>Técnico: M. Vargas</span></div></article>
        </section>
        <aside><article><p class="sobre">RESUMEN</p><h3>Detalles del servicio</h3><dl><div><dt>Cliente</dt><dd>{{ servicio.cliente }}</dd></div><div><dt>Ingreso</dt><dd>18 de julio de 2026</dd></div><div><dt>Diagnóstico</dt><dd>{{ servicio.diagnostico }}</dd></div><div><dt>Total estimado</dt><dd class="precio">$73.50</dd></div></dl></article><article class="ayuda"><span class="material-symbols-outlined">support_agent</span><h3>¿Necesitas ayuda?</h3><p>Habla con nuestro equipo e indica el número de servicio.</p><button>Contactar soporte</button></article></aside>
      </div>
    </main>
  </TiendaLayout>
</template>

<style scoped>
.seguimiento{max-width:1240px;margin:auto;padding:55px clamp(20px,5vw,60px)}.encabezado p,.sobre{color:#79dd68;font-size:10px;letter-spacing:.14em}.encabezado h1{font-size:clamp(34px,5vw,52px);letter-spacing:-.045em;margin:12px 0}.encabezado>span{color:#9da59b}.ticket{display:grid;grid-template-columns:repeat(3,1fr);background:#191919;border-left:4px solid #79dd68;margin:35px 0 24px}.ticket>div{padding:21px 25px;border-right:1px solid #343434}.ticket small,.equipo small{display:block;color:#7e887b;font-size:9px;letter-spacing:.1em;margin-bottom:8px}.ticket strong{font-size:15px}.ticket .estado{color:#79dd68;display:flex;align-items:center;gap:8px}.ticket i{width:7px;height:7px;border-radius:50%;background:#79dd68}.contenido{display:grid;grid-template-columns:minmax(0,1fr) 310px;gap:24px}.contenido>section,.contenido aside{display:flex;flex-direction:column;gap:18px}.equipo,.progreso,.actualizacion,aside article{background:#191919;border-radius:7px;padding:24px}.equipo{display:flex;align-items:center;gap:20px}.equipo-icon{width:70px;height:70px;display:grid;place-items:center;background:#103713;color:#79dd68}.equipo-icon span{font-size:39px}.equipo h2{font-size:22px;margin:5px 0}.equipo p{font-size:11px;color:#888}.progreso h2{font-size:18px;margin-bottom:30px}.pasos{display:grid;grid-template-columns:repeat(5,1fr);position:relative}.linea{height:2px;background:linear-gradient(90deg,#79dd68 0 50%,#3a3a3a 50%);position:relative;top:17px;margin:0 10%}.pasos>div{text-align:center;z-index:1;color:#777}.pasos i{width:35px;height:35px;display:grid;place-items:center;background:#333;border-radius:50%;margin:auto}.pasos .completo i,.pasos .actual i{background:#107c10;color:#fff}.pasos .actual i{box-shadow:0 0 0 5px #79dd6820}.pasos b,.pasos small{display:block;font-style:normal;font-size:10px;margin-top:8px}.pasos small{margin-top:4px;color:#666}.pasos .completo,.pasos .actual{color:#fff}.pasos .actual small{color:#79dd68}.actualizacion{display:grid;grid-template-columns:80px 1fr;gap:20px}.fecha b{display:block;color:#79dd68;font-size:11px}.fecha span{color:#777;font-size:10px}.actualizacion h3{margin-bottom:8px}.actualizacion p{color:#a4aaa1;line-height:1.6;font-size:13px}.tecnico{display:flex;align-items:center;gap:7px;color:#79dd68;font-size:10px;margin-top:15px}.tecnico span{font-size:17px}aside article h3{margin:9px 0 20px}dl{margin:0}dl div{padding:13px 0;border-bottom:1px solid #313131}dt{font-size:9px;color:#7c847a;text-transform:uppercase}dd{margin:5px 0 0;font-size:12px}.precio{font-size:19px;color:#79dd68;font-weight:800}.ayuda{text-align:center}.ayuda>span{font-size:38px;color:#79dd68}.ayuda p{color:#8e958c;font-size:12px;line-height:1.5}.ayuda button{width:100%;margin-top:17px;padding:11px;background:#252525;border:1px solid #393939;color:#fff;border-radius:5px}
@media(max-width:850px){.contenido{grid-template-columns:1fr}.ticket{grid-template-columns:1fr}.ticket>div{border-right:0;border-bottom:1px solid #343434}}@media(max-width:580px){.pasos b{display:none}.actualizacion{grid-template-columns:1fr}.progreso{padding:18px 10px}}
</style>

<script setup>
import { computed, onMounted } from 'vue';
import { storeToRefs } from 'pinia';
import AdminLayout from '../../components/AdminLayout.vue';
import { useDatosApiStore } from '../../stores/datosApi';
import { serviciosApi } from '../../services/recursosApi';

const props = defineProps({ id: { type: String, default: '1' } });
const datosStore = useDatosApiStore();
const { servicios } = storeToRefs(datosStore);
const servicio = computed(() => servicios.value.find(item => item.id === Number(props.id)) || servicios.value[0]);
onMounted(() => datosStore.cargarRecurso('servicios', serviciosApi));
</script>

<template>
  <AdminLayout titulo="Detalle de servicio" buscador-placeholder="Buscar ticket...">
    <template #header><div class="header"><div><router-link to="/admin/servicios" class="volver">← Volver a servicios</router-link><h2>Ticket #SRV-{{ 90209 + servicio.id }}</h2><p>Ingreso registrado el 18 de julio de 2026</p></div><span class="estado">{{ servicio.estado }}</span></div></template>
    <div class="detalle-grid">
      <section class="principal">
        <article class="equipo"><div class="icono"><span class="material-symbols-outlined">sports_esports</span></div><div><p class="eyebrow">DISPOSITIVO</p><h3>{{ servicio.dispositivo }}</h3><p>N.º de serie: <span class="mono">XBX-84A2-{{ servicio.id }}09</span></p></div></article>
        <article class="card"><h3>Diagnóstico y reparación</h3><div class="bloque"><small>Falla reportada</small><p>El equipo presenta temperatura elevada y se apaga durante sesiones de juego prolongadas.</p></div><div class="bloque"><small>Diagnóstico técnico</small><p>{{ servicio.diagnostico }}. Se realizará mantenimiento preventivo completo y pruebas de estabilidad.</p></div><label class="notas"><span>Notas del técnico</span><textarea rows="4">Equipo desmontado. Componentes internos en buen estado general. Pendiente prueba de carga.</textarea></label></article>
        <article class="card"><h3>Historial del servicio</h3><div class="timeline"><div class="paso listo"><i></i><div><strong>Equipo recibido</strong><p>18 jul · 10:32</p></div></div><div class="paso listo"><i></i><div><strong>Diagnóstico completado</strong><p>19 jul · 15:08</p></div></div><div class="paso actual"><i></i><div><strong>Reparación en curso</strong><p>Actualizado hoy · 09:45</p></div></div><div class="paso"><i></i><div><strong>Pruebas finales</strong><p>Pendiente</p></div></div></div></article>
      </section>
      <aside>
        <article class="card cliente"><p class="eyebrow">CLIENTE</p><div class="avatar">{{ servicio.cliente.split(' ').map(n=>n[0]).join('') }}</div><h3>{{ servicio.cliente }}</h3><p>alex.rivera@correo.com</p><p>+591 700 12345</p><button>Ver ficha del cliente</button></article>
        <article class="card"><h3>Resumen de costos</h3><p class="precio"><span>Diagnóstico</span><b>$20.00</b></p><p class="precio"><span>Mano de obra</span><b>$35.00</b></p><p class="precio"><span>Repuestos</span><b>$18.50</b></p><p class="precio total"><span>Total estimado</span><b>$73.50</b></p><button class="btn-primary ancho">Actualizar servicio</button></article>
      </aside>
    </div>
  </AdminLayout>
</template>

<style scoped>
.header{display:flex;justify-content:space-between;align-items:end}.header h2{font-size:31px;margin:9px 0 5px}.header p{color:#929b8f}.volver{color:#79dd68;font-size:12px}.estado{padding:8px 12px;background:#153c18;color:#79dd68;border-radius:5px;font-weight:700;font-size:12px}.detalle-grid{display:grid;grid-template-columns:minmax(0,1fr) 330px;gap:22px}.principal,aside{display:flex;flex-direction:column;gap:18px}.card,.equipo{background:#1b1b1b;border-radius:8px;padding:24px}.equipo{display:flex;align-items:center;gap:20px}.icono{width:74px;height:74px;display:grid;place-items:center;background:#103813;color:#79dd68;border-radius:7px}.icono span{font-size:39px}.equipo h3{font-size:22px;margin:5px 0}.equipo>div>p:last-child{color:#929292;font-size:12px}.eyebrow{font-size:10px;color:#79dd68;letter-spacing:.1em;font-weight:800}.card>h3{font-size:18px;margin-bottom:20px}.bloque{margin:18px 0}.bloque small,.notas span{display:block;color:#8f9b8b;font-size:11px;text-transform:uppercase;font-weight:700;margin-bottom:8px}.bloque p{line-height:1.6;color:#c7c7c7}.notas textarea{width:100%;background:#101010;border:1px solid #343434;border-radius:6px;color:#ccc;padding:14px;resize:none}.timeline{padding-left:4px}.paso{display:flex;gap:16px;position:relative;padding-bottom:24px;color:#737373}.paso:after{content:"";position:absolute;left:6px;top:14px;bottom:0;width:1px;background:#373737}.paso:last-child:after{display:none}.paso i{width:13px;height:13px;border-radius:50%;background:#414141;z-index:1}.paso.listo i,.paso.actual i{background:#79dd68}.paso.actual{color:#fff}.paso p{font-size:11px;margin-top:5px;color:#777}.cliente{text-align:center}.cliente .eyebrow{text-align:left}.avatar{width:62px;height:62px;border-radius:50%;display:grid;place-items:center;background:#31422f;margin:10px auto;color:#79dd68;font-weight:800}.cliente p{color:#929292;font-size:13px;margin:7px}.cliente button{width:100%;padding:11px;margin-top:12px;border:1px solid #3d3d3d;background:#292929;color:#fff;border-radius:6px}.precio{display:flex;justify-content:space-between;color:#aaa;margin:12px 0}.precio.total{padding-top:17px;border-top:1px solid #383838;color:#fff;font-size:18px}.ancho{width:100%;margin-top:13px}
@media(max-width:900px){.detalle-grid{grid-template-columns:1fr}.header{align-items:start;gap:15px;flex-direction:column}}
</style>

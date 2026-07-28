<script setup>
import { computed, onMounted, reactive, ref } from 'vue';
import AdminLayout from '../../components/AdminLayout.vue';
import { registrarPagoVentaApi, ventasApi } from '../../services/recursosApi';

const props = defineProps({ id: { type: String, required: true } });
const venta = ref(null);
const cargando = ref(true);
const guardando = ref(false);
const error = ref('');
const mensaje = ref('');
const formulario = reactive({ metodoPago: 'Efectivo', referencia: '', monto: 0 });
const pagado = computed(() => venta.value?.pagos?.reduce((suma, pago) => suma + Number(pago.monto), 0) || 0);
const saldo = computed(() => Math.max(0, Number(venta.value?.total || 0) - pagado.value));

async function cargar() {
  try {
    venta.value = await ventasApi.obtenerPorId(props.id);
    formulario.monto = Number(saldo.value.toFixed(2));
  } catch {
    error.value = 'No se pudo cargar la venta.';
  } finally {
    cargando.value = false;
  }
}

async function registrar() {
  error.value = '';
  mensaje.value = '';
  if (formulario.monto <= 0 || formulario.monto > saldo.value) {
    error.value = `El monto debe ser mayor a 0 y no superar el saldo de $${saldo.value.toFixed(2)}.`;
    return;
  }
  guardando.value = true;
  try {
    venta.value = await registrarPagoVentaApi(props.id, { ...formulario });
    mensaje.value = saldo.value > 0
      ? `Abono registrado. Queda un saldo de $${saldo.value.toFixed(2)}.`
      : 'Pago registrado. La venta quedó completamente cancelada.';
    formulario.monto = Number(saldo.value.toFixed(2));
    formulario.referencia = '';
  } catch (excepcion) {
    error.value = excepcion.response?.data?.message || 'No se pudo registrar el pago.';
  } finally {
    guardando.value = false;
  }
}

onMounted(cargar);
</script>

<template>
  <AdminLayout titulo="Registrar pago">
    <p v-if="cargando">Cargando venta...</p>
    <p v-else-if="!venta" class="error">{{ error }}</p>
    <div v-else class="form-shell">
      <section>
        <p class="eyebrow mono">VENTA #V-{{ String(venta.id).padStart(4, '0') }}</p>
        <h2>Registrar pago o abono</h2>
        <p class="muted">Solo deja constancia del medio usado. El sistema no procesa pagos bancarios.</p>
      </section>
      <section class="resumen">
        <article><small>TOTAL</small><strong>${{ Number(venta.total).toFixed(2) }}</strong></article>
        <article><small>PAGADO</small><strong>${{ pagado.toFixed(2) }}</strong></article>
        <article class="pendiente"><small>SALDO</small><strong>${{ saldo.toFixed(2) }}</strong></article>
      </section>
      <form v-if="saldo > 0 && venta.estado !== 'Anulada'" class="panel-caso formulario-caso" @submit.prevent="registrar">
        <div class="metodos">
          <label v-for="metodo in ['Efectivo', 'QR', 'Tarjeta']" :key="metodo" :class="{ seleccionado: formulario.metodoPago === metodo }">
            <input v-model="formulario.metodoPago" type="radio" :value="metodo">
            <span class="material-symbols-outlined">{{ metodo === 'Efectivo' ? 'payments' : metodo === 'QR' ? 'qr_code_2' : 'credit_card' }}</span>
            {{ metodo }}
          </label>
        </div>
        <div class="campo"><label>Monto a registrar</label><input v-model.number="formulario.monto" type="number" min=".01" :max="saldo" step=".01" required></div>
        <div class="campo"><label>Referencia o nota (opcional)</label><input v-model.trim="formulario.referencia" placeholder="Ej. comprobante 88421"></div>
        <p v-if="error" class="error">{{ error }}</p>
        <button class="btn-primary" type="submit" :disabled="guardando">{{ guardando ? 'Registrando...' : 'Confirmar registro' }}</button>
      </form>
      <p v-if="mensaje" class="exito">{{ mensaje }}</p>
      <p v-if="saldo === 0" class="exito">Esta venta ya está completamente cancelada.</p>
      <section class="panel-caso">
        <h3>Historial de pagos</h3>
        <p v-if="!venta.pagos?.length" class="muted">Todavía no existen pagos registrados.</p>
        <article v-for="pago in venta.pagos" :key="pago.id" class="pago">
          <div><strong>{{ pago.metodoPago }}</strong><small>{{ new Date(pago.fecha).toLocaleString('es-BO') }} {{ pago.referencia ? `· ${pago.referencia}` : '' }}</small></div>
          <b>${{ Number(pago.monto).toFixed(2) }}</b>
        </article>
      </section>
      <router-link class="accion-caso" :to="`/admin/ventas/${venta.id}`">Volver al detalle de venta</router-link>
    </div>
  </AdminLayout>
</template>

<style scoped>
.form-shell{max-width:820px;display:grid;gap:24px}.form-shell h2{font-size:32px;margin:8px 0}.resumen{display:grid;grid-template-columns:repeat(3,1fr);gap:12px}.resumen article{display:grid;gap:5px;padding:18px;background:#171717;border:1px solid #333;border-radius:8px}.resumen strong{font-size:24px}.resumen .pendiente{border-color:#79dd68}.metodos{display:grid;grid-template-columns:repeat(3,1fr);gap:12px}.metodos label{display:flex;flex-direction:column;gap:9px;padding:20px;border:1px solid #353535;border-radius:8px;background:#171717;cursor:pointer}.metodos input{position:absolute;opacity:0}.metodos .seleccionado{border-color:#79dd68;background:#132714}.pago{display:flex;justify-content:space-between;padding:13px 0;border-bottom:1px solid #333}.pago div{display:flex;flex-direction:column;gap:4px}.pago small,.muted{color:#929a90}.exito{color:#79dd68}.error{color:#ffb4ab}@media(max-width:600px){.metodos,.resumen{grid-template-columns:1fr}}
</style>

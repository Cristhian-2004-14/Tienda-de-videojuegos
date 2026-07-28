<script setup>
import { onMounted, ref } from 'vue';
import { useRouter } from 'vue-router';
import AdminLayout from '../../components/AdminLayout.vue';
import StatusBadge from '../../components/common/StatusBadge.vue';
import PaymentHistory from '../../components/payments/PaymentHistory.vue';
import PaymentSummary from '../../components/payments/PaymentSummary.vue';
import { formatearCodigo, formatearDinero, formatearFechaHora } from '../../composables/useFormatters';
import { usePaymentSummary } from '../../composables/usePaymentSummary';
import { anularVentaApi, ventasApi } from '../../services/recursosApi';

const props = defineProps({ id: { type: String, required: true } });
const router = useRouter();
const venta = ref(null);
const cargando = ref(true);
const error = ref('');
const { total, pagado, saldo } = usePaymentSummary(venta);

async function cargar() {
  try { venta.value = await ventasApi.obtenerPorId(props.id); }
  catch { error.value = 'No se encontró la venta.'; }
  finally { cargando.value = false; }
}
async function anular() {
  if (!window.confirm('¿Anular esta venta y devolver el stock?')) return;
  venta.value = await anularVentaApi(props.id);
}
function imprimir() { window.print(); }
onMounted(cargar);
</script>

<template>
  <AdminLayout titulo="Detalle de venta">
    <p v-if="cargando">Cargando venta...</p>
    <p v-else-if="error" class="error">{{ error }}</p>
    <div v-else-if="venta" class="page">
      <section class="detail-header panel-caso">
        <div>
          <p class="eyebrow mono">VENTA {{ formatearCodigo('V', venta.id) }}</p>
          <h2>{{ venta.cliente }}</h2>
          <p>{{ formatearFechaHora(venta.fecha) }} · {{ venta.empleado || 'Sin vendedor asignado' }}</p>
        </div>
        <StatusBadge :status="venta.estado" />
      </section>
      <section class="panel-caso table-wrap">
        <table class="tabla-caso">
          <thead><tr><th>Producto</th><th>Edición</th><th>Cantidad</th><th>Precio</th><th>Subtotal</th></tr></thead>
          <tbody><tr v-for="detail in venta.detalles" :key="`${detail.productoId}-${detail.edicion}`">
            <td>{{ detail.producto }}</td><td>{{ detail.edicion }}</td><td>{{ detail.cantidad }}</td>
            <td>{{ formatearDinero(detail.precioUnitario) }}</td><td>{{ formatearDinero(detail.subtotal) }}</td>
          </tr></tbody>
        </table>
      </section>
      <div class="columns">
        <PaymentHistory :payments="venta.pagos" />
        <PaymentSummary :total="total" :paid="pagado" :balance="saldo" />
      </div>
      <div class="actions">
        <button class="btn-secondary" @click="imprimir">Imprimir comprobante</button>
        <router-link v-if="saldo > 0 && venta.estado !== 'Anulada'" class="btn-primary" :to="`/admin/ventas/${venta.id}/pago`">Registrar pago</router-link>
        <button v-if="venta.estado !== 'Anulada' && !pagado" class="danger" @click="anular">Anular venta</button>
        <button class="btn-secondary" @click="router.push('/admin/ventas')">Volver</button>
      </div>
    </div>
  </AdminLayout>
</template>

<style scoped>
.page{display:grid;gap:20px}.detail-header{display:flex;justify-content:space-between;align-items:center}.detail-header h2{font-size:30px;margin:7px 0}.detail-header p:last-child{color:#929a90}.columns{display:grid;grid-template-columns:1.3fr 1fr;gap:20px}.table-wrap{overflow:auto}.actions{display:flex;gap:10px;flex-wrap:wrap}.actions a{display:inline-flex;align-items:center}.danger{border:1px solid #ffb4ab;background:#321819;color:#ffb4ab;padding:12px 18px;border-radius:8px}.error{color:#ffb4ab}@media(max-width:750px){.columns{grid-template-columns:1fr}}@media print{.actions{display:none}}
</style>

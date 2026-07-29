<script setup>
import { onMounted, ref } from 'vue';
import { useRouter } from 'vue-router';
import AdminLayout from '../../components/AdminLayout.vue';
import StatusBadge from '../../components/common/StatusBadge.vue';
import PaymentHistory from '../../components/payments/PaymentHistory.vue';
import PaymentSummary from '../../components/payments/PaymentSummary.vue';
import FacturaVenta from '../../components/ventas/FacturaVenta.vue';
import { formatearCodigo, formatearFechaHora } from '../../composables/useFormatters';
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
      <FacturaVenta class="factura-impresion" :venta="venta" />
      <div class="columns no-print">
        <PaymentHistory :payments="venta.pagos" />
        <PaymentSummary :total="total" :paid="pagado" :balance="saldo" />
      </div>
      <div class="actions no-print">
        <button class="btn-secondary" @click="imprimir">Imprimir factura</button>
        <router-link v-if="saldo > 0 && venta.estado !== 'Anulada'" class="btn-primary" :to="`/admin/ventas/${venta.id}/pago`">Registrar pago</router-link>
        <button v-if="venta.estado !== 'Anulada' && !pagado" class="danger" @click="anular">Anular venta</button>
        <button class="btn-secondary" @click="router.push('/admin/ventas')">Volver</button>
      </div>
    </div>
  </AdminLayout>
</template>

<style scoped>
.page{display:grid;gap:20px}.detail-header{display:flex;justify-content:space-between;align-items:center}.detail-header h2{font-size:30px;margin:7px 0}.detail-header p:last-child{color:#929a90}.columns{display:grid;grid-template-columns:1.3fr 1fr;gap:20px}.actions{display:flex;gap:10px;flex-wrap:wrap}.actions a{display:inline-flex;align-items:center}.danger{border:1px solid #ffb4ab;background:#321819;color:#ffb4ab;padding:12px 18px;border-radius:8px}.error{color:#ffb4ab}@media(max-width:750px){.columns{grid-template-columns:1fr}.detail-header{align-items:flex-start;flex-direction:column;gap:12px}}@media print{:global(body){background:#fff!important}.no-print,.detail-header,:global(.sidebar),:global(.topbar){display:none!important}:global(.admin-main){margin:0!important}:global(.content){display:block!important;max-width:none!important;padding:0!important}.page{display:block}.factura-impresion{box-shadow:none;border-radius:0;min-height:100vh}}
</style>

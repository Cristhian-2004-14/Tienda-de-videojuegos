<script setup>
import { onMounted, ref } from 'vue';
import AdminLayout from '../../components/AdminLayout.vue';
import AdminPageHeader from '../../components/common/AdminPageHeader.vue';
import PaymentForm from '../../components/payments/PaymentForm.vue';
import PaymentHistory from '../../components/payments/PaymentHistory.vue';
import PaymentSummary from '../../components/payments/PaymentSummary.vue';
import { formatearCodigo, formatearDinero } from '../../composables/useFormatters';
import { usePaymentSummary } from '../../composables/usePaymentSummary';
import { registrarPagoVentaApi, ventasApi } from '../../services/recursosApi';

const props = defineProps({ id: { type: String, required: true } });
const venta = ref(null);
const cargando = ref(true);
const guardando = ref(false);
const error = ref('');
const mensaje = ref('');
const { total, pagado, saldo } = usePaymentSummary(venta);

async function cargar() {
  try {
    venta.value = await ventasApi.obtenerPorId(props.id);
  } catch {
    error.value = 'No se pudo cargar la venta.';
  } finally {
    cargando.value = false;
  }
}

async function registrar(pago) {
  error.value = '';
  mensaje.value = '';
  if (pago.monto <= 0 || pago.monto > saldo.value) {
    error.value = `El monto debe ser mayor a 0 y no superar ${formatearDinero(saldo.value)}.`;
    return;
  }
  guardando.value = true;
  try {
    venta.value = await registrarPagoVentaApi(props.id, pago);
    mensaje.value = saldo.value > 0
      ? `Abono registrado. Queda un saldo de ${formatearDinero(saldo.value)}.`
      : 'Pago registrado. La venta quedó completamente cancelada.';
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
    <div v-else class="page">
      <AdminPageHeader
        :eyebrow="`VENTA ${formatearCodigo('V', venta.id)}`"
        title="Registrar pago o abono"
        description="Este registro es administrativo; el sistema no procesa pagos bancarios."
      />
      <PaymentSummary :total="total" :paid="pagado" :balance="saldo" />
      <PaymentForm
        v-if="saldo > 0 && venta.estado !== 'Anulada'"
        :max="saldo"
        :loading="guardando"
        submit-text="Confirmar registro"
        @submit="registrar"
      />
      <p v-if="error" class="error">{{ error }}</p>
      <p v-if="mensaje" class="success">{{ mensaje }}</p>
      <p v-if="saldo === 0" class="success">Esta venta ya está completamente cancelada.</p>
      <PaymentHistory :payments="venta.pagos" />
      <router-link class="accion-caso" :to="`/admin/ventas/${venta.id}`">
        Volver al detalle de venta
      </router-link>
    </div>
  </AdminLayout>
</template>

<style scoped>
.page{max-width:840px;display:grid;gap:24px}.error{color:#ffb4ab}.success{color:#79dd68}
</style>

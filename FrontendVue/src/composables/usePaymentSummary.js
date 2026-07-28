import { computed, unref } from 'vue';

export function usePaymentSummary(entidad) {
  const total = computed(() => Number(unref(entidad)?.total || 0));
  const pagado = computed(() =>
    (unref(entidad)?.pagos || []).reduce((suma, pago) => suma + Number(pago.monto || 0), 0));
  const saldo = computed(() => Math.max(0, total.value - pagado.value));

  return { total, pagado, saldo };
}

<script setup>
import { computed } from 'vue';
import { formatearDinero, formatearFechaHora } from '../../composables/useFormatters';

const props = defineProps({
  venta: { type: Object, required: true },
});

const numeroFactura = computed(() => `F-${String(props.venta.id).padStart(6, '0')}`);
const nombreCliente = computed(() => props.venta.cliente || 'Consumidor final');
</script>

<template>
  <section class="factura">
    <header class="factura-header">
      <div>
        <p class="marca">X-STORE KINETIC</p>
        <h2>Factura de venta</h2>
        <small>Comprobante interno de la operación</small>
      </div>
      <div class="numero">
        <span>N.º DE FACTURA</span>
        <strong>{{ numeroFactura }}</strong>
        <small>{{ formatearFechaHora(venta.fecha) }}</small>
      </div>
    </header>

    <div class="datos">
      <article>
        <span>FACTURAR A</span>
        <strong>{{ nombreCliente }}</strong>
        <p>CI / NIT: {{ venta.clienteCi || 'No informado' }}</p>
        <p>Teléfono: {{ venta.clienteTelefono || 'No informado' }}</p>
        <p v-if="venta.clienteEmail">Correo: {{ venta.clienteEmail }}</p>
        <p v-if="venta.clienteDireccion">Dirección: {{ venta.clienteDireccion }}</p>
      </article>
      <article>
        <span>INFORMACIÓN DE VENTA</span>
        <p>Tipo de cliente: {{ venta.tipoCliente || (venta.clienteId ? 'Registrado' : 'Ocasional') }}</p>
        <p>Vendedor: {{ venta.empleado || 'Sin vendedor asignado' }}</p>
        <p>Estado: {{ venta.estado }}</p>
      </article>
    </div>

    <div class="tabla-factura">
      <table>
        <thead>
          <tr><th>Producto</th><th>Cant.</th><th>Precio unitario</th><th>Subtotal</th></tr>
        </thead>
        <tbody>
          <tr v-for="detalle in venta.detalles" :key="`${detalle.productoId}-${detalle.edicion}`">
            <td><strong>{{ detalle.producto }}</strong><small>{{ detalle.edicion }}</small></td>
            <td>{{ detalle.cantidad }}</td>
            <td>{{ formatearDinero(detalle.precioUnitario) }}</td>
            <td>{{ formatearDinero(detalle.subtotal) }}</td>
          </tr>
        </tbody>
      </table>
    </div>

    <footer>
      <div class="nota">
        <strong>Gracias por su compra.</strong>
        <p>Conserve este documento como referencia de la operación.</p>
      </div>
      <div class="importes">
        <p><span>Subtotal</span><strong>{{ formatearDinero(venta.subtotal) }}</strong></p>
        <p><span>Descuento</span><strong>- {{ formatearDinero(venta.descuento) }}</strong></p>
        <p class="total"><span>Total</span><strong>{{ formatearDinero(venta.total) }}</strong></p>
        <p><span>Pagado</span><strong>{{ formatearDinero(venta.pagos?.reduce((suma, pago) => suma + pago.monto, 0) || 0) }}</strong></p>
        <p><span>Saldo</span><strong>{{ formatearDinero(venta.saldoPendiente) }}</strong></p>
      </div>
    </footer>
  </section>
</template>

<style scoped>
.factura{--factura-fondo:var(--color-surface-container-low);--factura-texto:#eef2ec;--factura-secundario:#929d90;--factura-borde:#343a34;--factura-cabecera:#222722;padding:34px;background:var(--factura-fondo);color:var(--factura-texto);border:1px solid var(--factura-borde);border-radius:10px;box-shadow:0 14px 40px #0004}.factura-header{display:flex;justify-content:space-between;gap:30px;padding-bottom:25px;border-bottom:3px solid #107c10}.marca{font:800 13px 'JetBrains Mono';letter-spacing:.15em;color:var(--color-primary)}.factura h2{margin:5px 0;font-size:30px;text-transform:uppercase}.factura-header small{color:var(--factura-secundario)}.numero{text-align:right;display:flex;flex-direction:column;gap:4px}.numero span,.datos span{font-size:9px;font-weight:800;letter-spacing:.12em;color:var(--factura-secundario)}.numero strong{font:700 20px 'JetBrains Mono'}.datos{display:grid;grid-template-columns:1fr 1fr;gap:30px;padding:25px 0}.datos article{display:flex;flex-direction:column;gap:5px}.datos article>strong{font-size:17px;margin:3px 0}.datos p{font-size:12px;color:var(--factura-secundario)}.tabla-factura{overflow:auto}.tabla-factura table{width:100%;border-collapse:collapse}.tabla-factura th{padding:11px;text-align:left;background:var(--factura-cabecera);color:#bdc7ba;font-size:10px;text-transform:uppercase}.tabla-factura th:not(:first-child),.tabla-factura td:not(:first-child){text-align:right}.tabla-factura td{padding:13px 11px;border-bottom:1px solid var(--factura-borde);font-size:12px}.tabla-factura td:first-child{display:flex;flex-direction:column;gap:3px}.tabla-factura td small{color:var(--factura-secundario)}.factura footer{display:grid;grid-template-columns:1fr minmax(260px,.55fr);gap:40px;padding-top:25px}.nota{align-self:end;color:var(--factura-secundario);font-size:12px}.nota strong{color:var(--color-primary)}.importes p{display:flex;justify-content:space-between;padding:6px 0}.importes .total{margin:5px 0;padding:12px 0;border-block:2px solid #107c10;font-size:18px}.importes .total strong{color:var(--color-primary)}@media(max-width:650px){.factura{padding:20px}.factura-header,.datos{grid-template-columns:1fr;display:grid}.numero{text-align:left}.factura footer{grid-template-columns:1fr}.nota{order:2}}@media print{.factura{--factura-fondo:#fff;--factura-texto:#182018;--factura-secundario:#596459;--factura-borde:#d7ddd6;--factura-cabecera:#e2e9e1;border:0;color:#182018}.marca,.nota strong,.importes .total strong{color:#107c10}.tabla-factura th{color:#3d493d}}
</style>

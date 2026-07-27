<script setup>
import { computed, onMounted } from 'vue';
import { storeToRefs } from 'pinia';
import TiendaLayout from '../../components/TiendaLayout.vue';
import ProductoVisual from '../../components/ProductoVisual.vue';
import { useDatosApiStore } from '../../stores/datosApi';
import { productosApi } from '../../services/recursosApi';

const datosStore = useDatosApiStore();
const { productos } = storeToRefs(datosStore);
const items = computed(() => productos.value.slice(0, 3).map((producto) => ({ ...producto, cantidad: 1 })));
onMounted(() => datosStore.cargarRecurso('productos', productosApi));
</script>

<template>
  <TiendaLayout>
    <main class="checkout">
      <p class="pasos mono"><b>01 CARRITO</b><span></span>02 ENVÍO<span></span>03 CONFIRMACIÓN</p>
      <h1>Finalizar compra</h1>
      <div class="checkout-grid">
        <section><h2>Tu carrito <small>(3)</small></h2><div class="items"><article v-for="item in items" :key="item.id"><div class="visual"><ProductoVisual :tipo="item.categoria" /></div><div class="item-info"><small>{{ item.categoria }}</small><h3>{{ item.nombre }}</h3><p>{{ item.marca }} · Garantía oficial</p><button>Eliminar</button></div><div class="item-precio"><strong>${{ item.precioVenta.toFixed(2) }}</strong><div><button>−</button><span>{{ item.cantidad }}</span><button>+</button></div></div></article></div><router-link to="/tienda" class="seguir">← Seguir comprando</router-link></section>
        <aside>
          <div class="resumen"><h2>Resumen de pedido</h2><p><span>Subtotal</span><strong>$629.97</strong></p><p><span>Envío</span><strong class="gratis">Gratis</strong></p><p><span>Impuestos</span><strong>$81.90</strong></p><p class="total"><span>Total</span><strong>$711.87</strong></p></div>
          <form><h3>Información de envío</h3><label>Nombre completo<input placeholder="John Wick" /></label><label>Dirección<input placeholder="Av. Principal 123" /></label><div><label>Ciudad<input placeholder="La Paz" /></label><label>Código postal<input placeholder="0000" /></label></div><h3>Método de pago</h3><label class="tarjeta"><span class="material-symbols-outlined">credit_card</span><input placeholder="Número de tarjeta" /></label><button type="button" class="pagar">Pagar $711.87</button><p class="seguro"><span class="material-symbols-outlined">lock</span>Pago protegido y cifrado</p></form>
        </aside>
      </div>
    </main>
  </TiendaLayout>
</template>

<style scoped>
.checkout{max-width:1280px;margin:auto;padding:45px clamp(20px,5vw,70px)}.pasos{display:flex;align-items:center;gap:12px;color:#626762;font-size:9px}.pasos b{color:#79dd68}.pasos span{height:1px;width:38px;background:#373737}.checkout>h1{font-size:42px;margin:20px 0 42px}.checkout-grid{display:grid;grid-template-columns:minmax(0,1.2fr) minmax(350px,.8fr);gap:60px}.checkout h2{font-size:20px;margin-bottom:19px}.checkout h2 small{color:#777}.items{border-top:1px solid #313131}.items article{display:grid;grid-template-columns:120px 1fr auto;gap:20px;padding:20px 0;border-bottom:1px solid #313131}.visual{height:115px;display:grid;place-items:center;background:#202020;border-radius:6px;padding:25px}.item-info small{color:#79dd68;text-transform:uppercase;font-size:9px}.item-info h3{font-size:17px;margin:7px 0}.item-info p{color:#888;font-size:11px}.item-info button{border:0;background:transparent;color:#8c8c8c;text-decoration:underline;padding:15px 0 0}.item-precio{text-align:right}.item-precio>div{display:flex;margin-top:30px}.item-precio button,.item-precio span{width:29px;height:29px;display:grid;place-items:center;border:1px solid #353535;background:#1d1d1d;color:#fff}.seguir{display:inline-block;color:#79dd68;margin-top:22px;font-size:12px}.resumen,form{background:#1a1a1a;padding:25px}.resumen p{display:flex;justify-content:space-between;color:#9a9a9a;margin:13px 0}.resumen .gratis{color:#79dd68}.resumen .total{border-top:1px solid #363636;padding-top:20px;color:#fff;font-size:20px}.resumen .total strong{color:#79dd68}form{margin-top:2px}form h3{font-size:11px;text-transform:uppercase;letter-spacing:.12em;color:#becab7;margin:10px 0 16px}form label{display:flex;flex-direction:column;gap:7px;font-size:10px;color:#91998e;margin-bottom:13px;text-transform:uppercase;font-weight:700}form input{background:#101010;border:1px solid #303030;color:#eee;padding:12px;border-radius:5px}form>div{display:grid;grid-template-columns:1fr 1fr;gap:12px}.tarjeta{position:relative}.tarjeta span{position:absolute;left:11px;bottom:10px;font-size:20px}.tarjeta input{padding-left:40px}.pagar{width:100%;padding:15px;border:0;background:#107c10;color:white;border-radius:5px;font-weight:900;margin-top:8px}.seguro{display:flex;justify-content:center;align-items:center;gap:5px;color:#777;font-size:9px;margin-top:12px}.seguro span{font-size:12px}
@media(max-width:900px){.checkout-grid{grid-template-columns:1fr}.checkout-grid{gap:40px}}@media(max-width:550px){.items article{grid-template-columns:85px 1fr}.item-precio{grid-column:2;display:flex;justify-content:space-between;align-items:center}.item-precio>div{margin:0}}
</style>

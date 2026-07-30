<script setup>
import { computed, onMounted, ref, watch } from 'vue';
import { storeToRefs } from 'pinia';
import TiendaLayout from '../../components/TiendaLayout.vue';
import ProductoVisual from '../../components/ProductoVisual.vue';
import { useDatosApiStore } from '../../stores/datosApi';
import { productosApi } from '../../services/recursosApi';
import { useCarritoStore } from '../../stores/carrito';

const props = defineProps({ id: { type: String, default: '1' } });
const datosStore = useDatosApiStore();
const carritoStore = useCarritoStore();
const { productos } = storeToRefs(datosStore);
const productoCompleto = ref(null);
const producto = computed(() => productoCompleto.value
  || productos.value.find(item => item.id === Number(props.id))
  || productos.value[0]);
const precioSeleccionado = computed(() => producto.value?.precioVenta || 0);
async function cargarProducto() {
  await datosStore.cargarRecurso('productos', productosApi);
  productoCompleto.value = await productosApi.obtenerPorId(props.id);
}
watch(() => props.id, cargarProducto);
onMounted(cargarProducto);
</script>

<template>
  <TiendaLayout>
    <main class="detalle">
      <nav class="migas"><router-link to="/tienda">Tienda</router-link><span>/</span><span>{{ producto.categoria }}</span><span>/</span><strong>{{ producto.nombre }}</strong></nav>
      <section class="producto">
        <div class="galeria"><div class="principal"><span class="edition mono">ENGINEERED FOR PLAY</span><ProductoVisual :producto="producto" grande /></div></div>
        <div class="compra"><p class="categoria">{{ producto.categoria }} / {{ producto.marca }}</p><h1>{{ producto.nombre }}</h1><div class="rating"><span>★★★★★</span><small>4.9 · 248 reseñas</small></div><p class="descripcion">{{ producto.descripcion?.trim() || 'Este producto todavía no tiene una descripción disponible.' }}</p><div class="precio">${{ precioSeleccionado.toFixed(2) }} <small>Impuestos incluidos</small></div>
          <div class="stock"><span></span>{{ producto.stock > 0 ? `Disponible · ${producto.stock} unidades` : 'Sin existencias' }}</div>
          <button class="agregar" :disabled="producto.stock <= 0" @click="carritoStore.agregarProducto(producto)"><span class="material-symbols-outlined">shopping_bag</span>Agregar a mi selección</button>
        </div>
      </section>
    </main>
  </TiendaLayout>
</template>

<style scoped>
.detalle{max-width:1440px;margin:auto;padding:25px clamp(20px,5vw,70px) 0}.migas{display:flex;gap:10px;color:#737b71;font-size:11px;margin:12px 0 32px}.migas strong{color:#b9c1b6}.producto{display:grid;grid-template-columns:minmax(0,1.25fr) minmax(360px,.75fr);gap:64px}.principal{height:570px;display:grid;place-items:center;background:radial-gradient(circle,#2c3b2b,#181818 60%);border-radius:8px;padding:100px;position:relative}.principal :deep(.producto-visual){transform:scale(1.6)}.edition{position:absolute;top:20px;left:20px;color:#79dd68;font-size:9px}.miniaturas{display:flex;gap:10px;margin-top:12px}.miniaturas button{width:72px;height:66px;border:1px solid #2e2e2e;background:#1d1d1d;color:#aaa;border-radius:5px;padding:13px}.miniaturas .activa{border-color:#79dd68}.compra{padding-top:17px}.categoria{font-size:10px;color:#79dd68;letter-spacing:.15em;font-weight:900;text-transform:uppercase}.compra h1{font-size:clamp(42px,5vw,68px);line-height:.98;text-transform:uppercase;letter-spacing:-.055em;margin:13px 0}.rating{display:flex;gap:12px;align-items:center}.rating span{color:#79dd68;letter-spacing:2px}.rating small{color:#899185}.descripcion{line-height:1.65;color:#afb5ad;margin:24px 0}.precio{font-size:31px;font-weight:800}.precio small{font-size:10px;color:#888;font-weight:400;margin-left:8px}.stock{font-size:11px;color:#9ea99b;display:flex;align-items:center;gap:8px;margin-top:24px}.stock span{width:8px;height:8px;background:#79dd68;border-radius:50%}.agregar{width:100%;border:0;padding:15px;margin-top:14px;border-radius:5px;font-weight:900;display:flex;align-items:center;justify-content:center;gap:9px;background:#107c10;color:#fff}
@media(max-width:900px){.producto{grid-template-columns:1fr}.principal{height:420px}.compra{padding-top:0}}@media(max-width:550px){.principal{height:300px;padding:60px}.beneficios{grid-template-columns:1fr}}
.agregar:disabled{opacity:.45;cursor:not-allowed}
.principal{height:610px;padding:0;overflow:hidden;background:#121512;border:1px solid #ffffff12;border-radius:14px;position:relative;box-shadow:0 28px 70px #0007}.principal :deep(.visual){width:100%;height:100%;min-height:0;transform:none}.principal:after{content:"";position:absolute;inset:0;z-index:2;border:1px solid #ffffff0d;border-radius:inherit;box-shadow:inset 0 -100px 100px #0003;pointer-events:none}.edition{z-index:4;padding:7px 10px;border:1px solid #79dd6860;background:#0b110bd9;color:#9ff18f;border-radius:4px}
@media(max-width:900px){.principal{height:480px}}@media(max-width:550px){.principal{height:340px;padding:0}}
</style>

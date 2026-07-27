<script setup>
import { computed, onMounted } from 'vue';
import { storeToRefs } from 'pinia';
import TiendaLayout from '../../components/TiendaLayout.vue';
import ProductoVisual from '../../components/ProductoVisual.vue';
import { useDatosApiStore } from '../../stores/datosApi';
import { productosApi } from '../../services/recursosApi';

const props = defineProps({ id: { type: String, default: '1' } });
const datosStore = useDatosApiStore();
const { productos } = storeToRefs(datosStore);
const producto = computed(() => productos.value.find(item => item.id === Number(props.id)) || productos.value[0]);
onMounted(() => datosStore.cargarRecurso('productos', productosApi));
</script>

<template>
  <TiendaLayout>
    <main class="detalle">
      <nav class="migas"><router-link to="/tienda">Tienda</router-link><span>/</span><span>{{ producto.categoria }}</span><span>/</span><strong>{{ producto.nombre }}</strong></nav>
      <section class="producto">
        <div class="galeria"><div class="principal"><span class="edition mono">ENGINEERED FOR PLAY</span><ProductoVisual :tipo="producto.categoria" /></div><div class="miniaturas"><button class="activa"><ProductoVisual :tipo="producto.categoria" /></button><button><span class="material-symbols-outlined">360</span></button><button><span class="material-symbols-outlined">play_circle</span></button></div></div>
        <div class="compra"><p class="categoria">{{ producto.categoria }} / {{ producto.marca }}</p><h1>{{ producto.nombre }}</h1><div class="rating"><span>★★★★★</span><small>4.9 · 248 reseñas</small></div><p class="descripcion">Rendimiento de nueva generación, velocidad extraordinaria y una experiencia diseñada para que nada se interponga entre tú y el juego.</p><div class="precio">${{ producto.precioVenta.toFixed(2) }} <small>Impuestos incluidos</small></div>
          <div class="opciones"><label>Edición</label><div><button class="activa">Estándar</button><button>Digital</button></div></div>
          <div class="stock"><span></span>{{ producto.stock > 0 ? `Disponible · ${producto.stock} unidades` : 'Sin existencias' }}</div>
          <button class="agregar"><span class="material-symbols-outlined">shopping_bag</span>Agregar al carrito</button>
          <button class="favorito"><span class="material-symbols-outlined">favorite</span>Guardar en favoritos</button>
          <div class="beneficios"><p><span class="material-symbols-outlined">local_shipping</span><b>Envío gratuito</b><small>Entrega estimada en 2–4 días</small></p><p><span class="material-symbols-outlined">verified_user</span><b>Garantía oficial</b><small>Cobertura por 12 meses</small></p></div>
        </div>
      </section>
      <section class="caracteristicas"><div><p class="categoria">MÁS ALLÁ DE LA VELOCIDAD</p><h2>Domina tu entorno</h2><p>Arquitectura optimizada para mantener imágenes nítidas, tiempos de carga mínimos y una respuesta inmediata.</p></div><div class="specs"><article><strong>4K</strong><span>Resolución nativa</span></article><article><strong>120</strong><span>Fotogramas por segundo</span></article><article><strong>1 TB</strong><span>Almacenamiento SSD</span></article><article><strong>8K</strong><span>Alto rango dinámico</span></article></div></section>
    </main>
  </TiendaLayout>
</template>

<style scoped>
.detalle{max-width:1440px;margin:auto;padding:25px clamp(20px,5vw,70px) 0}.migas{display:flex;gap:10px;color:#737b71;font-size:11px;margin:12px 0 32px}.migas strong{color:#b9c1b6}.producto{display:grid;grid-template-columns:minmax(0,1.25fr) minmax(360px,.75fr);gap:64px}.principal{height:570px;display:grid;place-items:center;background:radial-gradient(circle,#2c3b2b,#181818 60%);border-radius:8px;padding:100px;position:relative}.principal :deep(.producto-visual){transform:scale(1.6)}.edition{position:absolute;top:20px;left:20px;color:#79dd68;font-size:9px}.miniaturas{display:flex;gap:10px;margin-top:12px}.miniaturas button{width:72px;height:66px;border:1px solid #2e2e2e;background:#1d1d1d;color:#aaa;border-radius:5px;padding:13px}.miniaturas .activa{border-color:#79dd68}.compra{padding-top:17px}.categoria{font-size:10px;color:#79dd68;letter-spacing:.15em;font-weight:900;text-transform:uppercase}.compra h1{font-size:clamp(42px,5vw,68px);line-height:.98;text-transform:uppercase;letter-spacing:-.055em;margin:13px 0}.rating{display:flex;gap:12px;align-items:center}.rating span{color:#79dd68;letter-spacing:2px}.rating small{color:#899185}.descripcion{line-height:1.65;color:#afb5ad;margin:24px 0}.precio{font-size:31px;font-weight:800}.precio small{font-size:10px;color:#888;font-weight:400;margin-left:8px}.opciones{margin:24px 0}.opciones label{display:block;font-size:11px;font-weight:800;margin-bottom:9px}.opciones button{padding:10px 17px;background:#1e1e1e;border:1px solid #343434;color:#bbb}.opciones button:first-child{border-radius:5px 0 0 5px}.opciones button:last-child{border-radius:0 5px 5px 0}.opciones .activa{border-color:#79dd68;color:#fff}.stock{font-size:11px;color:#9ea99b;display:flex;align-items:center;gap:8px}.stock span{width:8px;height:8px;background:#79dd68;border-radius:50%}.agregar,.favorito{width:100%;border:0;padding:15px;margin-top:14px;border-radius:5px;font-weight:900;display:flex;align-items:center;justify-content:center;gap:9px}.agregar{background:#107c10;color:#fff}.favorito{background:#252525;color:#ddd}.beneficios{display:grid;grid-template-columns:1fr 1fr;gap:12px;margin-top:18px}.beneficios p{display:grid;grid-template-columns:auto 1fr;column-gap:9px;background:#181818;padding:14px}.beneficios span{grid-row:1/3;color:#79dd68}.beneficios b{font-size:11px}.beneficios small{font-size:9px;color:#777;margin-top:3px}.caracteristicas{margin:90px 0 30px;padding:50px;background:#191919;display:grid;grid-template-columns:.8fr 1.2fr;gap:60px}.caracteristicas h2{font-size:38px;margin:12px 0}.caracteristicas>div>p:last-child{color:#9ba097;line-height:1.6}.specs{display:grid;grid-template-columns:repeat(2,1fr);gap:1px;background:#393939}.specs article{background:#191919;padding:24px}.specs strong{font-size:30px;color:#79dd68;display:block}.specs span{font-size:11px;color:#92998f}
@media(max-width:900px){.producto{grid-template-columns:1fr}.principal{height:420px}.caracteristicas{grid-template-columns:1fr}.compra{padding-top:0}}@media(max-width:550px){.principal{height:300px;padding:60px}.beneficios{grid-template-columns:1fr}.caracteristicas{padding:28px}.specs{grid-template-columns:1fr}}
</style>
